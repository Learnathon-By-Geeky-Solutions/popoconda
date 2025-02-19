using UnityEngine;
using Game;
using UnityEngine.UIElements;


namespace UI
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private UIDocument pauseMenuDocument;
        private Button _resumeButton;
        private Button _restartButton;
        private Button _mainMenuButton;
        
        public delegate void StatEvent();
        public static event StatEvent UIEnableEvent;
        public static event StatEvent UIDisableEvent;
        public static event StatEvent RestartEvent;
        public static event StatEvent MainMenuEvent;
        
        
        private void Start()
        {
            VisualElement root = pauseMenuDocument.rootVisualElement;
            
            _resumeButton = root.Q<Button>("resume-button");
            _restartButton = root.Q<Button>("restart-button");
            _mainMenuButton = root.Q<Button>("main-menu-button");
            
            pauseMenuDocument.rootVisualElement.style.display = DisplayStyle.None;
            UIDisableEvent?.Invoke();
            HandleResumeButtonClicked();
            
            _resumeButton.clicked += HandleResumeButtonClicked;
            _restartButton.clicked += HandleRestartButtonClicked;
            _mainMenuButton.clicked += HandleMainMenuButtonClicked;
        }
        
        private void OnEnable()
        {
            InputManager.OnMenuPressed += HandlePause;
            InputManager.OnCancelPressed += HandlePause;
            
        }
        private void OnDisable()
        {
            InputManager.OnMenuPressed -= HandlePause;
            InputManager.OnCancelPressed -= HandlePause;
            _resumeButton.clicked -= HandleResumeButtonClicked;
            _restartButton.clicked -= HandleRestartButtonClicked;
            _mainMenuButton.clicked -= HandleMainMenuButtonClicked;
        }
        
        private void HandlePause()
        {
            if ((int)Time.timeScale == 0)
            {
                HandleResumeButtonClicked();
            }
            else
            {
                Time.timeScale = 0;
                Debug.Log("Pause Menu Opened");
                pauseMenuDocument.rootVisualElement.style.display = DisplayStyle.Flex;
                UIEnableEvent?.Invoke();
            }
        }
        
        private void HandleResumeButtonClicked()
        {
            Time.timeScale = 1;
            pauseMenuDocument.rootVisualElement.style.display = DisplayStyle.None;
            UIDisableEvent?.Invoke();
        }
        
        private void HandleRestartButtonClicked()
        {
            Time.timeScale = 1;
            pauseMenuDocument.rootVisualElement.style.display = DisplayStyle.None;
            RestartEvent?.Invoke();
        }
        
        private void HandleMainMenuButtonClicked()
        {
            Time.timeScale = 1;
            pauseMenuDocument.rootVisualElement.style.display = DisplayStyle.None;
            MainMenuEvent?.Invoke();
        }
    }
}
