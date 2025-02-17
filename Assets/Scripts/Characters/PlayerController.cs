using System;
using UnityEngine;
using Combat;
using Game;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.InputSystem;
using Studio23.SS2.SaveSystem.Interfaces;

namespace Characters
{
    public class PlayerController : MonoBehaviour, ISaveable
    {
        private Rigidbody _playerRigidbody;
        [SerializeField] private Texture2D crosshairTexture;

        private Camera _playerCamera;
        [SerializeField] private Health playerHealth;
        [SerializeField] private Player player;

        private ShootingController _shootingController;
        private Vector3 _direction;

        private bool _isStunned;
        private static int _currentHealth = 150;

        private CancellationTokenSource _cancellationTokenSource;

        public delegate void StateEventWithFloat(float value);
        public static event StateEventWithFloat OnPlayerMove;

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

        private void OnEnable()
        {
            InputManager.OnMousePositionChanged += HandleMousePosition;
            InputManager.OnMoveAxisChanged += HandleMoveAxis;
            InputManager.OnJumpPressed += HandleJump;
            InputManager.OnFirePressed += HandleFire;
            playerHealth.OnHealthChange += UpdateHealthUI;
            playerHealth.OnDeath += OnPlayerDeath;
            GameManager.SetPlayerTransform(transform);
            EnergyBlast.OnEnergyBlastHit += ApplyBlastDamage;
            Bullet.OnBulletHit += ApplyDamage;
            FireLaser.OnLaserHit += ApplyDamage;
            StunController.OnStun += Stunned;
        }

        private void OnDestroy()
        {
            InputManager.OnMousePositionChanged -= HandleMousePosition;
            InputManager.OnMoveAxisChanged -= HandleMoveAxis;
            InputManager.OnJumpPressed -= HandleJump;
            InputManager.OnFirePressed -= HandleFire;
            playerHealth.OnHealthChange -= UpdateHealthUI;
            playerHealth.OnDeath -= OnPlayerDeath;
            EnergyBlast.OnEnergyBlastHit -= ApplyBlastDamage;
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
                UpdateGunRotation(ray.GetPoint(distance));
            }
        }

        private void UpdateGunRotation(Vector3 targetPosition)
        {
            _direction = targetPosition - player.GunRotatePoint.transform.position;
            float rotationZ = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;

            if (player.PlayerGfx.transform.localScale.x < 0)
                rotationZ += 180f;

            player.PlayerGfx.transform.localScale = _direction.x < 0 ? new Vector3(-1, 1, 1) : Vector3.one;
            player.GunRotatePoint.transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);
        }

        private void OnGUI()
        {
            if (crosshairTexture != null)
            {
                Vector2 mousePosition = Event.current.mousePosition;
                float crosshairSize = 32f; // Adjust size as needed
                GUI.DrawTexture(new Rect(mousePosition.x - crosshairSize / 2, mousePosition.y - crosshairSize / 2, crosshairSize, crosshairSize), crosshairTexture);
            }
        }

        private static void UpdateHealthUI(float currentHealth)
        {
            OnPlayerHealthChange?.Invoke(currentHealth);
        }

        private void OnPlayerDeath()
        {
            player.Die();
            gameObject.SetActive(false);
        }

        private void HandleMoveAxis(float value)
        {
            if (_isStunned) return;

            ApplyMovement(value);
            HandleMousePosition(Mouse.current.position.ReadValue());
        }

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
            if (_isStunned || player.JetpackFuel <= 0) return;

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

        private void HandleFire()
        {
            if (_isStunned) return;
            _shootingController.FireBullet(_direction);
        }

        private void ApplyDamage(int damage, GameObject hitObject)
        {
            if (hitObject == gameObject)
            {
                _currentHealth -= damage;
                playerHealth.TakeDamage(damage);
            }
        }

        private void ApplyBlastDamage(int damage)
        {
            _currentHealth -= damage;
            playerHealth.TakeDamage(damage);
        }

        private void Stunned(bool isStunned)
        {
            _isStunned = isStunned;
        }

        private bool IsGrounded()
        {
            float distance = 1.5f;
            Vector3 direction = Vector3.down;
            return Physics.Raycast(transform.position, direction, distance);
        }

        // Unique ID for saving
        public string GetUniqueID()
        {
            return "PlayerControllerUniqueID";
        }

        public bool IsDirty { get; set; } = true;

        public UniTask<string> GetSerializedData()
        {
            PlayerState state = new PlayerState(_currentHealth);
            return UniTask.FromResult(JsonUtility.ToJson(state));
        }

        public UniTask AssignSerializedData(string data)
        {
            if (!string.IsNullOrEmpty(data))
            {
                PlayerState state = JsonUtility.FromJson<PlayerState>(data);
                _currentHealth = state.Health;
            }
            return UniTask.CompletedTask;
        }

        [Serializable]
        private class PlayerState
        {
            public int Health;

            public PlayerState(int health)
            {
                Health = health;
            }
        }
    }
}
