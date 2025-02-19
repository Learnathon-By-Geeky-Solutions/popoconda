using Game;
using UnityEngine;
using UnityEngine.UIElements;


namespace UI
{
    public class GameOver : MonoBehaviour
    {
        [SerializeField] private UIDocument gameOverDocument;
        private Button _retryButton;
        private Button _mainMenuButton;
        
        public delegate void StatEvent();
        public static event StatEvent UIEnableEvent;
        public static event StatEvent UIDisableEvent;
        
        public static event StatEvent RetryEvent;
        public static event StatEvent MenuEvent;
        
        private void Start()
        {
            VisualElement root = gameOverDocument.rootVisualElement;
            
            _retryButton = root.Q<Button>("retry-button");
            _mainMenuButton = root.Q<Button>("main-menu-button");
            
            gameOverDocument.rootVisualElement.style.display = DisplayStyle.None;
            UIDisableEvent?.Invoke();

            GameManager.LoseEvent += HandleGameOver;
            _retryButton.clicked += HandleRetryButtonClicked;
            _mainMenuButton.clicked += HandleMenuButtonClicked;
        }
        
        private void OnDestroy()
        {
            GameManager.LoseEvent -= HandleGameOver;
            _retryButton.clicked -= HandleRetryButtonClicked;
            _mainMenuButton.clicked -= HandleMenuButtonClicked;
        }
        
        private void HandleGameOver()
        {
            gameOverDocument.rootVisualElement.style.display = DisplayStyle.Flex;
            UIEnableEvent?.Invoke();
           
        }
        
        private static void HandleRetryButtonClicked()
        {
            RetryEvent?.Invoke();
            
        }
        
        private static void HandleMenuButtonClicked()
        {
            MenuEvent?.Invoke();
        }
    }
}
