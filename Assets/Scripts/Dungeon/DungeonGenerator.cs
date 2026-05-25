using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Impostazioni Dungeon")]
    public Room startRoomPrefab;
    public List<Room> roomPrefabs;
    public int maxRooms = 6;

    private List<Room> spawnedRooms = new List<Room>();
    private Queue<RoomConnector> connectorQueue = new Queue<RoomConnector>();

    private struct RoomConnector
    {
        public Room parentRoom;
        public Room.DoorDirection direction;

        public RoomConnector(Room parent, Room.DoorDirection dir)
        {
            parentRoom = parent;
            direction = dir;
        }
    }

    void Start()
    {
        Generate();
    }

    [Header("Popolamento Stanze")]
    public GameObject platformPrefab;
    public List<GameObject> enemyPrefabs;
    public GameObject gatePrefab;
    public int platformsPerRoom = 3;
    public int enemiesPerRoom = 2;

    [ContextMenu("Generate Dungeon")]
    public void Generate()
    {
        ClearDungeon();

        if (startRoomPrefab == null)
        {
            Debug.LogError("Start Room Prefab non assegnato!");
            return;
        }

        Room startRoom = Instantiate(startRoomPrefab, Vector3.zero, Quaternion.identity, transform);
        SetupRoom(startRoom, false); // La stanza iniziale di solito è vuota o speciale
        spawnedRooms.Add(startRoom);
        AddConnectors(startRoom);

        int attempts = 0;
        int maxAttempts = 500; 

        while (spawnedRooms.Count < maxRooms && connectorQueue.Count > 0 && attempts < maxAttempts)
        {
            attempts++;
            RoomConnector connector = connectorQueue.Dequeue();

            Room newRoom = TrySpawnRoom(connector);
            if (newRoom != null)
            {
                SetupRoom(newRoom, true);
                spawnedRooms.Add(newRoom);
                AddConnectors(newRoom);
            }
        }

        CloseUnusedExits();
        
        Debug.Log($"Dungeon generato con {spawnedRooms.Count} stanze.");
    }

    private void SetupRoom(Room room, bool populate)
    {
        room.platformPrefab = platformPrefab;
        room.enemyPrefabs = enemyPrefabs;
        room.gatePrefab = gatePrefab;

        if (populate)
        {
            room.PopulateRoom(platformsPerRoom, enemiesPerRoom);
        }
    }

    private void ClearDungeon()
    {
        foreach (var room in spawnedRooms)
        {
            if (room != null) 
            {
                if (Application.isPlaying) Destroy(room.gameObject);
                else DestroyImmediate(room.gameObject);
            }
        }
        spawnedRooms.Clear();
        connectorQueue.Clear();
    }

    private void AddConnectors(Room room)
    {
        if (room.topExit) connectorQueue.Enqueue(new RoomConnector(room, Room.DoorDirection.Top));
        if (room.bottomExit) connectorQueue.Enqueue(new RoomConnector(room, Room.DoorDirection.Bottom));
        if (room.leftExit) connectorQueue.Enqueue(new RoomConnector(room, Room.DoorDirection.Left));
        if (room.rightExit) connectorQueue.Enqueue(new RoomConnector(room, Room.DoorDirection.Right));
    }

    private Vector2 GetSpawnPosition(Room parent, Room prefab, Room.DoorDirection dir)
{
        Vector2 pos = parent.transform.position;
        switch (dir)
        {
            case Room.DoorDirection.Top:
                pos.y += (parent.roomSize.y / 2f) + (prefab.roomSize.y / 2f);
                break;
            case Room.DoorDirection.Bottom:
                pos.y -= (parent.roomSize.y / 2f) + (prefab.roomSize.y / 2f);
                break;
            case Room.DoorDirection.Left:
                pos.x -= (parent.roomSize.x / 2f) + (prefab.roomSize.x / 2f);
                break;
            case Room.DoorDirection.Right:
                pos.x += (parent.roomSize.x / 2f) + (prefab.roomSize.x / 2f);
                break;
        }
        return pos;
    }

    private Room TrySpawnRoom(RoomConnector connector)
    {
        List<Room> validPrefabs = new List<Room>();
        Room.DoorDirection requiredEntry = GetOpposite(connector.direction);

        foreach (var prefab in roomPrefabs)
        {
            if (HasExit(prefab, requiredEntry))
                validPrefabs.Add(prefab);
        }

        if (validPrefabs.Count == 0) return null;

        validPrefabs.Shuffle();

        foreach (var prefab in validPrefabs)
        {
            Vector2 targetPos = GetSpawnPosition(connector.parentRoom, prefab, connector.direction);

            if (!IsOverlapping(targetPos, prefab.roomSize))
            {
                Room spawned = Instantiate(prefab, (Vector3)targetPos, Quaternion.identity, transform);
                return spawned;
            }
        }

        return null;
    }

    private bool IsOverlapping(Vector2 pos, Vector2 size)
    {
        Rect newRoomRect = new Rect(pos - size / 2f, size);
        newRoomRect.min += Vector2.one * 0.1f; // Tolleranza maggiore per evitare sovrapposizioni millimetriche
        newRoomRect.max -= Vector2.one * 0.1f;

        foreach (var room in spawnedRooms)
        {
            Rect existingRect = new Rect((Vector2)room.transform.position - room.roomSize / 2f, room.roomSize);
            if (newRoomRect.Overlaps(existingRect)) return true;
        }
        return false;
    }

    private void CloseUnusedExits()
    {
        foreach (var room in spawnedRooms)
        {
            CheckAndClose(room, Room.DoorDirection.Top);
            CheckAndClose(room, Room.DoorDirection.Bottom);
            CheckAndClose(room, Room.DoorDirection.Left);
            CheckAndClose(room, Room.DoorDirection.Right);
        }
    }

    private void CheckAndClose(Room room, Room.DoorDirection dir)
    {
        bool hasDesiredExit = HasExit(room, dir);
        if (!hasDesiredExit) return;

        bool connected = false;
        Vector2 dirVec = Vector2.zero;
        switch (dir)
        {
            case Room.DoorDirection.Top: dirVec = Vector2.up; break;
            case Room.DoorDirection.Bottom: dirVec = Vector2.down; break;
            case Room.DoorDirection.Left: dirVec = Vector2.left; break;
            case Room.DoorDirection.Right: dirVec = Vector2.right; break;
        }

        foreach (var other in spawnedRooms)
        {
            if (other == room) continue;

            // Calcoliamo se la stanza "other" si trova nella direzione "dir" rispetto a "room"
            Vector2 toOther = (Vector2)other.transform.position - (Vector2)room.transform.position;
            float dist = toOther.magnitude;
            
            // Distanza teorica se fossero perfettamente adiacenti
            float expectedDist = 0;
            if (dir == Room.DoorDirection.Top || dir == Room.DoorDirection.Bottom)
                expectedDist = (room.roomSize.y / 2f) + (other.roomSize.y / 2f);
            else
                expectedDist = (room.roomSize.x / 2f) + (other.roomSize.x / 2f);

            // Verifichiamo se sono adiacenti e allineati
            if (Mathf.Abs(dist - expectedDist) < 0.2f)
            {
                if (Vector2.Dot(toOther.normalized, dirVec) > 0.9f)
                {
                    if (HasExit(other, GetOpposite(dir)))
                    {
                        connected = true;
                        break;
                    }
                }
            }
        }

        room.SetDoorState(dir, connected);
    }

    private bool HasExit(Room room, Room.DoorDirection dir)
    {
        switch (dir)
        {
            case Room.DoorDirection.Top: return room.topExit;
            case Room.DoorDirection.Bottom: return room.bottomExit;
            case Room.DoorDirection.Left: return room.leftExit;
            case Room.DoorDirection.Right: return room.rightExit;
            default: return false;
        }
    }

    private Room.DoorDirection GetOpposite(Room.DoorDirection dir)
    {
        switch (dir)
        {
            case Room.DoorDirection.Top: return Room.DoorDirection.Bottom;
            case Room.DoorDirection.Bottom: return Room.DoorDirection.Top;
            case Room.DoorDirection.Left: return Room.DoorDirection.Right;
            case Room.DoorDirection.Right: return Room.DoorDirection.Left;
            default: return Room.DoorDirection.Top;
        }
    }
}

// Estensione semplice per mescolare la lista
public static class ListExtensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
