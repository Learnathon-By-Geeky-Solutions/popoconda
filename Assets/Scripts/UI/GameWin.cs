using UnityEngine;
using UnityEngine.UIElements;
using Game;
using Cursor = UnityEngine.Cursor;

namespace UI
{
    public class GameWin : MonoBehaviour
    {
        [SerializeField] private UIDocument gameWinDocument;
        private Button _nextLevelButton;
        private Button _mainMenuButton;
        
        public delegate void StatEvent();
        public static event StatEvent UIEnableEvent;
        public static event StatEvent UIDisableEvent;
        public static event StatEvent NextLevelEvent;
        public static event StatEvent MenuEvent;
        
        private void Start()
        {
            VisualElement root = gameWinDocument.rootVisualElement;
            
            _nextLevelButton = root.Q<Button>("next-level-button");
            _mainMenuButton = root.Q<Button>("main-menu-button");
            
            gameWinDocument.rootVisualElement.style.display = DisplayStyle.None;
            UIDisableEvent?.Invoke();
            
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
            UIEnableEvent?.Invoke();
            Cursor.visible = true;
        }
        
        private static void HandleNextLevelButtonClicked()
        {
            NextLevelEvent?.Invoke();
        }
        
        private static void HandleMenuButtonClicked()
        {
            Debug.Log("Main Menu button clicked");
            MenuEvent?.Invoke();
        }
    }
}