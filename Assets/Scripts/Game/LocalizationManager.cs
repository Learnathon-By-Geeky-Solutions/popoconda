using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Studio23.SS2.SaveSystem.Core;
using Studio23.SS2.SaveSystem.Interfaces;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Game
{
    public class LocalizationManager : MonoBehaviour, ISaveable
    {
        private static LocalizationManager Instance { get; set; }
        
        private int _languageIndex; 
        
        private void Awake()
        {
            _languageIndex = LocalizationSettings.AvailableLocales.Locales.IndexOf(LocalizationSettings.SelectedLocale);
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
            await SaveSystem.Instance.Load();
        }

        private void OnEnable()
        {
            LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
        }
        
        private void OnDisable()
        {
            LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
        }
        
        private void OnLocaleChanged(Locale locale)
        {
            _languageIndex = LocalizationSettings.AvailableLocales.Locales.IndexOf(locale);
            SaveSystem.Instance.Save(); // Save after language change
        }
        
        public string GetUniqueID()
        {
            return "LocaleSaveID";
        }
        
        public bool IsDirty { get; set; } = true;
        
        public UniTask<string> GetSerializedData()
        {
            GameState state = new GameState(_languageIndex);
            return UniTask.FromResult(JsonUtility.ToJson(state));
        }
        
        public UniTask AssignSerializedData(string data)
        {
            if (!string.IsNullOrEmpty(data))
            {
                GameState state = JsonUtility.FromJson<GameState>(data);
                _languageIndex = state.languageIndex;
                Debug.Log("Loaded Language: " + _languageIndex);
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_languageIndex];
            }
            return UniTask.CompletedTask;
        }
        
        [Serializable]
        private class GameState
        {
            public int languageIndex;
            
            public GameState(int language)
            {
                this.languageIndex = language;
            }
        }
    }
    
}
