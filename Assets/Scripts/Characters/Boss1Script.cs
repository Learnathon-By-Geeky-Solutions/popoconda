using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using Combat;

namespace Characters
{
    public class Boss1Script : MonoBehaviour
    {
        [SerializeField] private GameObject enemyBody;
        [SerializeField] private GameObject gunRotatePoint;
        
        private Enemy _enemy;
        private ShootingController _shootingController;
        private Health _enemyHealth;
        private FireLaser _fireLaser;
        
        private bool _isActionScheduled;
        
        public float MaxHealth => _enemyHealth.maxHealth;
        public float CurrentHealth => _enemyHealth.currentHealth;
        
        private void Awake()
        {
            _shootingController = GetComponent<ShootingController>();
            _fireLaser = GetComponent<FireLaser>();
            _enemy = GetComponent<Enemy>();
            _enemyHealth = GetComponent<Health>();
            _isActionScheduled = false;
        }

        private void Update()
        {
            _enemy.GetPlayerPosition();
            
            gunRotatePoint.transform.rotation = Quaternion.Euler(0f, 0f, _enemy.rotationZ);
            
            enemyBody.transform.localScale = _enemy.directionToPlayer.x < 0 ? new Vector3(-1, 1, 1) : new Vector3(1, 1, 1);
            
            _enemy.MoveTowardsPlayer();
        }

        private void FixedUpdate()
        {
            if (!_isActionScheduled)
            {
                _isActionScheduled = true;
                StartCoroutine(FireActionCoroutine());
            }
        }
        
        private IEnumerator FireActionCoroutine()
        {
            Task fireActionsTask = FireActionsAsync();
            yield return new WaitUntil((() => fireActionsTask.IsCompleted));
            _isActionScheduled = false;
        }

        private async Task FireActionsAsync()
        {
            // Fire bullets for 15 to 20 seconds
            bool isFiringBullet = Random.Range(0, 3) != 0; // 66% chance to fire bullets, 33% chance to fire lasers
            float fireDuration = isFiringBullet ? Random.Range(15f, 20f) : Random.Range(5f, 7f);

            float startTime = Time.time;
    
            while (Time.time - startTime < fireDuration)
            {
                if (isFiringBullet)
                {
                    _shootingController.FireBullet(_enemy.directionToPlayer);
                }
                else
                {
                    _fireLaser.FireLaserProjectile(_enemy.directionToPlayer);
                }
        
                // Wait 0.1 second before firing again
                await Task.Delay(100);
            }
        }
    }
}
