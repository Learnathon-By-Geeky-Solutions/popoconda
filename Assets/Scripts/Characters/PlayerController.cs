using UnityEngine;
using Combat;
using Game;
using Input;
using Cysharp.Threading.Tasks;
using System.Threading;
using Cutscene;
using UnityEngine.InputSystem;
using Weapon;

namespace Characters
{
    public class PlayerController : MonoBehaviour
    {
        private Rigidbody _playerRigidbody;

        private UnityEngine.Camera _playerCamera;
        [SerializeField] private Health playerHealth;
        [SerializeField] private Player player;
        [SerializeField] private ParticleSystem jetpackParticle1;
        [SerializeField] private ParticleSystem jetpackParticle2;
        private GunData newGunData;
        
        private bool _below75Triggered;
        private bool _below50Triggered;
        private bool _below25Triggered;

        private ShootingController _shootingController;
        private Vector3 _direction;
        
        private bool _onVerticalPlatform;

        private CancellationTokenSource _cancellationTokenSource;
        
        public delegate void StatEventWithFloat(float value);
        public delegate void StatEventWithInt(int value);
        public delegate void StatEvent();
        public static event StatEvent OnPlayerHit;
        public static event StatEvent OnBulletShoot;
        
        public static event StatEventWithInt OnBossStateChange;
        public static event StatEventWithFloat OnPlayerMove;
        public static event Health.StatEventWithFloat OnPlayerHealthChange;
        public static event Health.StatEventWithFloat OnJetpackFuelChange;

        private void Awake()
        { 
            _playerCamera = UnityEngine.Camera.main;
            _playerRigidbody = GetComponent<Rigidbody>();
            Cursor.visible = false;
            _shootingController = GetComponent<ShootingController>();
            player.Initialize();
            playerHealth.Initialize();
            OnJetpackFuelChange?.Invoke(player.JetpackFuel / player.JetpackFuelMax);
            _below75Triggered = false;
            _below50Triggered = false;
            _below25Triggered = false;
            _onVerticalPlatform = false;
        }

        private void OnEnable()
        {
            GameManager.SetPlayerTransform(transform);
            InputManager.OnMousePositionChanged += HandleMousePosition;
            InputManager.OnMoveAxisChanged += HandleMoveAxis;
            InputManager.OnJumpPressed += HandleJump;
            InputManager.OnFirePressed += HandleFire;
            playerHealth.OnHealthChange += UpdateHealthUI;
            playerHealth.OnHealthChange += ChangeBossState;
            playerHealth.OnDeath += OnPlayerDeath;
            CutsceneManager.OnVerticalPlatformEvent += DisableGravity;
            Hero.OnHeroDeath += playerHealth.ResetHealth;
            Hero.OnHeroDeath += () => _below25Triggered = false;
            Hero.OnHeroDeath += ResetJetpackFuel;
            Bullet.OnBulletHit += ApplyDamage;
        }

        private void OnDestroy()
        {
            InputManager.OnMousePositionChanged -= HandleMousePosition;
            InputManager.OnMoveAxisChanged -= HandleMoveAxis;
            InputManager.OnJumpPressed -= HandleJump;
            InputManager.OnFirePressed -= HandleFire;
            playerHealth.OnHealthChange -= UpdateHealthUI;
            playerHealth.OnHealthChange -= ChangeBossState;
            playerHealth.OnDeath -= OnPlayerDeath;
            CutsceneManager.OnVerticalPlatformEvent -= DisableGravity;
            Hero.OnHeroDeath -= playerHealth.ResetHealth;
            Hero.OnHeroDeath -= ResetJetpackFuel;
            Bullet.OnBulletHit -= ApplyDamage;
            
            GameManager.ClearPlayerTransform();
            
            // Cancel the refill task
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }
        
        
        private void HandleMousePosition(Vector2 screenPosition)
        {
            if (_playerCamera == null) return;

            Ray ray = _playerCamera.ScreenPointToRay(screenPosition);
            Plane gunPlane = new Plane(Vector3.forward, player.GunRotatePoint.transform.position);

            if (gunPlane.Raycast(ray, out float distance))
            {
                UpdateGunRotation(ray.GetPoint(distance));
            }
        }
        
