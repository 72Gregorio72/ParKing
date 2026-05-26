using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public int currentHealth;

    [SerializeField] private Slider healthSlider;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= 1;
        Debug.Log("Player Health: " + currentHealth);

        UpdateUI();

        if (currentHealth <= 0)
        {
            Die();
        }

        Debug.Log("Player took " + damageAmount + " damage.");
    }

    private void UpdateUI()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    void Die()
    {
        Debug.Log("Player has died!");
        // Add death logic here (e.g., respawn, game over screen, etc.)
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("OnTriggerEnter2D called with: " + collider.gameObject.name + " (tag: " + collider.gameObject.tag + ")");
        if (collider.gameObject.CompareTag("Boss") || collider.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(1);
        }
    }
}
