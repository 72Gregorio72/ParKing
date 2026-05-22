using UnityEngine;

public class DoDamage : MonoBehaviour
{
    public int damageAmount = 10;
    private GameObject shooter; // il player che ha sparato

    public void SetShooter(GameObject shooterObj)
    {
        shooter = shooterObj;
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), shooter.GetComponent<Collider2D>(), true);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>().TakeDamage(damageAmount);
            Debug.Log("Player took damage!");
            Destroy(gameObject);
        }
        else if (!other.isTrigger)
            Destroy(gameObject);
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
