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
        [SerializeField] private Texture2D crosshairTexture;
        [SerializeField] private InputManager inputManager;

        private Camera _playerCamera;
        [SerializeField] private Health playerHealth;
        [SerializeField] private Player player;

        private ShootingController _shootingController;
        private Vector3 _direction;
        
        private bool _isStunned;

        private CancellationTokenSource _cancellationTokenSource;
        
        public delegate void PlayerPosition(Vector3 position);
        public static event PlayerPosition OnPlayerPositionChange;

        public static event Health.StatEventWithFloat OnPlayerHealthChange;
        public static event Health.StatEventWithFloat OnJetpackFuelChange;

        private void Awake()
        { 
            _playerCamera = Camera.main;
            _playerRigidbody = GetComponent<Rigidbody>();
            Cursor.visible = false;
            _shootingController = GetComponent<ShootingController>();
            player.Initialize();
            playerHealth.Initialize(true);
            OnJetpackFuelChange?.Invoke(player.JetpackFuel / player.JetpackFuelMax);
        }
        private void Update()
        {
            if (gameObject && gameObject.activeInHierarchy)
            {
                OnPlayerPositionChange?.Invoke(transform.position);
            }
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
            inputManager.OnMousePositionChanged += HandleMousePosition;
            inputManager.OnMoveAxisChanged += HandleMoveAxis;
            inputManager.OnJumpPressed += HandleJump;
            inputManager.OnFirePressed += HandleFire;
            playerHealth.OnHealthChange += UpdateHealthUI;
            playerHealth.OnDeath += OnPlayerDeath;
            EnergyBlast.OnEnergyBlastHit += ApplyBlastDamage;
            Bullet.OnBulletHit += ApplyDamage;
            FireLaser.OnLaserHit += ApplyDamage;
            StunController.OnStun +=  Stunned;
        }

        private void OnDestroy()
        {
            inputManager.OnMousePositionChanged -= HandleMousePosition;
            inputManager.OnMoveAxisChanged -= HandleMoveAxis;
            inputManager.OnJumpPressed -= HandleJump;
            inputManager.OnFirePressed -= HandleFire;
            playerHealth.OnHealthChange -= UpdateHealthUI;
            playerHealth.OnDeath -= OnPlayerDeath;
            EnergyBlast.OnEnergyBlastHit += ApplyBlastDamage;
            Bullet.OnBulletHit -= ApplyDamage;
            FireLaser.OnLaserHit -= ApplyDamage;
            StunController.OnStun -= Stunned;
            
            // Cancel the refill task
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }

        private void HandleMousePosition(Vector2 screenPosition)
        {
            if (_playerCamera == null || _isStunned) return;

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
        
        // Show the crosshair on the screen using OnGUI
        private void OnGUI()
        {
            if (crosshairTexture != null)
            {
                Vector2 mousePosition = Event.current.mousePosition;
                float crosshairSize = 32f; // Adjust size as needed
                GUI.DrawTexture(new Rect(mousePosition.x - crosshairSize / 2, mousePosition.y - crosshairSize / 2, crosshairSize, crosshairSize), crosshairTexture);
                
            }
        }

        private void HandleMoveAxis(float value)
        {
            if (_isStunned) return;
            _playerRigidbody.AddRelativeForce(Vector3.right * (value * player.MoveSpeed * Time.deltaTime));
        }

        private void HandleJump()
        {
            if (_isStunned || player.JetpackFuel <= 0) return;

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
            if (_isStunned) return;
            _shootingController.FireBullet(_direction);
        }

        private void ApplyDamage(int damage, GameObject hitObject)
        {
            if (hitObject == gameObject)
            {
                playerHealth.TakeDamage(damage);
            }
        }
        private void ApplyBlastDamage(int damage)
        {
            playerHealth.TakeDamage(damage);
        }
        
        private void Stunned(bool isStunned)
        {
            _isStunned = isStunned;
        }
    }
}
