using System;
using UnityEngine;
using Characters;
using Studio23.SS2.SaveSystem.Core;
using Cysharp.Threading.Tasks;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using Studio23.SS2.SaveSystem.Interfaces;


namespace Game
{
    public class GameManager : MonoBehaviour, ISaveable
    {
        private static GameManager Instance { get; set; }
        
        private static Transform _playerTransform;
        
        private int _unlockedLevels;
        private string _language = LocalizationSettings.SelectedLocale.LocaleName;
        
                
        public delegate void GameResult();
        public static event GameResult WinEvent;
        public static event GameResult LoseEvent;
        
        public static event GameResult DisableHudEvent;
        

        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        private async void Start()
        {
            await SaveSystem.Instance.Initialize();
    
            // Load this specific instance of GameManager
            await SaveSystem.Instance.Load(this);

            Debug.Log("Loaded Unlocked Levels: " + _unlockedLevels);
            Debug.Log("Loaded Language: " + _language);
        
            // Apply the loaded language
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales
                .Find(locale => locale.LocaleName == _language);
        }

        private void OnEnable()
        {
            Enemy.OnBossDeath += Win;
            Player.OnPlayerDeath += Lose;
            Scene.SceneManager.OnLevelUnlock += UnlockLevel;
            LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
        }

        private void OnDisable()
        {
            Enemy.OnBossDeath -= Win;
            Player.OnPlayerDeath -= Lose;
            Scene.SceneManager.OnLevelUnlock -= UnlockLevel;
            LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
        }
        
        public static int GetUnlockedLevels()
        {
            return Instance?._unlockedLevels ?? 0;
        }

        
        private void UnlockLevel(int levelIndex)
        {
            _unlockedLevels = levelIndex;
            IsDirty = true; // Mark data as changed
            SaveSystem.Instance.Save(this, "GameManager"); // Save after unlocking a level
        }

        
        private void OnLocaleChanged(Locale locale)
        {
            _language = locale.LocaleName;
            IsDirty = true; // Mark data as changed
            SaveSystem.Instance.Save(this, "GameManager"); // Save after language change
        }
        
        public static void SetPlayerTransform(Transform playerTransform)
        {
            if (_playerTransform == null)
            {
                _playerTransform = playerTransform;
            }
        }
        
        public static Vector3 GetPlayerPosition()
        {
            if(_playerTransform == null)
            {
                return Vector3.zero;
            }
            return _playerTransform.position;
        }

        private void Win()
        {
            WinEvent?.Invoke();
            DisableHudEvent?.Invoke();
        }

        private void Lose()
        {
            LoseEvent?.Invoke();
            DisableHudEvent?.Invoke();
        }
        
        // unique identifier for the save system
        public string GetUniqueID()
        {
            return "GameSaveID";
        }
        
        public bool IsDirty { get; set; } = true;
        
        public UniTask<string> GetSerializedData()
        {
            GameState state = new GameState(_unlockedLevels, _language);
            Debug.Log("Get Unlocked Levels: " + _unlockedLevels);
            Debug.Log("Get Language: " + _language);
            return UniTask.FromResult(JsonUtility.ToJson(state));
        }
        
        public UniTask AssignSerializedData(string data)
        {
            if (!string.IsNullOrEmpty(data))
            {
                GameState state = JsonUtility.FromJson<GameState>(data);
                _unlockedLevels = state.unlockedLevels;
                _language = state.language;
                Debug.Log("Assign Unlocked Levels: " + _unlockedLevels);
                Debug.Log("Assign Language: " + _language);
            }
            return UniTask.CompletedTask;
        }
        
        [Serializable]
        private class GameState
        {
            public int unlockedLevels;
            public string language;
            
            public GameState(int unlockedLevels, string language)
            {
                this.unlockedLevels = unlockedLevels;
                this.language = language;
            }
        }

    }
}