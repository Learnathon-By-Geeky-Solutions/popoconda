using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Cursor = UnityEngine.Cursor;

namespace UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private UIDocument mainMenuDocument;
        private Button _playButton;
        private Button _optionsButton;
        private Button _quitButton;
        
        public delegate void OnButtonClicked();
        public static event OnButtonClicked OptionButtonClicked;
        
        private void OnEnable()
        {
            VisualElement root = mainMenuDocument.rootVisualElement;
            
            _playButton = root.Q<Button>("play-button");
            _optionsButton = root.Q<Button>("option-button");
            _quitButton = root.Q<Button>("quit-button");
            Cursor.visible = true;
            _playButton.clicked += HandlePlayButtonClicked;
            _optionsButton.clicked += HandleOptionsButtonClicked;
            _quitButton.clicked += HandleQuitButtonClicked;
        }
        
        private void OnDisable()
        {
            _playButton.clicked -= HandlePlayButtonClicked;
            _optionsButton.clicked -= HandleOptionsButtonClicked;
            _quitButton.clicked -= HandleQuitButtonClicked;
        }
        
        private static void HandlePlayButtonClicked()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        
        private static void HandleOptionsButtonClicked()
        {
            OptionButtonClicked?.Invoke();
        }
        
        private static void HandleQuitButtonClicked()
        {
            Application.Quit();
            Debug.Log("Game quit");
        }
        
    }
    
}