using UnityEngine;
using Combat;
using UI;
using Cysharp.Threading.Tasks;
using Random = UnityEngine.Random;

namespace Characters
{
    public class Boss1Script : MonoBehaviour
    {
        [SerializeField] private GameObject gunRotatePoint;
        [SerializeField] private HudHandler hudHandler;

        private Enemy _enemy;
        [SerializeField] private Health enemyHealth;
        private ShootingController _shootingController;
        private FireLaser _fireLaser;
        
        private Vector2 _playerDirection;
        private float _distanceToPlayer;

        private bool _isAlive;
        
        public static event Health.StatEventWithFloat OnEnemyHealthChange;


        private void Awake()
        {
            _shootingController = GetComponent<ShootingController>();
            _fireLaser = GetComponent<FireLaser>();
            _enemy = GetComponent<Enemy>();
            _isAlive = true;

            enemyHealth.Initialize(false);
            enemyHealth.OnDeath += OnBossDeath;
            enemyHealth.OnHealthChange += UpdateHealthUI;
            Bullet.OnBulletHit += ApplyDamage;
            PlayerController.OnPlayerPosition += GetPosition;

            ScheduleFireActionsAsync().Forget();
            UpdatePositionAsync().Forget();
        }

        private void OnDestroy()
        {
            PlayerController.OnPlayerPosition -= GetPosition;
        }

        private void Update()
        {
            if (_isAlive)
            {
                _enemy.MoveTowardsPlayer(_playerDirection, _distanceToPlayer);
            }
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

        private async UniTask ScheduleFireActionsAsync()
        {
            while (_isAlive)
            {
                await UniTask.Delay(1000);
                if (_isAlive)
                {
                    await FireActionsAsync();
                }
            }
        }

        private async UniTask FireActionsAsync()
        {
            bool isFiringBullet = Random.Range(0, 3) != 0;
            float fireDuration = isFiringBullet ? Random.Range(15f, 20f) : Random.Range(5f, 7f);
            float startTime = Time.time;

            while (Time.time - startTime < fireDuration && _isAlive)
            {
                if (isFiringBullet)
                {
                    _shootingController.FireBullet(_playerDirection);
                }
                else
                {
                    _fireLaser.FireLaserProjectile(_playerDirection);
                }
                await UniTask.Delay(100);
            }
            await ScheduleFireActionsAsync();
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

        private void OnBossDeath()
        {
            _isAlive = false;
        }

        private void GetPosition(Vector3 playerPosition)
        {
            _playerDirection = playerPosition - transform.position;
            _distanceToPlayer = _playerDirection.magnitude;
        }
    }
}