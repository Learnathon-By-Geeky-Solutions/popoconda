using Cysharp.Threading.Tasks;
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
        private SceneInstance _optionMenuInstance;
        private SceneInstance _levelSelectMenuInstance;
        private SceneInstance _levelInstance;
        private SceneInstance _playerInstance;
        private SceneInstance _gameUiInstance;
        
        private int _unlockedLevels;
        private int _currentLevelIndex = -1;
        
        public delegate void StateEventWithInt(int value);
        public static event StateEventWithInt OnLevelUnlock;

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
        
        private async void Start()
        {
            await UniTask.DelayFrame(1); // Ensure SaveSystem has loaded first
            _unlockedLevels = GameManager.GetUnlockedLevels();
            Debug.Log("SceneManager Loaded Unlocked Levels: " + _unlockedLevels);
        }

        private void OnEnable()
        {
            MainMenu.PlayButtonClicked += LoadLevelSelectScene;
            MainMenu.OptionButtonClicked += LoadOptionMenu;
            OptionMenu.BackButtonClicked += LoadMainMenu;
            LevelSelectMenu.level1Event += LoadLevel1;
            LevelSelectMenu.level2Event += LoadLevel2;
            LevelSelectMenu.level3Event += LoadLevel3;
            LevelSelectMenu.backEvent += LoadMainMenu;
            GameOver.RetryEvent += LoadCurrentLevel;
            GameOver.MenuEvent += LoadMainMenu;
            GameWin.MenuEvent += LoadMainMenu;
            GameWin.NextLevelEvent += LoadNextLevel;
            PauseMenu.RestartEvent += LoadCurrentLevel;
            PauseMenu.MainMenuEvent += LoadMainMenu;
            MainMenuLoader.MainMenuEvent += LoadMainMenu;
        }
        
        private void OnDisable()
        {
            MainMenu.PlayButtonClicked -= LoadLevelSelectScene;
            MainMenu.OptionButtonClicked -= LoadOptionMenu;
            OptionMenu.BackButtonClicked -= LoadMainMenu;
            LevelSelectMenu.level1Event -= LoadLevel1;
            LevelSelectMenu.level2Event -= LoadLevel2;
            LevelSelectMenu.level3Event -= LoadLevel3;
            LevelSelectMenu.backEvent -= LoadMainMenu;
            GameOver.RetryEvent -= LoadCurrentLevel;
            GameOver.MenuEvent -= LoadMainMenu;
            GameWin.MenuEvent -= LoadMainMenu;
            GameWin.NextLevelEvent -= LoadNextLevel;
            PauseMenu.RestartEvent -= LoadCurrentLevel;
            PauseMenu.MainMenuEvent -= LoadMainMenu;
            MainMenuLoader.MainMenuEvent -= LoadMainMenu;
        }
        
        private void LoadMainMenu()
        {
            if (sceneData.MainMenuScene == null)
            {
                Debug.LogError("Main Menu scene not assigned in SceneDataSO!");
                return;
            }
            
            if(_levelSelectMenuInstance.Scene.isLoaded)
            {
                Addressables.UnloadSceneAsync(_levelSelectMenuInstance);
            }
            if(_optionMenuInstance.Scene.isLoaded)
            {
                Addressables.UnloadSceneAsync(_optionMenuInstance);
            }
            if (_levelInstance.Scene.isLoaded)
            {
                Addressables.UnloadSceneAsync(_levelInstance);
            }
            if (_playerInstance.Scene.isLoaded)
            {
                Addressables.UnloadSceneAsync(_playerInstance);
            }
            if (_gameUiInstance.Scene.isLoaded)
            {
                Addressables.UnloadSceneAsync(_gameUiInstance);
            }

            Addressables.LoadSceneAsync(sceneData.MainMenuScene, USM.LoadSceneMode.Additive)
                .Completed += handle => _mainMenuInstance = handle.Result;
        }
        
        private void LoadOptionMenu()
        {
            if (sceneData.OptionMenuScene == null)
            {
                Debug.LogError("Option Menu scene not assigned in SceneDataSO!");
                return;
            }
            
            if (_mainMenuInstance.Scene.isLoaded)
            {
                Addressables.UnloadSceneAsync(_mainMenuInstance);
            }
            Addressables.LoadSceneAsync(sceneData.OptionMenuScene, USM.LoadSceneMode.Additive)
                .Completed += handle => _optionMenuInstance = handle.Result;
        }
        
        private void LoadLevelSelectScene()
        {
            if (sceneData.LevelSelectScene == null)
            {
                Debug.LogError("Level Select scene not assigned in SceneDataSO!");
                return;
            }
            
            if (_mainMenuInstance.Scene.isLoaded)
            {
                Addressables.UnloadSceneAsync(_mainMenuInstance);
            }
            Addressables.LoadSceneAsync(sceneData.LevelSelectScene, USM.LoadSceneMode.Additive)
                .Completed += handle => _levelSelectMenuInstance = handle.Result;
        }
        
        private void LoadLevel1()
        {
            if (sceneData.Levels.Count == 0)
            {
                Debug.LogError("No levels found in SceneDataSO!");
                return;
            }
            
            Addressables.UnloadSceneAsync(_levelSelectMenuInstance);
            _currentLevelIndex = 0;
            LoadLevel(sceneData.Levels[_currentLevelIndex]);
            LoadPlayer();
            LoadGameUI();
        }
        
        private void LoadLevel2()
        {
            if (sceneData.Levels.Count == 0)
            {
                Debug.LogError("No levels found in SceneDataSO!");
                return;
            }
            if(_unlockedLevels < 1)
            {
                Debug.Log("Unlocked levels: " + _unlockedLevels);
                Debug.Log("Level 2 is locked!");
                return;
            }
            
            Addressables.UnloadSceneAsync(_levelSelectMenuInstance);
            _currentLevelIndex = 1;
            LoadLevel(sceneData.Levels[_currentLevelIndex]);
            LoadPlayer();
            LoadGameUI();
        }
        
        private void LoadLevel3()
        {
            if (sceneData.Levels.Count == 0)
            {
                Debug.LogError("No levels found in SceneDataSO!");
                return;
            }
            
            if(_unlockedLevels < 2)
            {
                Debug.Log("Level 3 is locked!");
                Debug.Log("Unlocked levels: " + _unlockedLevels);
                return;
            }
            
            Addressables.UnloadSceneAsync(_levelSelectMenuInstance);
            _currentLevelIndex = 2;
            LoadLevel(sceneData.Levels[_currentLevelIndex]);
            LoadPlayer();
            LoadGameUI();
        }

        private void LoadNextLevel()
        {
            if (sceneData.Levels.Count == 0)
            {
                Debug.LogError("No levels found in SceneDataSO!");
                return;
            }
            
            if(_unlockedLevels  <= _currentLevelIndex)
            {
                _unlockedLevels++;
                OnLevelUnlock?.Invoke(_unlockedLevels);
            }
            Debug.Log("Unlocked levels: " + _unlockedLevels);

            _currentLevelIndex++;
            if (_currentLevelIndex >= sceneData.Levels.Count)
            {
                Debug.Log("No more levels to load. Loading Main Menu.");
                LoadMainMenu();
                return;
            }
            
            Addressables.UnloadSceneAsync(_levelInstance);
            LoadLevel(sceneData.Levels[_currentLevelIndex]);
            Addressables.UnloadSceneAsync(_playerInstance).Completed += handle =>
            {
                _playerInstance = handle.Result;
                LoadPlayer();

            };
            Addressables.UnloadSceneAsync(_gameUiInstance).Completed += handle =>
            {
                _gameUiInstance = handle.Result;
                LoadGameUI();
            };
        }

        private void LoadCurrentLevel()
        {
            if (sceneData.Levels.Count == 0)
            {
                Debug.LogError("No levels found in SceneDataSO!");
                return;
            }

            // unload the current level
            Addressables.UnloadSceneAsync(_levelInstance).Completed += handle =>
            {
                _levelInstance = handle.Result;
                LoadLevel(sceneData.Levels[_currentLevelIndex]);
            };
            Addressables.UnloadSceneAsync(_playerInstance).Completed += handle =>
            {
                _playerInstance = handle.Result;
                LoadPlayer();

            };
            Addressables.UnloadSceneAsync(_gameUiInstance).Completed += handle =>
            {
                _gameUiInstance = handle.Result;
                LoadGameUI();
            };
        }

        private void LoadLevel(AssetReference levelReference)
        {
            Debug.Log($"Loading Level: {levelReference.RuntimeKey}");
            Debug.Log("Corrent level index: " + _currentLevelIndex);
            
            levelReference.LoadSceneAsync(USM.LoadSceneMode.Additive).Completed += handle =>
            {
                _levelInstance = handle.Result;
            };
        }

        private void LoadPlayer()
        {
            sceneData.PlayerScene.LoadSceneAsync(USM.LoadSceneMode.Additive).Completed += pHandle =>
            {
                _playerInstance = pHandle.Result;
            };
        }
        
        private void LoadGameUI()
        {
            sceneData.GameUIScene.LoadSceneAsync(USM.LoadSceneMode.Additive).Completed += uiHandle =>
            {
                _gameUiInstance = uiHandle.Result;
            };
        }

    }
}
