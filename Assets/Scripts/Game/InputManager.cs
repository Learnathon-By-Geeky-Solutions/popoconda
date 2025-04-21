using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;
using System.Threading;
using System;

namespace Game
{
    public class InputManager : MonoBehaviour
    {
        private static InputManager Instance { get; set; }

        [SerializeField] private InputActionReference positionAction;
        [SerializeField] private InputActionReference fireAction;
        [SerializeField] private InputActionReference moveAction;
        [SerializeField] private InputActionReference jumpAction;
        [SerializeField] private InputActionReference interactAction;
        [SerializeField] private InputActionReference menuAction;
        [SerializeField] private InputActionReference cancelAction;
        [SerializeField] private InputActionReference submitAction;
        [SerializeField] private InputActionReference navigateAction;
        [SerializeField] private InputActionReference nextAction;


        public delegate void PositionChangeDelegate(Vector2 position);
        public delegate void AxisDelegate(float value);
        public delegate void SimpleActionDelegate();
        

        // Gameplay
        public static event PositionChangeDelegate OnMousePositionChanged;
        public static event AxisDelegate OnMoveAxisChanged;
        public static event SimpleActionDelegate OnJumpPressed;
        public static event SimpleActionDelegate OnFirePressed;
        public static event SimpleActionDelegate OnInteractPressed;
        public static event SimpleActionDelegate OnMenuPressed;
        
        
        // UI
        public static event SimpleActionDelegate OnSubmitPressed;
        public static event SimpleActionDelegate OnCancelPressed;
        
        public static event AxisDelegate OnNavigateAxisChanged;
        
        
        // Cutscene
        public static event SimpleActionDelegate OnNextPressed;

        private CancellationTokenSource _moveCts;
        private CancellationTokenSource _jumpCts;
        private CancellationTokenSource _fireCts;

        private Action<InputAction.CallbackContext> _movePerformed;
        private Action<InputAction.CallbackContext> _moveCanceled;
        private Action<InputAction.CallbackContext> _jumpPerformed;
        private Action<InputAction.CallbackContext> _jumpCanceled;
        private Action<InputAction.CallbackContext> _firePerformed;
        private Action<InputAction.CallbackContext> _fireCanceled;
        private Action<InputAction.CallbackContext> _interactPerformed;
        private Action<InputAction.CallbackContext> _menuPerformed;
        
        private Action<InputAction.CallbackContext> _submitPerformed;
        private Action<InputAction.CallbackContext> _cancelPerformed;
        private Action<InputAction.CallbackContext> _navigatePerformed;
        
        private Action<InputAction.CallbackContext> _nextPerformed;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            positionAction.action.Enable();
            moveAction.action.Enable();
            jumpAction.action.Enable();
            fireAction.action.Enable();
            menuAction.action.Enable();
            interactAction.action.Enable();
            
            navigateAction.action.Enable();
            submitAction.action.Enable();
            cancelAction.action.Enable();
            
            nextAction.action.Enable();

            positionAction.action.performed += HandlePositionChange;

            _movePerformed = _ => HandleMovePressed();
            _moveCanceled = _ => HandleMoveReleased();
            _jumpPerformed = _ => HandleJumpPressed();
            _jumpCanceled = _ => HandleJumpReleased();
            _firePerformed = _ => HandleFirePressed();
            _fireCanceled = _ => HandleFireReleased();
            _menuPerformed = _ => HandleMenuPressed();
            _interactPerformed = _ => HandleInteractPressed();
            
            _navigatePerformed = _ => HandleNavigatePressed();
            _submitPerformed = _ => HandleSubmitPressed();
            _cancelPerformed = _ => HandleCancelPressed();
            
            _nextPerformed = _ => HandleNextPressed();

            moveAction.action.performed += _movePerformed;
            moveAction.action.canceled += _moveCanceled;
            jumpAction.action.performed += _jumpPerformed;
            jumpAction.action.canceled += _jumpCanceled;
            fireAction.action.performed += _firePerformed;
            fireAction.action.canceled += _fireCanceled;
            menuAction.action.performed += _menuPerformed;
            interactAction.action.performed += _interactPerformed;
            
