using UnityEngine;

namespace Combat
{
    public class Health : MonoBehaviour
    {
        public int maxHealth;
        public int currentHealth;


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            currentHealth = maxHealth;

        }

        // Take damage from an external source
        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            if (currentHealth < 0)
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

}
