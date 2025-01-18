using UnityEngine;
using Combat;
using Game;

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
        private float _rotationZ;
        private bool _isFiring;
        
        // Create a delegate for player position
        public delegate void PlayerPosition(Vector3 position);
        public static event PlayerPosition OnPlayerPosition;
        
        public static event Health.StatEventWithFloat OnPlayerHealthChange;
        public static event Health.StatEventWithFloat OnJetpackFuelChange;
        
        private void Awake()
        {
            _playerCamera = Camera.main;
            _playerRigidbody = gameObject.GetComponent<Rigidbody>();
            _shootingController = gameObject.GetComponent<ShootingController>();
            player.Initialize();
            playerHealth.Initialize();
            playerHealth.OnHealthChange += UpdateHealthUI;
            playerHealth.OnDeath += OnPlayerDeath;
            Bullet.OnBulletHit += ApplyDamage;
            FireLaser.OnLaserHit += ApplyDamage;
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
        }

        private void OnDisable()
        {
            inputManager.OnPositionChanged -= HandleMousePosition;
            inputManager.OnMoveAxisChanged -= HandleMoveAxis;
            inputManager.OnJumpPressed -= HandleJump;
            inputManager.OnFirePressed -= HandleFire;
        }

        private void HandleMousePosition(Vector2 screenPosition)
        {
            if (_playerCamera == null && !player.IsAlive) return;
            
            Ray ray = _playerCamera.ScreenPointToRay(screenPosition);
            
            Plane gunPlane = new Plane(Vector3.forward, player.GunRotatePoint.transform.position);

            // If the ray intersects the plane
            if (gunPlane.Raycast(ray, out float distance))
            {
                Vector3 targetPosition = ray.GetPoint(distance);                    // Get the intersection point in world space
                
                _direction = targetPosition - player.GunRotatePoint.transform.position;

                _rotationZ = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;  // Calculate the rotation in degrees

                player.GunRotatePoint.transform.rotation = Quaternion.Euler(0f, 0f, _rotationZ);    // Rotate the gun towards the target
                
                player.PlayerGfx.transform.localScale = _direction.x < 0 ? new Vector3(-1, 1, 1) : new Vector3(1, 1, 1); // Flip the player sprite if needed
                
            }
        }
        
        private void HandleMoveAxis(float value)
        {
            if(!player.IsAlive) return;
            _playerRigidbody.AddRelativeForce(Vector3.right * (value * player.MoveSpeed * Time.fixedDeltaTime));
        }

        private void HandleJump()
        {
            if (!player.IsAlive) return;
            if (player.JetpackFuel > 0)
            {
                _playerRigidbody.AddForce(Vector3.up * (player.FlySpeed * Time.deltaTime), ForceMode.VelocityChange);
                player.JetpackFuel -= Time.deltaTime * player.FuelConsumeRate;
                OnJetpackFuelChange?.Invoke(player.JetpackFuel/player.JetpackFuelMax);
            }
            OnJetpackFuelChange?.Invoke(player.JetpackFuel/player.JetpackFuelMax);
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
