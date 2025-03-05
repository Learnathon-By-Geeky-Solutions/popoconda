using System;
using System.Threading.Tasks;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Studio23.SS2.SaveSystem.Core;
using Studio23.SS2.SaveSystem.Interfaces;

namespace Game
{
    public class LevelManager : MonoBehaviour, ISaveable
    {
        private static LevelManager Instance { get; set; }
        
        private static int _unlockedLevels;
        
        public static int UnlockedLevels => _unlockedLevels;
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
        
        private void Start()
        {
            InitializeLoadGame();
        }
        
        private static async Task InitializeLoadGame()
        {
            await SaveSystem.Instance.Load();
        }

        private void OnEnable()
        {
            Scene.SceneManager.OnLevelUnlock += UnlockLevel;
            Debug.Log("LevelManager Loaded Unlocked Levels: " + _unlockedLevels);
        }
        
        private void OnDisable()
        {
            Scene.SceneManager.OnLevelUnlock -= UnlockLevel;
        }

        
        private static void UnlockLevel(int levelIndex)
        {
            if(levelIndex >= _unlockedLevels)
            {
                _unlockedLevels = levelIndex;
                SaveSystem.Instance.Save(); // Save after unlocking a level
            }
        }
        
        public string GetUniqueID()
        {
            return "LevelSaveID";
        }
        
        public bool IsDirty { get; set; } = true;
        
        public UniTask<string> GetSerializedData()
        {
            GameState state = new GameState(_unlockedLevels);
            Debug.Log("Get Unlocked Levels: " + _unlockedLevels);
            return UniTask.FromResult(JsonUtility.ToJson(state));
        }
        
        public UniTask AssignSerializedData(string data)
        {
            if (!string.IsNullOrEmpty(data))
            {
                GameState state = JsonUtility.FromJson<GameState>(data);
                _unlockedLevels = state.unlockedLevels;
                Debug.Log("Assign Unlocked Levels: " + _unlockedLevels);
            }
            return UniTask.CompletedTask;
        }
        
        [Serializable]
        private class GameState
        {
            public int unlockedLevels;
            
            public GameState(int unlockedLevels)
            {
                this.unlockedLevels = unlockedLevels;
            }
        }
    }
    
}