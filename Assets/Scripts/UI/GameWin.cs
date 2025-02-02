using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Game;

namespace UI
{
    public class GameWin : MonoBehaviour
    {
        [SerializeField] private UIDocument gameWinDocument;
        private Button _nextLevelButton;
        private Button _mainMenuButton;
        
        private void Start()
        {
            VisualElement root = gameWinDocument.rootVisualElement;
            
            _nextLevelButton = root.Q<Button>("next-level-button");
            _mainMenuButton = root.Q<Button>("main-menu-button");
            
            gameWinDocument.rootVisualElement.style.display = DisplayStyle.None;
            
            GameManager.WinEvent += HandleGameWin;
            _nextLevelButton.clicked += HandleNextLevelButtonClicked;
            _mainMenuButton.clicked += HandleMenuButtonClicked;
            
        }
        
        private void OnDestroy()
        {
            GameManager.WinEvent -= HandleGameWin;
            _nextLevelButton.clicked -= HandleNextLevelButtonClicked;
            _mainMenuButton.clicked -= HandleMenuButtonClicked;
        }
        
        private void HandleGameWin()
        {
            gameWinDocument.rootVisualElement.style.display = DisplayStyle.Flex;
        }
        
        private static void HandleNextLevelButtonClicked()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        
        private static void HandleMenuButtonClicked()
        {
            Debug.Log("Main Menu button clicked");
            SceneManager.LoadScene(0);
        }
    }
}