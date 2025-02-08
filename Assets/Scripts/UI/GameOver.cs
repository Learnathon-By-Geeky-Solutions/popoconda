using Game;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

namespace UI
{
    public class GameOver : MonoBehaviour
    {
        [SerializeField] private UIDocument gameOverDocument;
        private Button _retryButton;
        private Button _mainMenuButton;
        
        private void Start()
        {
            VisualElement root = gameOverDocument.rootVisualElement;
            
            _retryButton = root.Q<Button>("retry-button");
            _mainMenuButton = root.Q<Button>("main-menu-button");
            
            gameOverDocument.rootVisualElement.style.display = DisplayStyle.None;

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
            Cursor.visible = true;
        }
        
        private static void HandleRetryButtonClicked()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
        private static void HandleMenuButtonClicked()
        {
            SceneManager.LoadScene(1);
        }
    }
}