            navigateAction.action.performed += _navigatePerformed;
            submitAction.action.performed += _submitPerformed;
            cancelAction.action.performed += _cancelPerformed;
            
            nextAction.action.performed += _nextPerformed;
        }

        private void OnDisable()
        {
            positionAction.action.performed -= HandlePositionChange;
            
            moveAction.action.performed -= _movePerformed;
            moveAction.action.canceled -= _moveCanceled;
            jumpAction.action.performed -= _jumpPerformed;
            jumpAction.action.canceled -= _jumpCanceled;
            fireAction.action.performed -= _firePerformed;
            fireAction.action.canceled -= _fireCanceled;
            menuAction.action.performed -= _menuPerformed;
            interactAction.action.performed -= _interactPerformed;
            
            navigateAction.action.performed -= _navigatePerformed;
            submitAction.action.performed -= _submitPerformed;
            cancelAction.action.performed -= _cancelPerformed;
            
            nextAction.action.performed -= _nextPerformed;

            positionAction.action.Disable();
            moveAction.action.Disable();
            jumpAction.action.Disable();
            fireAction.action.Disable();
            menuAction.action.Disable();
            interactAction.action.Disable();
            
            navigateAction.action.Disable();
            submitAction.action.Disable();
            cancelAction.action.Disable();
            
            nextAction.action.Disable();

            // Properly clean up events
            OnMousePositionChanged = null;
            OnMoveAxisChanged = null;
            OnJumpPressed = null;
            OnFirePressed = null;
            OnMenuPressed = null;
            OnInteractPressed = null;
            OnNavigateAxisChanged = null;
            OnSubmitPressed = null;
            OnCancelPressed = null;
            OnNextPressed = null;

            // Dispose of all CancellationTokens
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

        private void HandleMovePressed()
        {
            _moveCts?.Cancel();
            _moveCts?.Dispose();
            _moveCts = new CancellationTokenSource();
            MoveAction(_moveCts.Token).Forget();
        }

        private void HandleMoveReleased()
        {
            _moveCts?.Cancel();
        }

        private void HandleJumpPressed()
        {
            _jumpCts?.Cancel();
            _jumpCts?.Dispose();
            _jumpCts = new CancellationTokenSource();
            JumpAction(_jumpCts.Token).Forget();
        }

        private void HandleJumpReleased()
        {
            _jumpCts?.Cancel();
        }

        private void HandleFirePressed()
        {
            _fireCts?.Cancel();
            _fireCts?.Dispose();
            _fireCts = new CancellationTokenSource();
            FireAction(_fireCts.Token).Forget();
        }

        private void HandleFireReleased()
        {
            _fireCts?.Cancel();
        }

        private static void HandleMenuPressed()
        {
            Debug.Log("Menu pressed");
            OnMenuPressed?.Invoke();
        }
        
        private static void HandleInteractPressed()
        {
            Debug.Log("Interact pressed");
            OnInteractPressed?.Invoke();
        }
        
        private void HandleNavigatePressed()
        {
            Debug.Log("Navigate pressed");
            OnNavigateAxisChanged?.Invoke(navigateAction.action.ReadValue<float>());
            Debug.Log("Navigate axis: " + navigateAction.action.ReadValue<float>());
        }
        
        private static void HandleSubmitPressed()
        {
            Debug.Log("Submit pressed");
            OnSubmitPressed?.Invoke();
        }

        private static void HandleCancelPressed()
        {
            Debug.Log("Cancel pressed");
            OnCancelPressed?.Invoke();
        }
        
        private static void HandleNextPressed()
        {
            Debug.Log("Next pressed");
            OnNextPressed?.Invoke();
        }

        private async UniTaskVoid MoveAction(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                OnMoveAxisChanged?.Invoke(moveAction.action.ReadValue<float>());
                await UniTask.Yield(PlayerLoopTiming.Update, token);
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
