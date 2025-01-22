using UnityEngine;
using Combat;
using Cysharp.Threading.Tasks;

namespace Characters
{
    public class Boss2Script : MonoBehaviour
    {
        private Enemy _enemy;
        [SerializeField] private GameObject gunRotatePoint;
        
        [SerializeField] private Health enemyHealth;
        private ShootingController _shootingController;
        
        private Vector2 _playerDirection;
        private float _distanceToPlayer;
        
        private bool _isAlive;
        
        public static event Health.StatEventWithFloat OnEnemyHealthChange;
        public static event Health.StatEvent OnBoss2Death;
        
        private void Awake()
        {
            _shootingController = GetComponent<ShootingController>();
            _enemy = GetComponent<Enemy>();
            _isAlive = true;
            
            enemyHealth.Initialize(false);
            enemyHealth.OnDeath += OnBossDeath;
            enemyHealth.OnHealthChange += UpdateHealthUI;
            Bullet.OnBulletHit += ApplyDamage;
            PlayerController.OnPlayerPosition += GetPosition;
            
            UpdatePositionAsync().Forget();
        }
        
        private void OnDestroy()
        {
            PlayerController.OnPlayerPosition -= GetPosition;
            enemyHealth.OnDeath -= OnBossDeath;
            enemyHealth.OnHealthChange -= UpdateHealthUI;
            Bullet.OnBulletHit -= ApplyDamage;
        }
        
        private void Update()
        {
            if (_isAlive)
            {
                _enemy.MoveTowardsPlayer(_playerDirection, _distanceToPlayer);
            }
            _shootingController.FireBullet(_playerDirection);
        }
        
        private void GetPosition(Vector3 playerPosition)
        {
            _playerDirection = (playerPosition - transform.position).normalized;
            _distanceToPlayer = Vector3.Distance(playerPosition, transform.position);
        }
        private async UniTask UpdatePositionAsync()
        {
            while (_isAlive)
            {
                if (_playerDirection != Vector2.zero)
                {
                    float rotationZ = Mathf.Atan2(_playerDirection.y, _playerDirection.x) * Mathf.Rad2Deg;
                    bool isFacingRight = _playerDirection.x > 0;
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
            Debug.Log("Boss2 health: " + currentHealth);
            OnEnemyHealthChange?.Invoke(currentHealth);
        }
        
        private void OnBossDeath()
        {
            _isAlive = false;
            OnBoss2Death?.Invoke();
            Destroy(gameObject);
        }
        
        
    }
}
