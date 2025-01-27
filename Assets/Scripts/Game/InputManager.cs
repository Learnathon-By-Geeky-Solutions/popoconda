using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private InputActionReference positionAction;
        [SerializeField] private InputActionReference fireAction;
        [SerializeField] private InputActionReference moveAction;
        [SerializeField] private InputActionReference jumpAction;
        
        public delegate void PositionChangeDelegate(Vector2 position);
        public delegate void MoveAxisDelegate(float value);
        public delegate void SimpleActionDelegate();
        
        public event PositionChangeDelegate OnMousePositionChanged;
        public event MoveAxisDelegate OnMoveAxisChanged;
        public event SimpleActionDelegate OnJumpPressed;
        public event SimpleActionDelegate OnFirePressed;

        private void OnEnable()
        {
            positionAction.action.Enable();
            moveAction.action.Enable();
            jumpAction.action.Enable();
            fireAction.action.Enable();
            
            positionAction.action.performed += HandlePositionChange;
            moveAction.action.performed += HandleMoveAxisChange;
            jumpAction.action.performed += HandleJumpPressed;
            fireAction.action.performed += HandleFirePressed;
        }

        private void OnDisable()
        {
            positionAction.action.performed -= HandlePositionChange;
            moveAction.action.performed -= HandleMoveAxisChange;
            jumpAction.action.performed -= HandleJumpPressed;
            fireAction.action.performed -= HandleFirePressed;
            
            positionAction.action.Disable();
            moveAction.action.Disable();
            jumpAction.action.Disable();
            fireAction.action.Disable();
        }
        
        private void HandlePositionChange(InputAction.CallbackContext context)
        {
            OnMousePositionChanged?.Invoke(context.ReadValue<Vector2>());
        }

        private void HandleMoveAxisChange(InputAction.CallbackContext context)
        {
            OnMoveAxisChanged?.Invoke(context.ReadValue<float>());
        }

        private void HandleJumpPressed(InputAction.CallbackContext _)
        {
            OnJumpPressed?.Invoke();
        }

        private void HandleFirePressed(InputAction.CallbackContext _)
        {
            OnFirePressed?.Invoke();
        }
        
        private void Update()
        {
            if (moveAction.action.IsPressed())
            {
                OnMoveAxisChanged?.Invoke(moveAction.action.ReadValue<float>());
            }
            if (jumpAction.action.IsPressed())
            {
                OnJumpPressed?.Invoke();
            }
            if (fireAction.action.IsPressed())
            {
                OnFirePressed?.Invoke();
            }
        }
    }
}
