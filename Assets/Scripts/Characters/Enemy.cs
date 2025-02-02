using System.Threading;
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

        private CancellationTokenSource _cancellationTokenSource;

        protected virtual void Awake()
        {
            ShootingController = GetComponent<ShootingController>();
            _isAlive = true;
            enemyHealth.Initialize(false);
            _cancellationTokenSource = new CancellationTokenSource(); // Initialize CancellationTokenSource
            UpdatePositionAsync(_cancellationTokenSource.Token).Forget(); // Perform async operation
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
            // Unsubscribe from events to avoid null reference when destroyed
            PlayerController.OnPlayerPositionChange -= Move;
            enemyHealth.OnDeath -= OnEnemyDeath;
            enemyHealth.OnHealthChange -= UpdateHealthUI;
            Bullet.OnBulletHit -= ApplyDamage;

            // Cancel the async tasks and dispose of the token source
            _cancellationTokenSource?.Cancel(); // Stop async operations
            _cancellationTokenSource?.Dispose(); // Dispose of the token source
        }

        private void Move(Vector3 playerPosition)
        {
            PlayerDirection = playerPosition - transform.position;
            float distanceToPlayer = PlayerDirection.magnitude;
            if (distanceToPlayer >= 16) 
                transform.position += new Vector3(PlayerDirection.x * (0.3f * Time.deltaTime), 0, 0);
        }

        private async UniTask UpdatePositionAsync(CancellationToken token)
        {
            // Ensure task stops when cancelled or object is destroyed
            while (_isAlive && !token.IsCancellationRequested)
            {
                // Check if the object has been destroyed
                if (gameObject == null || !gameObject.activeInHierarchy)
                {
                    return; // Exit if the object is destroyed
                }

                if (PlayerDirection != Vector3.zero)
                {
                    float rotationZ = Mathf.Atan2(PlayerDirection.y, PlayerDirection.x) * Mathf.Rad2Deg;
                    bool isFacingRight = PlayerDirection.x > 0;
                    transform.localScale = new Vector3(isFacingRight ? 1 : -1, 1, 1);
                    gunRotatePoint.transform.rotation = Quaternion.Euler(0, 0, isFacingRight ? rotationZ : rotationZ + 180f);
                }

                // Await next delay with cancellation support
                await UniTask.Delay(50, cancellationToken: token);
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
            _isAlive = false; // Stop movement and other actions
            _cancellationTokenSource?.Cancel(); // Stop async operations when dead
            OnBossDeath?.Invoke(); // Trigger death event
            ShootingController = null; // Clear the shooting controller
        }
    }
}
