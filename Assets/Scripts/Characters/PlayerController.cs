using UnityEngine;
using UnityEngine.InputSystem;
using Combat;
using UI;

namespace Characters
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private GameObject playerObject;
        private Rigidbody _playerRigidbody;
        [SerializeField] private GameObject playerGfx;
        [SerializeField] private GameObject gunRotatePoint;
        
        [SerializeField] private InputActionReference positionAction;
        [SerializeField] private InputActionReference fireAction;
        [SerializeField] private InputActionReference moveAction;
        [SerializeField] private InputActionReference jumpAction;
        
        private Camera _playerCamera;
        private Vector3 _mousePosition;
        
        [SerializeField] private float moveSpeed;
        
        [SerializeField] private float flySpeed;
        [SerializeField] private float jetpackFuelMax;
        private float _jetpackFuel;
        [SerializeField] private float fuelConsumeRate;
        [SerializeField] private float fuelFillRate;
        private bool _isJumping;
        
        private ShootingController _shootingController;
        private bool _isFiring;
        
        [SerializeField] private HudHandler hudHandler;
        
        private Health _playerHealth;
        public float MaxHealth =>_playerHealth.maxHealth;
        public float CurrentHealth =>_playerHealth.currentHealth;
        
        void Awake()
        {
            _playerCamera = Camera.main;
            _playerRigidbody = playerObject.GetComponent<Rigidbody>();
            _playerHealth = playerObject.GetComponent<Health>();
            _shootingController = playerObject.GetComponent<ShootingController>();
            
            
            _jetpackFuel = jetpackFuelMax;
        }

        void Update()
        {
            if (_isJumping)
            {
                HandleJetpack();
            }
            else
            {
                if (_jetpackFuel < jetpackFuelMax)
                {
                    _jetpackFuel += Time.deltaTime * fuelFillRate;
                    _jetpackFuel = Mathf.Clamp(_jetpackFuel, 0, jetpackFuelMax);
                }
            }
            Aim();
        }
        void FixedUpdate()
        {
            HandleMovement();
            if(_isFiring) HandleShooting();
            hudHandler.UpdateJetpackFuel(_jetpackFuel, jetpackFuelMax);
        }
        
        
        private void HandleMovement()
        {
            float xInput = moveAction.action.ReadValue<float>();
            _playerRigidbody.AddRelativeForce(Vector3.right * (xInput * moveSpeed * Time.fixedDeltaTime));
        }

        private void HandleJetpack()
        {
            if (_jetpackFuel > 0)
            {
                _playerRigidbody.AddForce(Vector3.up * (flySpeed * Time.deltaTime), ForceMode.VelocityChange);
                _jetpackFuel -= Time.deltaTime * fuelConsumeRate;
            }
        }
        
        private Vector3 _difference;
        private float _rotationZ;

        private void Aim()
        {
            
            if(_playerCamera == null) return;
            Vector2 screenPosition = positionAction.action.ReadValue<Vector2>();
            Ray ray = _playerCamera.ScreenPointToRay(screenPosition);

            Plane gunPlane = new Plane(Vector3.forward, gunRotatePoint.transform.position);

            if (gunPlane.Raycast(ray, out float distance))
            {
                _mousePosition = ray.GetPoint(distance);
            }

            _difference = _mousePosition - gunRotatePoint.transform.position;
            _rotationZ = Mathf.Atan2(_difference.y, _difference.x) * Mathf.Rad2Deg;

            gunRotatePoint.transform.rotation = Quaternion.Euler(0f, 0f, _rotationZ);
            FlipPlayerGraphics(_rotationZ);
        }
        
        private void FlipPlayerGraphics(float rotation)
        {
            if (rotation > 90 || rotation < -90)
            {
                playerGfx.transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                playerGfx.transform.localScale = new Vector3(1, 1, 1);
            }
        }
        
        private void HandleShooting()
        {
            _shootingController.FireBullet(_difference.normalized);
        }
        
        private void OnEnable()
        {
            moveAction.action.Enable();
            jumpAction.action.Enable();
            fireAction.action.Enable();

            jumpAction.action.started += OnJumpStart;
            jumpAction.action.canceled += OnJumpEnd;
            
            fireAction.action.performed += OnFireStart;
            fireAction.action.canceled += OnFireEnd;
        }

        private void OnDisable()
        {
            moveAction.action.Disable();
            jumpAction.action.Disable();
            fireAction.action.Disable();

            jumpAction.action.started -= OnJumpStart;
            jumpAction.action.canceled -= OnJumpEnd;
            
            fireAction.action.performed -= OnFireStart;
            fireAction.action.canceled -= OnFireEnd;
        }

        private enum InputActionState { Start, End }

        private void HandleJump(InputActionState state)
        {
            _isJumping = (state == InputActionState.Start);
        }

        private void HandleFire(InputActionState state)
        {
            _isFiring = (state == InputActionState.Start);
        }
        
        private void OnJumpStart(InputAction.CallbackContext context) => HandleJump(InputActionState.Start);
        private void OnJumpEnd(InputAction.CallbackContext context) => HandleJump(InputActionState.End);
        private void OnFireStart(InputAction.CallbackContext context) => HandleFire(InputActionState.Start);
        private void OnFireEnd(InputAction.CallbackContext context) => HandleFire(InputActionState.End);


    }
}
