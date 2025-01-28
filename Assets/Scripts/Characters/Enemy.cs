using UnityEngine;
using Combat;
using Cysharp.Threading.Tasks;

namespace Characters
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private GameObject gunRotatePoint;
        
        protected ShootingController ShootingController;
        protected Vector3 PlayerDirection;
        [SerializeField] protected Health enemyHealth;
        
        
        private bool _isAlive;
        
        public static event Health.StatEventWithFloat OnEnemyHealthChange;
        public static event Health.StatEvent OnBossDeath;
        
        
        protected virtual void Awake()
        {
            ShootingController = GetComponent<ShootingController>();
            _isAlive = true;
            enemyHealth.Initialize(false);
            UpdatePositionAsync().Forget();
        }
        
        protected virtual void OnEnable()
        {
            PlayerController.OnPlayerPositionChange += Move;
            enemyHealth.OnDeath += OnEnemyDeath;
            enemyHealth.OnHealthChange += UpdateHealthUI;
            Bullet.OnBulletHit += ApplyDamage;
        }
        
        protected virtual void OnDestroy()
        {
            PlayerController.OnPlayerPositionChange -= Move;
            enemyHealth.OnDeath -= OnEnemyDeath;
            enemyHealth.OnHealthChange -= UpdateHealthUI;
            Bullet.OnBulletHit -= ApplyDamage;
        }
        private void Move(Vector3 playerPosition)
        {
            PlayerDirection = playerPosition - transform.position;
            float distanceToPlayer = PlayerDirection.magnitude;
            if(distanceToPlayer >= 16) transform.position += new Vector3(PlayerDirection.x * (0.3f * Time.deltaTime), 0, 0);
        }
        
        private async UniTask UpdatePositionAsync()
        {
            while (_isAlive)
            {
                if (PlayerDirection != Vector3.zero)
                {
                    float rotationZ = Mathf.Atan2(PlayerDirection.y, PlayerDirection.x) * Mathf.Rad2Deg;
                    bool isFacingRight = PlayerDirection.x > 0;
                    transform.localScale = new Vector3(isFacingRight ? 1 : -1, 1, 1);
                    gunRotatePoint.transform.rotation = Quaternion.Euler(0, 0, isFacingRight ? rotationZ : rotationZ + 180f);
                }
                await UniTask.Delay(50);
            }
        }
        
        private void ApplyDamage(int damage, GameObject hitObject)
        {
            if (hitObject == gameObject)
            {
                enemyHealth.TakeDamage(damage);
            }
        }

        private static void UpdateHealthUI(float currentHealth)
        {
            OnEnemyHealthChange?.Invoke(currentHealth);
        }

        private void OnEnemyDeath()
        {
            _isAlive = false;
            Destroy(gameObject);
            OnBossDeath?.Invoke();
            ShootingController = null;
        }
    }
}


