using UnityEngine;
using UI;

namespace Combat
{
    public class Health : MonoBehaviour
    {
        public int maxHealth;
        public int currentHealth;
        [SerializeField] private HudHandler hudHandler;
        
        private void Awake()
        {
            currentHealth = maxHealth;
        }
        
        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            hudHandler.UpdateHealth();
            if (currentHealth <= 0)
            {
                Die();
            }
        }
        
        private void Die()
        {
            Destroy(gameObject);
        }
    }

}
