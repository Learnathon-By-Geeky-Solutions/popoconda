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

        private static SceneInstance mainMenuInstance;
        private static SceneInstance levelInstance;
        private static SceneInstance playerInstance;
        private static SceneInstance gameUiInstance;

        private int currentLevelIndex = -1;

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
            MainMenu.PlayButtonClicked += LoadNextScene;
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
            MainMenu.PlayButtonClicked -= LoadNextScene;
            GameOver.RetryEvent -= LoadCurrentLevel;
            GameOver.MenuEvent -= LoadMainMenu;
            GameWin.MenuEvent -= LoadMainMenu;
            GameWin.NextLevelEvent -= LoadNextLevel;
            PauseMenu.RestartEvent -= LoadCurrentLevel;
            PauseMenu.MainMenuEvent -= LoadMainMenu;
            MainMenuLoader.MainMenuEvent -= LoadMainMenu;
        }

        private void LoadNextScene()
        {
            if (sceneData.levels.Count == 0)
            {
                Debug.LogError("No levels found in SceneDataSO!");
                return;
            }

            currentLevelIndex = 0;
            LoadLevel(sceneData.levels[currentLevelIndex]);
        }

        private void LoadNextLevel()
        {
            if (sceneData.levels.Count == 0)
            {
                Debug.LogError("No levels found in SceneDataSO!");
                return;
            }

            currentLevelIndex++;
            if (currentLevelIndex >= sceneData.levels.Count)
            {
                Debug.Log("No more levels to load. Loading Main Menu.");
                LoadMainMenu();
                return;
            }

            LoadLevel(sceneData.levels[currentLevelIndex]);
        }

        private void LoadCurrentLevel()
        {
            if (sceneData.levels.Count == 0)
            {
                Debug.LogError("No levels found in SceneDataSO!");
                return;
            }

            // unload the current level
            Addressables.UnloadSceneAsync(levelInstance).Completed += handle =>
            {
                Addressables.UnloadSceneAsync(gameUiInstance);
                LoadLevel(sceneData.levels[currentLevelIndex]);
            };
            LoadLevel(sceneData.levels[currentLevelIndex]);
        }

        private void LoadMainMenu()
        {
            if (sceneData.mainMenuScene == null)
            {
                Debug.LogError("Main Menu scene not assigned in SceneDataSO!");
                return;
            }

            Addressables.LoadSceneAsync(sceneData.mainMenuScene, USM.LoadSceneMode.Single)
                .Completed += handle => mainMenuInstance = handle.Result;
        }

        private void LoadLevel(AssetReference levelReference)
        {
            Debug.Log($"Loading Level: {levelReference.RuntimeKey}");
            Debug.Log("Corrent level index: " + currentLevelIndex);

            // Step 1: Load the level scene non-additively (MainMenu will be unloaded automatically)
            levelReference.LoadSceneAsync(USM.LoadSceneMode.Single).Completed += handle =>
            {
                levelInstance = handle.Result;
                
                LoadPlayer();
                LoadGameUI();
                
            };
        }

        private void LoadPlayer()
        {
            sceneData.playerScene.LoadSceneAsync(USM.LoadSceneMode.Additive).Completed += pHandle =>
            {
                playerInstance = pHandle.Result;
            };
        }
        
        private void LoadGameUI()
        {
            sceneData.gameUIScene.LoadSceneAsync(USM.LoadSceneMode.Additive).Completed += uiHandle =>
            {
                gameUiInstance = uiHandle.Result;
            };
        }

    }
}
