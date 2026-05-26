using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Room parentRoom;
    public int health = 3;

    public void Initialize(Room room)
    {
        parentRoom = room;
    }

    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        if (parentRoom != null)
        {
            parentRoom.EnemyDefeated(gameObject);
        }
        Destroy(gameObject);
    }

    // Per test: se clicchiamo sul nemico in Play mode lo "uccidiamo"
    private void OnMouseDown()
    {
        Die();
    }
}
