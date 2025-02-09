using UnityEngine;
using Game;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;
using UnityEngine.SceneManagement;

namespace UI
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private UIDocument pauseMenuDocument;
        private Button _resumeButton;
        private Button _restartButton;
        private Button _mainMenuButton;
        
        public delegate void UIEvent();
        public static event UIEvent UIEnableEvent;
        public static event UIEvent UIDisableEvent;
        
        private void Start()
        {
            VisualElement root = pauseMenuDocument.rootVisualElement;
            
            _resumeButton = root.Q<Button>("resume-button");
            _restartButton = root.Q<Button>("restart-button");
            _mainMenuButton = root.Q<Button>("main-menu-button");
            
            pauseMenuDocument.rootVisualElement.style.display = DisplayStyle.None;
            UIDisableEvent?.Invoke();
            
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
                Cursor.visible = true;
            }
        }
        
        private void HandleResumeButtonClicked()
        {
            Time.timeScale = 1;
            Cursor.visible = false;
            pauseMenuDocument.rootVisualElement.style.display = DisplayStyle.None;
            UIDisableEvent?.Invoke();
            Cursor.visible = false;
        }
        
        private void HandleRestartButtonClicked()
        {
            Time.timeScale = 1;
            Cursor.visible = false;
            pauseMenuDocument.rootVisualElement.style.display = DisplayStyle.None;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
        private void HandleMainMenuButtonClicked()
        {
            Time.timeScale = 1;
            Cursor.visible = false;
            pauseMenuDocument.rootVisualElement.style.display = DisplayStyle.None;
            SceneManager.LoadScene(1);
        }
    }
}
