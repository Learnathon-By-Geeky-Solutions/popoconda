using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace UI
{
    public class OptionMenu : MonoBehaviour
    {
        [SerializeField] private UIDocument optionMenuDocument;
        private Button _englishButton;
        private Button _banglaButton;
        
        
        private void OnEnable()
        {
            VisualElement root = optionMenuDocument.rootVisualElement;
            
            _englishButton = root.Q<Button>("english-text-button");
            _banglaButton = root.Q<Button>("bangla-text-button");
            
            MainMenu.OptionButtonClicked += () => optionMenuDocument.sortingOrder = 1;
            
            optionMenuDocument.sortingOrder = -1;
            
            _englishButton.clicked += HandleEnglishButtonClicked;
            _banglaButton.clicked += HandleBanglaButtonClicked;
        }
        
        private void OnDisable()
        {
            _englishButton.clicked -= HandleEnglishButtonClicked;
            _banglaButton.clicked -= HandleBanglaButtonClicked;
        }
        
        private void HandleEnglishButtonClicked()
        {
            Debug.Log("English button clicked");
            SetLocale("en");  // Change locale to English
        }
        
        private void HandleBanglaButtonClicked()
        {
            Debug.Log("Bangla button clicked");
            SetLocale("bn");  // Change locale to Bangla
        }

        private void SetLocale(string localeCode)
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
            
            optionMenuDocument.sortingOrder = -1;
        }
    }
}