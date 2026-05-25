using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Room : MonoBehaviour
{
    [Header("Configurazione Uscite")]
    public bool topExit;
    public bool bottomExit;
    public bool leftExit;
    public bool rightExit;

    [Header("Dimensioni Stanza")]
    public Vector2 roomSize = new Vector2(20, 10);

    [Header("Riferimenti Porte/Muri")]
    public GameObject topDoorObj;
    public GameObject bottomDoorObj;
    public GameObject leftDoorObj;
    public GameObject rightDoorObj;
    public GameObject topWallObj;
    public GameObject bottomWallObj;
    public GameObject leftWallObj;
    public GameObject rightWallObj;

    [Header("Sistema di Blocco (Lock)")]
    [Tooltip("Prefab del cancello che chiude fisicamente l'uscita quando la stanza è lockata.")]
    public GameObject gatePrefab;
    private List<GameObject> activeGates = new List<GameObject>();
    private bool isRoomCleared = false;
    private bool isPlayerInside = false;

    [Header("Popolamento")]
    public List<GameObject> enemyPrefabs;
    public GameObject platformPrefab;
    private List<GameObject> spawnedEnemies = new List<GameObject>();

    public UnityEvent OnRoomCleared;

    private void Start()
    {
        // Aggiungi un trigger centrale per rilevare il player
        BoxCollider2D trigger = gameObject.AddComponent<BoxCollider2D>();
        trigger.size = roomSize * 0.8f; // Leggermente più piccolo della stanza
        trigger.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isPlayerInside && !isRoomCleared)
        {
            isPlayerInside = true;
            if (spawnedEnemies.Count > 0)
            {
                LockRoom();
            }
        }
    }

    public void PopulateRoom(int platformCount, int enemyCount)
    {
        SpawnPlatforms(platformCount);
        SpawnEnemies(enemyCount);
    }

    private void SpawnPlatforms(int count)
    {
        if (platformPrefab == null) return;
        for (int i = 0; i < count; i++)
        {
            Vector2 randomPos = (Vector2)transform.position + new Vector2(
                Random.Range(-roomSize.x / 3f, roomSize.x / 3f),
                Random.Range(-roomSize.y / 3f, roomSize.y / 3f)
            );
            Instantiate(platformPrefab, randomPos, Quaternion.identity, transform);
        }
    }

    private void SpawnEnemies(int count)
    {
        if (enemyPrefabs == null || enemyPrefabs.Count == 0) return;
        for (int i = 0; i < count; i++)
        {
            Vector2 randomPos = (Vector2)transform.position + new Vector2(
                Random.Range(-roomSize.x / 3f, roomSize.x / 3f),
                Random.Range(-roomSize.y / 3f, roomSize.y / 3f)
            );
            GameObject enemyObj = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Count)], randomPos, Quaternion.identity, transform);
            spawnedEnemies.Add(enemyObj);
            
            // Inizializza l'enemy con riferimento a questa stanza
            Enemy enemy = enemyObj.GetComponent<Enemy>();
            if (enemy != null) enemy.Initialize(this);
        }
    }

    public void EnemyDefeated(GameObject enemy)
    {
        spawnedEnemies.Remove(enemy);
        if (spawnedEnemies.Count == 0)
        {
            UnlockRoom();
        }
    }

    private void LockRoom()
    {
        Debug.Log("Stanza Bloccata! Sconfiggi i nemici.");
        // Crea cancelli su tutte le uscite ATTIVE
        if (topExit) SpawnGate(new Vector3(0, roomSize.y / 2f, 0));
        if (bottomExit) SpawnGate(new Vector3(0, -roomSize.y / 2f, 0));
        if (leftExit) SpawnGate(new Vector3(-roomSize.x / 2f, 0, 0));
        if (rightExit) SpawnGate(new Vector3(roomSize.x / 2f, 0, 0));
    }

    private void SpawnGate(Vector3 localPos)
    {
        if (gatePrefab == null) return;
        GameObject gate = Instantiate(gatePrefab, transform.position + localPos, Quaternion.identity, transform);
        activeGates.Add(gate);
    }

    private void UnlockRoom()
    {
        Debug.Log("Stanza Liberata!");
        isRoomCleared = true;
        foreach (var gate in activeGates)
        {
            if (gate != null) Destroy(gate);
        }
        activeGates.Clear();
        OnRoomCleared?.Invoke();
    }

    public void SetDoorState(DoorDirection direction, bool isOpen)
    {
        switch (direction)
        {
            case DoorDirection.Top:
                if (topDoorObj) topDoorObj.SetActive(isOpen);
                if (topWallObj) topWallObj.SetActive(!isOpen);
                break;
            case DoorDirection.Bottom:
                if (bottomDoorObj) bottomDoorObj.SetActive(isOpen);
                if (bottomWallObj) bottomWallObj.SetActive(!isOpen);
                break;
            case DoorDirection.Left:
                if (leftDoorObj) leftDoorObj.SetActive(isOpen);
                if (leftWallObj) leftWallObj.SetActive(!isOpen);
                break;
            case DoorDirection.Right:
                if (rightDoorObj) rightDoorObj.SetActive(isOpen);
                if (rightWallObj) rightWallObj.SetActive(!isOpen);
                break;
        }
    }

    public enum DoorDirection { Top, Bottom, Left, Right }
}
