using UnityEngine;
using Combat;
using Game;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace Characters
{
    public class PlayerController : MonoBehaviour
    {
        private Rigidbody _playerRigidbody;
        [SerializeField] private InputManager inputManager;

        private Camera _playerCamera;
        [SerializeField] private Health playerHealth;
        [SerializeField] private Player player;

        private ShootingController _shootingController;
        private Vector3 _direction;

        private CancellationTokenSource _cancellationTokenSource;
        
        public delegate void PlayerPosition(Vector3 position);
        public static event PlayerPosition OnPlayerPosition;

        public static event Health.StatEventWithFloat OnPlayerHealthChange;
        public static event Health.StatEventWithFloat OnJetpackFuelChange;

        private void Awake()
        { 
            _playerCamera = Camera.main;
            _playerRigidbody = GetComponent<Rigidbody>();
            _shootingController = GetComponent<ShootingController>();
            player.Initialize();
            playerHealth.Initialize(true);
            playerHealth.OnHealthChange += UpdateHealthUI;
            playerHealth.OnDeath += OnPlayerDeath;
        }

        private void Update()
        {
            OnPlayerPosition?.Invoke(transform.position);
        }

        private static void UpdateHealthUI(float currentHealth)
        {
            OnPlayerHealthChange?.Invoke(currentHealth);
        }

        private void OnPlayerDeath()
        {
            player.Die();
        }

        private void OnEnable()
        {
            inputManager.OnPositionChanged += HandleMousePosition;
            inputManager.OnMoveAxisChanged += HandleMoveAxis;
            inputManager.OnJumpPressed += HandleJump;
            inputManager.OnFirePressed += HandleFire;
            Bullet.OnBulletHit += ApplyDamage;
            FireLaser.OnLaserHit += ApplyDamage;
        }

        private void OnDestroy()
        {
            inputManager.OnPositionChanged -= HandleMousePosition;
            inputManager.OnMoveAxisChanged -= HandleMoveAxis;
            inputManager.OnJumpPressed -= HandleJump;
            inputManager.OnFirePressed -= HandleFire;
            Bullet.OnBulletHit -= ApplyDamage;
            FireLaser.OnLaserHit -= ApplyDamage;
        }

        private void HandleMousePosition(Vector2 screenPosition)
        {
            if (_playerCamera == null || !player.IsAlive) return;

            Ray ray = _playerCamera.ScreenPointToRay(screenPosition);
            Plane gunPlane = new Plane(Vector3.forward, player.GunRotatePoint.transform.position);

            if (gunPlane.Raycast(ray, out float distance))
            {
                Vector3 targetPosition = ray.GetPoint(distance);
                _direction = targetPosition - player.GunRotatePoint.transform.position;
                float rotationZ = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;

                player.GunRotatePoint.transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);
                player.PlayerGfx.transform.localScale = _direction.x < 0 ? new Vector3(-1, 1, 1) : Vector3.one;
            }
        }

        private void HandleMoveAxis(float value)
        {
            if (!player.IsAlive) return;
            _playerRigidbody.AddRelativeForce(Vector3.right * (value * player.MoveSpeed * Time.fixedDeltaTime));
        }

        private void HandleJump()
        {
            if (!player.IsAlive || player.JetpackFuel <= 0) return;

            _playerRigidbody.AddForce(Vector3.up * (player.FlySpeed * Time.deltaTime), ForceMode.VelocityChange);
            player.JetpackFuel -= Time.deltaTime * player.FuelConsumeRate;
            OnJetpackFuelChange?.Invoke(player.JetpackFuel / player.JetpackFuelMax);

            ResetRefillTimer();
        }

        private void ResetRefillTimer()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose(); // Properly dispose of the old token
            _cancellationTokenSource = new CancellationTokenSource();

            RefillJetpack(_cancellationTokenSource.Token).Forget();
        }

        private async UniTask RefillJetpack(CancellationToken token)
        {
            await UniTask.Delay(1000, cancellationToken: token);
            while (player.JetpackFuel < player.JetpackFuelMax && !token.IsCancellationRequested)
            {
                player.JetpackFuel += Time.deltaTime * player.FuelFillRate;
                OnJetpackFuelChange?.Invoke(player.JetpackFuel / player.JetpackFuelMax);
                await UniTask.Yield(token);
            }
            
        }

        private void HandleFire()
        {
            if (!player.IsAlive) return;
            _shootingController.FireBullet(_direction);
        }

        private void ApplyDamage(int damage, GameObject hitObject)
        {
            if (hitObject == gameObject)
            {
                playerHealth.TakeDamage(damage);
            }
        }
    }
}
