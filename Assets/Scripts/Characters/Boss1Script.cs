using UnityEngine;
using Combat;

namespace Characters
{
    public class Boss1Script : MonoBehaviour
    {
        [SerializeField] private GameObject gunRotatePoint;
        
        private Enemy _enemy;
        private ShootingController _shootingController;
        private Health _enemyHealth;
        
        public float MaxHealth=>_enemyHealth.maxHealth;
        public float CurrentHealth=>_enemyHealth.currentHealth;
        
        void Awake()
        {
            _shootingController = GetComponent<ShootingController>();
            _enemy = GetComponent<Enemy>();
            _enemyHealth = GetComponent<Health>();
        }

        private void Update()
        {
            
            _enemy.GetPlayerPosition();
            
            gunRotatePoint.transform.rotation  = Quaternion.Euler(0f, 0f, _enemy.rotationZ);
        }

        private void FixedUpdate()
        {
            _shootingController.FireBullet(_enemy.directionToPlayer, _enemy.rotationZ);

        }
    }
    
}
