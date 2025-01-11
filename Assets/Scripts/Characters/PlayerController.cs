using UnityEngine;
using UnityEngine.InputSystem;
using Combat;

namespace Characters
{
    public class PlayerController : MonoBehaviour
    {
        public GameObject playerObject;
        public Rigidbody playerRigidbody;

        public InputActionReference moveAction;
        public InputActionReference jumpAction;

        [SerializeField] private float moveSpeed;
        [SerializeField] private float flySpeed;
        [SerializeField] private float jetpackFuel;
        [SerializeField] private float fuelConsumeRate;
        [SerializeField] private float fuelFillRate;
        private Health _playerHealth;
        public float maxHealth;
        public float currentHealth;


        private bool _isJumping;

        void Start()
        {
            playerRigidbody = playerObject.GetComponent<Rigidbody>();
            _playerHealth = playerObject.GetComponent<Health>();
        }

        void Update()
        {
            // Get the 1d axis input from the player
            float xInput = moveAction.action.ReadValue<float>();

            // Move the player left and right
            playerRigidbody.AddRelativeForce(Vector3.right * (xInput * Time.deltaTime * moveSpeed));

            // Jetpack logic
            if (_isJumping && jetpackFuel > 0)
            {
                playerRigidbody.AddForce(Vector3.up * (flySpeed * Time.deltaTime), ForceMode.VelocityChange);
                jetpackFuel -= Time.deltaTime * fuelConsumeRate;
            }
            else if (jetpackFuel < 5 && !_isJumping)
            {
                jetpackFuel += Time.deltaTime * fuelFillRate;
            }
            if (_playerHealth)
            {
                currentHealth = _playerHealth.currentHealth;
                maxHealth = _playerHealth.maxHealth;
            }
        }

        private void OnEnable()
        {
            moveAction.action.Enable();
            jumpAction.action.Enable();

            jumpAction.action.started += OnJumpStart;
            jumpAction.action.canceled += OnJumpEnd;
        }

        private void OnDisable()
        {
            moveAction.action.Disable();
            jumpAction.action.Disable();

            jumpAction.action.started -= OnJumpStart;
            jumpAction.action.canceled -= OnJumpEnd;
        }

        private void OnJumpStart(InputAction.CallbackContext context)
        {
            _isJumping = true;
        }

        private void OnJumpEnd(InputAction.CallbackContext context)
        {
            _isJumping = false;
        }
    }
}
