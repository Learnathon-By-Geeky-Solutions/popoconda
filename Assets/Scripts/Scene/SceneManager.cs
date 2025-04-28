using Characters;
using Cutscene;
using Cysharp.Threading.Tasks;
using Dialogue;
using UnityEngine;
using UnityEngine.AddressableAssets;
using USM = UnityEngine.SceneManagement;
using UI;
using Game;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Scene
{
     public class SceneManager : MonoBehaviour
    {
        public SceneData sceneData; // Reference to the Scriptable Object

        private static SceneManager Instance { get; set; }

        private SceneInstance _mainMenuInstance;
        private SceneInstance _gameUIInstance;
        private SceneInstance _optionMenuInstance;
        private SceneInstance _levelSelectMenuInstance;
        private SceneInstance _levelInstance;
        private SceneInstance _dialogueInstance;
        private SceneInstance _verticalPlatformInstance;
        
        private int _currentLevelIndex = -1;
        
        public delegate void StateEventWithInt(int value);
        public static event StateEventWithInt OnLevelUnlock;
        public static event StateEventWithInt OnLevelLoaded;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            MainMenu.PlayButtonClicked += LoadLevelSelectScene;
            MainMenu.OptionButtonClicked += LoadOptionMenu;
            OptionMenu.BackButtonClicked += LoadMainMenu;
            LevelSelectMenu.levelEvent += LoadLevel;
            LevelSelectMenu.backEvent += LoadMainMenu;
            CutsceneManager.OnCutsceneStart += UnloadGameUI;
            CutsceneManager.OnCutsceneEnd += LoadGameUI;
            GameOver.RetryEvent += LoadCurrentLevel;
            GameOver.MenuEvent += LoadMainMenu;
            GameWin.MenuEvent += LoadMainMenu;
            GameWin.NextLevelEvent += LoadNextLevel;
            PauseMenu.RestartEvent += LoadCurrentLevel;
            PauseMenu.MainMenuEvent += LoadMainMenu;
            MainMenuLoader.MainMenuEvent += LoadMainMenu;
            DialogueManager.OnDialogueEnd += UnloadDialogue;
            HeroAI.OnHeroSurvive += LoadVerticalPlatform;
        }
        
        private void OnDisable()
        {
            MainMenu.PlayButtonClicked -= LoadLevelSelectScene;
            MainMenu.OptionButtonClicked -= LoadOptionMenu;
            OptionMenu.BackButtonClicked -= LoadMainMenu;
            LevelSelectMenu.levelEvent -= LoadLevel;
            LevelSelectMenu.backEvent -= LoadMainMenu;
            CutsceneManager.OnCutsceneStart -= UnloadGameUI;
            CutsceneManager.OnCutsceneEnd -= LoadGameUI;
            GameOver.RetryEvent -= LoadCurrentLevel;
            GameOver.MenuEvent -= LoadMainMenu;
            GameWin.MenuEvent -= LoadMainMenu;
            GameWin.NextLevelEvent -= LoadNextLevel;
            PauseMenu.RestartEvent -= LoadCurrentLevel;
            PauseMenu.MainMenuEvent -= LoadMainMenu;
            MainMenuLoader.MainMenuEvent -= LoadMainMenu;
            //DialogueManager.OnDialogueEnd -= UnloadDialogue;
            HeroAI.OnHeroSurvive -= LoadVerticalPlatform;
        }
        
        private void LoadMainMenu()
        {
            if (sceneData.MainMenuScene == null)
            {
                return;
            }

            Addressables.LoadSceneAsync(sceneData.MainMenuScene);
        }
        
        private void LoadOptionMenu()
        {
            if (sceneData.OptionMenuScene == null)
            {
                return;
            }
            
            Addressables.LoadSceneAsync(sceneData.OptionMenuScene);
        }
        
        private void LoadLevelSelectScene()
        {
            if (sceneData.LevelSelectScene == null)
            {
                Debug.LogError("Level Select scene not assigned in SceneDataSO!");
                return;
            }
            
            Addressables.LoadSceneAsync(sceneData.LevelSelectScene);
        }

        private void LoadNextLevel()
        {
            if (sceneData.Levels.Count == 0)
            {
                Debug.LogError("No levels found in SceneDataSO!");
                return;
            }
            
            OnLevelUnlock?.Invoke(_currentLevelIndex+1);

            Debug.Log("Unlocked levels: " + LevelManager.UnlockedLevels);

            _currentLevelIndex++;
            if (_currentLevelIndex >= sceneData.Levels.Count)
            {
                Debug.Log("No more levels to load. Loading Main Menu.");
                LoadMainMenu();
                return;
            }
            
            LoadLevel(sceneData.Levels[_currentLevelIndex]);
        }

        private async void LoadCurrentLevel()
        {
            if (sceneData.Levels.Count == 0)
            {
                Debug.LogError("No levels found in SceneDataSO!");
                return;
            }

            // Check if the current level is valid
            if (_levelInstance.Scene.IsValid())
            {
                // Unload the current level scene
                await Addressables.UnloadSceneAsync(_levelInstance).ToUniTask();
                _levelInstance = default;
            }

            // Load the level again
            LoadLevel(sceneData.Levels[_currentLevelIndex]);
        }

        private void LoadLevel(int level)
        {
            if (sceneData.Levels.Count == 0)
            {
                Debug.LogError("No levels found in SceneDataSO!");
                return;
            }
            
            _currentLevelIndex = level;
            LoadLevel(sceneData.Levels[_currentLevelIndex]);
        }

        private void LoadLevel(AssetReference levelReference)
        {
            Debug.Log("Corrent level index: " + _currentLevelIndex);
            
            levelReference.LoadSceneAsync().Completed += handle =>
            {
                _levelInstance = handle.Result;
                ReSubscribeToCutsceneEvent();
            };
        }
        
        private void LoadVerticalPlatform()
        {
            if (sceneData.VerticalPlatformScene == null)
            {
                Debug.LogError("Vertical Platform scene not assigned in SceneDataSO!");
                return;
            }

            Addressables.LoadSceneAsync(sceneData.VerticalPlatformScene, USM.LoadSceneMode.Additive).Completed += handle =>
            {
                _verticalPlatformInstance = handle.Result;
            };
        }
        
        private void ReSubscribeToCutsceneEvent()
        {
            //CutsceneManager.OnCutsceneEnd -= LoadDialogue;
            //CutsceneManager.OnCutsceneEnd += LoadDialogue;
        }
        
        private void LoadGameUI()
        {
            if (sceneData.GameUIScene == null)
            {
                Debug.LogError("Game UI scene not assigned in SceneDataSO!");
                return;
            }
            
            if (_gameUIInstance.Scene.IsValid())
            {
                Addressables.UnloadSceneAsync(_gameUIInstance);
            }

            sceneData.GameUIScene.LoadSceneAsync(USM.LoadSceneMode.Additive).Completed += handle =>
            {
                _gameUIInstance = handle.Result;
            };
        }
        
        private void UnloadGameUI()
        {
            if (_gameUIInstance.Scene.IsValid())
            {
                Addressables.UnloadSceneAsync(_gameUIInstance);
            }
        }
        

        private void LoadDialogue()
        {
            sceneData.DialogueScene.LoadSceneAsync(USM.LoadSceneMode.Additive).Completed += dialogueHandle =>
            {
                _dialogueInstance = dialogueHandle.Result;
                OnLevelLoaded?.Invoke(_currentLevelIndex);
            };
        }
        
        private void UnloadDialogue()
        {
            if (_dialogueInstance.Scene.IsValid())
            {
                Addressables.UnloadSceneAsync(_dialogueInstance);
                LoadGameUI();
            }
        }

    }
}
