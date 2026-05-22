using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
	public int maxHealth = 100;

	public int currentHealth;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        currentHealth = maxHealth;
    }

	// Update is called once per frame
	void Update()
	{

	}

	public void TakeDamage(int damageAmount)
	{
		currentHealth -= damageAmount;
		Debug.Log("Player Health: " + currentHealth);

		if (currentHealth <= 0)
		{
			Die();
		}

		Debug.Log("Player took " + damageAmount + " damage.");
	}
	
	void Die()
	{
		Debug.Log("Player has died!");
		// Add death logic here (e.g., respawn, game over screen, etc.)
	}
}
