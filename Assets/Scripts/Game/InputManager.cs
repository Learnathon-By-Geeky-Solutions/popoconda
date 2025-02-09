using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;
using System.Threading;

namespace Game
{
    public class InputManager : MonoBehaviour
    {
        private static InputManager _instance;
        
        [SerializeField] private InputActionReference positionAction;
        [SerializeField] private InputActionReference fireAction;
        [SerializeField] private InputActionReference moveAction;
        [SerializeField] private InputActionReference jumpAction;
        [SerializeField] private InputActionReference menuAction;
        [SerializeField] private InputActionReference cancelAction;
        
        public delegate void PositionChangeDelegate(Vector2 position);
        public delegate void MoveAxisDelegate(float value);
        public delegate void SimpleActionDelegate();
        
        public static event PositionChangeDelegate OnMousePositionChanged;
        public static event MoveAxisDelegate OnMoveAxisChanged;
        public static event SimpleActionDelegate OnJumpPressed;
        public static event SimpleActionDelegate OnFirePressed;
        public static event SimpleActionDelegate OnMenuPressed;
        
        public static event SimpleActionDelegate OnCancelPressed;

        private CancellationTokenSource _moveCts;
        private CancellationTokenSource _jumpCts;
        private CancellationTokenSource _fireCts;
        
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }


        private void OnEnable()
        {
            positionAction.action.Enable();
            moveAction.action.Enable();
            jumpAction.action.Enable();
            fireAction.action.Enable();
            menuAction.action.Enable();
            cancelAction.action.Enable();
            
            positionAction.action.performed += HandlePositionChange;
            moveAction.action.performed += HandleMovePressed;
            moveAction.action.canceled += HandleMoveReleased;
            jumpAction.action.performed += HandleJumpPressed;
            jumpAction.action.canceled += HandleJumpReleased;
            fireAction.action.performed += HandleFirePressed;
            fireAction.action.canceled += HandleFireReleased;
            menuAction.action.performed += HandleMenuPressed;
            cancelAction.action.performed += HandleCancelPressed;
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
            menuAction.action.performed -= HandleMenuPressed;
            cancelAction.action.performed -= HandleCancelPressed;
            
            positionAction.action.Disable();
            moveAction.action.Disable();
            jumpAction.action.Disable();
            fireAction.action.Disable();
            menuAction.action.Disable();
            cancelAction.action.Disable();
            
            // Cancel all the CancellationTokenSources
            _moveCts?.Cancel();
            _moveCts?.Dispose();
            _jumpCts?.Cancel();
            _jumpCts?.Dispose();
            _fireCts?.Cancel();
            _fireCts?.Dispose();
        }
        
        private static void HandlePositionChange(InputAction.CallbackContext context)
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
        
        private static void HandleMenuPressed(InputAction.CallbackContext _)
        {
            Debug.Log("Menu pressed");
            OnMenuPressed?.Invoke();
        }
        
        private static void HandleCancelPressed(InputAction.CallbackContext _)
        {
            Debug.Log("Cancel pressed");
            OnCancelPressed?.Invoke();
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

        
        private static async UniTaskVoid JumpAction(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                OnJumpPressed?.Invoke();
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }
        
        private static async UniTaskVoid FireAction(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                OnFirePressed?.Invoke();
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }
    }
}
