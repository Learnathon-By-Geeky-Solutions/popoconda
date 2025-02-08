using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;
using System.Threading;

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

        private CancellationTokenSource _moveCts;
        private CancellationTokenSource _jumpCts;
        private CancellationTokenSource _fireCts;

        private void OnEnable()
        {
            positionAction.action.Enable();
            moveAction.action.Enable();
            jumpAction.action.Enable();
            fireAction.action.Enable();
            
            positionAction.action.performed += HandlePositionChange;
            moveAction.action.performed += HandleMovePressed;
            moveAction.action.canceled += HandleMoveReleased;
            jumpAction.action.performed += HandleJumpPressed;
            jumpAction.action.canceled += HandleJumpReleased;
            fireAction.action.performed += HandleFirePressed;
            fireAction.action.canceled += HandleFireReleased;
        }

        private void OnDisable()
        {
            positionAction.action.performed -= HandlePositionChange;
            moveAction.action.performed -= HandleMovePressed;
            moveAction.action.canceled -= HandleMoveReleased;
            jumpAction.action.performed -= HandleJumpPressed;
            jumpAction.action.canceled -= HandleJumpReleased;
            fireAction.action.performed -= HandleFirePressed;
            fireAction.action.canceled -= HandleFireReleased;
            
            positionAction.action.Disable();
            moveAction.action.Disable();
            jumpAction.action.Disable();
            fireAction.action.Disable();
            
            // Cancel all the CancellationTokenSources
            _moveCts?.Cancel();
            _moveCts?.Dispose();
            _jumpCts?.Cancel();
            _jumpCts?.Dispose();
            _fireCts?.Cancel();
            _fireCts?.Dispose();
        }
        
        private void HandlePositionChange(InputAction.CallbackContext context)
        {
            OnMousePositionChanged?.Invoke(context.ReadValue<Vector2>());
        }

        private void HandleMovePressed(InputAction.CallbackContext context)
        {
            _moveCts = new CancellationTokenSource();
            MoveAction(_moveCts.Token).Forget();
        }
        
        private void HandleMoveReleased(InputAction.CallbackContext context)
        {
            _moveCts?.Cancel();
        }
        
        private void HandleJumpPressed(InputAction.CallbackContext _)
        {
            _jumpCts = new CancellationTokenSource();
            JumpAction(_jumpCts.Token).Forget();
        }
        
        private void HandleJumpReleased(InputAction.CallbackContext _)
        {
            _jumpCts?.Cancel();
        }

        private void HandleFirePressed(InputAction.CallbackContext _)
        {
            _fireCts = new CancellationTokenSource();
            FireAction(_fireCts.Token).Forget();
        }
        
        private void HandleFireReleased(InputAction.CallbackContext _)
        {
            _fireCts?.Cancel();
        }
        
        private async UniTaskVoid MoveAction(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                float moveValue = moveAction.action.ReadValue<float>();
                OnMoveAxisChanged?.Invoke(moveValue);
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate, token);
            }
        }

        
        private async UniTaskVoid JumpAction(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                OnJumpPressed?.Invoke();
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }
        
        private async UniTaskVoid FireAction(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                OnFirePressed?.Invoke();
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }
    }
}
