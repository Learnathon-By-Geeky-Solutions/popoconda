using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth;
    private float currentHealth;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;

    }

    // Take damage from an external source
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Die and destroy the GameObject
    void Die()
    {
        Destroy(gameObject);
    }
}
