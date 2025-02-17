using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace UI
{
    public class OptionMenu : MonoBehaviour
    {
        [SerializeField] private UIDocument optionMenuDocument;
        private ToggleButtonGroup _languageToggle;
        private Button _englishButton;
        private Button _banglaButton;
        private Button _backButton;
        
        public delegate void StatEvent();
        public static event StatEvent BackButtonClicked;
        
        private void OnEnable()
        {
            VisualElement root = optionMenuDocument.rootVisualElement;
            
            _languageToggle = root.Q<ToggleButtonGroup>("language-toggle");
            _englishButton = root.Q<Button>("english-text-button");
            _banglaButton = root.Q<Button>("bangla-text-button");
            _backButton = root.Q<Button>("back-button");

            if (LocalizationSettings.SelectedLocale.LocaleName == "Bangla (bn)")
            {
                var state = _languageToggle.value;
                state[1] = true;
                state[0] = false; 
                _languageToggle.value = state;
            }
            
            _englishButton.clicked += HandleEnglishButtonClicked;
            _banglaButton.clicked += HandleBanglaButtonClicked;
            _backButton.clicked += HandleBackButtonClicked;
        }
        
        private void OnDisable()
        {
            _englishButton.clicked -= HandleEnglishButtonClicked;
            _banglaButton.clicked -= HandleBanglaButtonClicked;
            _backButton.clicked -= HandleBackButtonClicked;
        }
        
        private static void HandleEnglishButtonClicked()
        {
            Debug.Log("English button clicked");
            SetLocale("en");  // Change locale to English
        }
        
        private static void HandleBanglaButtonClicked()
        {
            Debug.Log("Bangla button clicked");
            SetLocale("bn");  // Change locale to Bangla
        }

        private static void HandleBackButtonClicked()
        {
            Debug.Log("Back button clicked");
            BackButtonClicked?.Invoke();
        }
        
        private static void SetLocale(string localeCode)
        {
            Locale newLocale = LocalizationSettings.AvailableLocales.GetLocale(localeCode);
            if (newLocale != null)
            {
                LocalizationSettings.SelectedLocale = newLocale;
                Debug.Log("Locale changed to: " + localeCode);
            }
            else
            {
                Debug.LogWarning("Locale not found: " + localeCode);
            }
            
        }
    }
}