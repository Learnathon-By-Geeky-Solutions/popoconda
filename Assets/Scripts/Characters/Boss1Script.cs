using UnityEngine;
using Combat;

namespace Characters
{
    public class Boss1Script : MonoBehaviour
    {
        private Health _enemyHealth;
        public float maxHealth;
        public float currentHealth;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _enemyHealth = GetComponent<Health>();
            
        }
        // Update is called once per frame
        void Update()
        {
            if (_enemyHealth)
            {
                currentHealth = _enemyHealth.currentHealth;
                maxHealth = _enemyHealth.maxHealth;
            }
        }
    }
    
}
