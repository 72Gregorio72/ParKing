using UnityEngine;
using UnityEngine.UI;

public class BossHealth : Enemy
{
    [Header("Boss Health Settings")]
    [SerializeField] private Slider healthSlider;

    private void Start()
    {
        health = 10;
        if (healthSlider != null)
        {
            healthSlider.maxValue = health;
            healthSlider.value = health;
        }
    }

    public override void TakeDamage(int damage)
    {
        // Il testo dice "ogni volta che viene colpito da un attacco del player ne perde una".
        health -= 1; 
        
        UpdateUI();
        Debug.Log("Boss took 1 damage! Remaining lives: " + health);

        if (health <= 0)
        {
            health = 0;
            Die();
        }
    }

    private void UpdateUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = health;
        }
    }
}
