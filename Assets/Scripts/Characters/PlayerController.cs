using UnityEngine;
using Combat;
using Game;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.InputSystem;

namespace Characters
{
    public class PlayerController : MonoBehaviour
    {
        private Rigidbody _playerRigidbody;
        private UnityEngine.Camera _playerCamera;

        [SerializeField] private Health playerHealth;
        [SerializeField] private Player player;

        private ShootingController _shootingController;
        private Vector3 _direction;

        private bool _isStunned;

        // 🔫 Fire rate control
        [SerializeField] private float fireCooldown = 0.35f;
        private float _nextFireTime;

        // 🚀 Jetpack smoothing
        [SerializeField] private float jetpackForce = 8f;
        [SerializeField] private float maxUpwardSpeed = 6f;

        private CancellationTokenSource _cancellationTokenSource;

        public delegate void StateEventWithFloat(float value);
        public delegate void StateEvent();
        public static event StateEvent onPlayerHit;
        public static event StateEventWithFloat OnPlayerMove;

        public static event Health.StatEventWithFloat OnPlayerHealthChange;
        public static event Health.StatEventWithFloat OnJetpackFuelChange;

        private void Awake()
        {
            _playerCamera = UnityEngine.Camera.main;
            _playerRigidbody = GetComponent<Rigidbody>();
            _shootingController = GetComponent<ShootingController>();

            Cursor.visible = false;

            player.Initialize();

            // ❌ Auto-heal OFF
            playerHealth.Initialize(false);

            OnJetpackFuelChange?.Invoke(player.JetpackFuel / player.JetpackFuelMax);
        }

        private void Start()
        {
            PlayerSpawner.OnCutsceneEnd += () =>
                GameManager.SetPlayerTransform(transform);
        }

        private void OnEnable()
        {
            InputManager.OnMousePositionChanged += HandleMousePosition;
            InputManager.OnMoveAxisChanged += HandleMoveAxis;
            InputManager.OnJumpPressed += HandleJetpack;
            InputManager.OnFirePressed += HandleFire;

            playerHealth.OnHealthChange += UpdateHealthUI;
            playerHealth.OnDeath += OnPlayerDeath;

            Bullet.OnBulletHit += ApplyDamage;
            FireLaser.OnLaserHit += ApplyDamage;
            EnergyBlast.OnEnergyBlastHit += ApplyBlastDamage;

            StunController.OnStun += Stunned;
        }

        private void OnDestroy()
        {
            InputManager.OnMousePositionChanged -= HandleMousePosition;
            InputManager.OnMoveAxisChanged -= HandleMoveAxis;
            InputManager.OnJumpPressed -= HandleJetpack;
            InputManager.OnFirePressed -= HandleFire;

            playerHealth.OnHealthChange -= UpdateHealthUI;
            playerHealth.OnDeath -= OnPlayerDeath;

            Bullet.OnBulletHit -= ApplyDamage;
            FireLaser.OnLaserHit -= ApplyDamage;
            EnergyBlast.OnEnergyBlastHit -= ApplyBlastDamage;

            StunController.OnStun -= Stunned;

            GameManager.ClearPlayerTransform();

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }

        // ===================== AIM =====================
        private void HandleMousePosition(Vector2 screenPosition)
        {
            if (_isStunned || _playerCamera == null) return;

            Ray ray = _playerCamera.ScreenPointToRay(screenPosition);
            Plane plane = new Plane(Vector3.forward, player.GunRotatePoint.transform.position);

            if (plane.Raycast(ray, out float distance))
                UpdateGunRotation(ray.GetPoint(distance));
        }

        private void UpdateGunRotation(Vector3 target)
        {
            _direction = target - player.GunRotatePoint.transform.position;
            float rotZ = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;

            bool facingLeft = _direction.x < 0;
            player.PlayerGfx.transform.localScale = facingLeft ? new Vector3(-1, 1, 1) : Vector3.one;
            player.GunRotatePoint.transform.rotation =
                Quaternion.Euler(0, 0, facingLeft ? rotZ + 180f : rotZ);
        }

        // ===================== MOVE =====================
        private void HandleMoveAxis(float value)
        {
            if (_isStunned) return;

            Vector3 v = _playerRigidbody.linearVelocity;
            v.x = value * player.MoveSpeed;
            _playerRigidbody.linearVelocity = v;

            if (IsGrounded())
                OnPlayerMove?.Invoke(value);
        }

        // ===================== JETPACK (SMOOTH) =====================
        private void HandleJetpack()
        {
            if (_isStunned || player.JetpackFuel <= 0f) return;

            Vector3 v = _playerRigidbody.linearVelocity;

            if (v.y < maxUpwardSpeed)
            {
                _playerRigidbody.AddForce(Vector3.up * jetpackForce, ForceMode.Acceleration);
            }

            player.JetpackFuel -= Time.deltaTime * player.FuelConsumeRate;
            OnJetpackFuelChange?.Invoke(player.JetpackFuel / player.JetpackFuelMax);

            ResetRefillTimer();
        }

        private void ResetRefillTimer()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
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

        // ===================== FIRE =====================
        private void HandleFire()
        {
            if (_isStunned || Time.time < _nextFireTime) return;

            _nextFireTime = Time.time + fireCooldown;
            _shootingController.FireBullet(_direction);
        }

        // ===================== DAMAGE =====================
        private void ApplyDamage(int damage, GameObject hit)
        {
            if (hit != gameObject) return;
            playerHealth.TakeDamage(damage);
            onPlayerHit?.Invoke();
        }

        private void ApplyBlastDamage(int damage)
        {
            playerHealth.TakeDamage(damage);
            onPlayerHit?.Invoke();
        }

        private static void UpdateHealthUI(float current)
        {
            OnPlayerHealthChange?.Invoke(current);
        }

        private void OnPlayerDeath()
        {
            player.Die();
            gameObject.SetActive(false);
        }

        private void Stunned(bool value) => _isStunned = value;

        private bool IsGrounded()
        {
            return Physics.Raycast(transform.position, Vector3.down, 1.5f);
        }
    }
}