using System.Threading;
using UnityEngine;
using Combat;
using Cutscene;
using Game;
using Cysharp.Threading.Tasks;

namespace Characters
{
    public class Hero : MonoBehaviour
    {
        [SerializeField] private GameObject gunRotatePoint;
        protected Vector3 PlayerDirection;
        [SerializeField] protected Health heroHealth;
        private Vector3 _initialScale;
        
        protected Rigidbody HeroRigidbody;
        protected ShootingController ShootingController;
        protected Dash Dash;
        protected Shield Shield;
        
        [Header("Hero Data")]
        [SerializeField] protected float moveSpeed;
        protected bool CanMove = true;
        private bool _isAlive;
        private bool _onVerticalPlatform;
        
        public delegate void StatEvent();
        public static event StatEvent OnHeroMove;
        public static event StatEvent OnHeroStop;
        public static event Health.StatEventWithFloat OnHeroHealthChange;
        public static event StatEvent OnHeroDeath;

        private CancellationTokenSource _cancellationTokenSource;

        protected virtual void Awake()
        {
            HeroRigidbody = GetComponent<Rigidbody>();
            Dash = GetComponent<Dash>();
            Shield = GetComponent<Shield>();
            _initialScale = transform.localScale;
        }

        protected virtual void OnEnable()
        {
            ShootingController = GetComponent<ShootingController>();
            _isAlive = true;
            heroHealth.Initialize();
            
            heroHealth.OnDeath += ApplyHeroDeath;
            heroHealth.OnHealthChange += UpdateHealthUI;
            Bullet.OnBulletHit += ApplyDamage;
            CutsceneManager.OnVerticalPlatformEvent += DisableGravity;
            _cancellationTokenSource = new CancellationTokenSource(); // Initialize CancellationTokenSource
            UpdatePositionAsync(_cancellationTokenSource.Token).Forget(); // Perform async operation
        }
        
        protected virtual void Update()
        {
            Vector3 playerPosition = GameManager.GetPlayerPosition();
            PlayerDirection = playerPosition - gunRotatePoint.transform.position;
            Move(playerPosition);
        }

        protected virtual void OnDisable()
        {
            heroHealth.OnDeath -= ApplyHeroDeath;
            heroHealth.OnHealthChange -= UpdateHealthUI;
            CutsceneManager.OnVerticalPlatformEvent -= DisableGravity;
            Bullet.OnBulletHit -= ApplyDamage;

            // Cancel the async tasks and dispose of the token source
            _cancellationTokenSource?.Cancel(); // Stop async operations
            _cancellationTokenSource?.Dispose(); // Dispose of the token source
        }

        private void Move(Vector3 playerPosition)
        {
            if(!CanMove) return;
            
            float distanceToPlayer = playerPosition.x - transform.position.x;
            
            if (Mathf.Abs(distanceToPlayer) >= 16)
            {
                transform.position += new Vector3(PlayerDirection.x * (moveSpeed * Time.deltaTime), 0, 0);
                if (!_onVerticalPlatform)
                {
                    OnHeroMove?.Invoke();
                }
                
            }
            else
            {
                OnHeroStop?.Invoke();
            }
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

                    // Flip while keeping original scale
                    float direction = isFacingRight ? 1f : -1f;
                    transform.localScale = new Vector3(_initialScale.x * direction, _initialScale.y, _initialScale.z);

                    // Rotate gun
                    float finalRotation = isFacingRight ? rotationZ : rotationZ + 180f;
                    gunRotatePoint.transform.rotation = Quaternion.Euler(0, 0, finalRotation);
                }

                // Await next delay with cancellation support
                await UniTask.Delay(50, cancellationToken: token);
            }
        }

        private void ApplyDamage(int damage, GameObject hitObject)
        {
            if (hitObject == gameObject)
            {
                heroHealth.TakeDamage(damage);
            }
        }

        private static void UpdateHealthUI(float currentHealth)
        {
            OnHeroHealthChange?.Invoke(currentHealth);
        }

        protected void ApplyHeroDeath()
        {
            _isAlive = false; // Stop movement and other actions
            _cancellationTokenSource?.Cancel(); // Stop async operations when dead
            ShootingController = null;
            heroHealth.HealthBuff(5);
            OnHeroDeath?.Invoke(); // Trigger death event
        }
        
        private void DisableGravity()
        {
            if (HeroRigidbody == null) return;
            HeroRigidbody.useGravity = false;
            HeroRigidbody.linearVelocity = Vector3.zero;
            _onVerticalPlatform = true;
        }
    }
}
