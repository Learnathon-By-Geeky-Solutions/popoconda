using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

namespace UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private UIDocument mainMenuDocument;
        private Button _playButton;
        private Button _optionsButton;
        private Button _quitButton;
        
        private void Start()
        {
            VisualElement root = mainMenuDocument.rootVisualElement;
            
            _playButton = root.Q<Button>("play-button");
            _optionsButton = root.Q<Button>("option-button");
            _quitButton = root.Q<Button>("quit-button");
            
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
            Debug.Log("Options button clicked");
        }
        
        private static void HandleQuitButtonClicked()
        {
            Application.Quit();
            Debug.Log("Game quit");
        }
        
    }
    
}