        // <<summary>>
        // Update the gun rotation based on target position
        // <<summary>>
        private void UpdateGunRotation(Vector3 targetPosition)
        {
            _direction = targetPosition - player.GunRotatePoint.transform.position;
            float rotationZ = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;

            if (player.PlayerGfx.transform.localScale.x < 0)
                rotationZ += 180f;
            
            player.PlayerGfx.transform.localScale = _direction.x < 0 ? new Vector3(-1, 1, 1) : Vector3.one;
            player.GunRotatePoint.transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);
        }
        
        
        private static void UpdateHealthUI(float currentHealth)
        {
            OnPlayerHealthChange?.Invoke(currentHealth);
        }

        private void ChangeBossState(float currentHealth)
        {
            CheckAndInvokeBossState(currentHealth, 0.75f, ref _below75Triggered, 1);
            CheckAndInvokeBossState(currentHealth, 0.50f, ref _below50Triggered, 2);
            CheckAndInvokeBossState(currentHealth, 0.25f, ref _below25Triggered, 3);
        }

        private static void CheckAndInvokeBossState(float currentHealth, float threshold, ref bool triggered, int bossState)
        {
            if (!triggered && currentHealth <= threshold)
            {
                triggered = true;
                OnBossStateChange?.Invoke(bossState);
            }
        }

        private void OnPlayerDeath()
        {
            player.Die();
            gameObject.SetActive(false);
        }

        
        private void HandleMoveAxis(float value)
        {
            ApplyMovement(value);
            HandleMousePosition(Mouse.current.position.ReadValue());
        }
        
        //<<summary>>
        // Apply movement to the player in the x-axis (-1, 0, 1)
        //<<summary>>
        private void ApplyMovement(float value)
        {
            float targetVelocityX = value * player.MoveSpeed;
            
            Vector3 currentVelocity = _playerRigidbody.linearVelocity;
            
            _playerRigidbody.linearVelocity = new Vector3(targetVelocityX, currentVelocity.y, currentVelocity.z);

            if (IsGrounded())
            {
                
                OnPlayerMove?.Invoke(Mathf.Sign(_direction.x) * value);
            }
        }

        private void HandleJump()
        {
            if (player.JetpackFuel <= 0 || _onVerticalPlatform) return;
            
            if (!jetpackParticle1.isPlaying) jetpackParticle1.Play();
            if (!jetpackParticle2.isPlaying) jetpackParticle2.Play();

            _playerRigidbody.AddForce(Vector3.up * (player.FlySpeed * Time.deltaTime), ForceMode.VelocityChange);
            
            player.JetpackFuel -= Time.deltaTime * player.FuelConsumeRate;
            OnJetpackFuelChange?.Invoke(player.JetpackFuel / player.JetpackFuelMax);
            OnPlayerMove?.Invoke(0);
            
            ResetRefillTimer();
            HandleMousePosition(Mouse.current.position.ReadValue());
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
        
        private void ResetJetpackFuel()
        {
            player.JetpackFuel = player.JetpackFuelMax;
            OnJetpackFuelChange?.Invoke(player.JetpackFuel / player.JetpackFuelMax);
        }

        private void HandleFire()
        {
            _shootingController.FireBullet(_direction);
            OnBulletShoot?.Invoke();
        }

        private void ApplyDamage(int damage, GameObject hitObject)
        {
            if (hitObject == gameObject)
            {
                playerHealth.TakeDamage(damage);
                OnPlayerHit?.Invoke();
            }
            
        }
        
        // <<summary>>
        // Check if the player is grounded
        // <<summary>>
        private bool IsGrounded()
        {
            float distance = 1.5f;
            Vector3 direction = Vector3.down;
            return Physics.Raycast(transform.position, direction, distance);
        }
        
        private void DisableGravity()
        {
            if(_playerRigidbody == null) return;
            _playerRigidbody.useGravity = false;
            _playerRigidbody.linearVelocity = Vector3.zero;
            _onVerticalPlatform = true;
            jetpackParticle1.loop = true;
            jetpackParticle2.loop = true;
            jetpackParticle1.Play();
            jetpackParticle2.Play();
        }

    }
}