using UnityEngine;
using UnityEngine.InputSystem.HID;
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
        private Button _backButton;
        
        private void OnEnable()
        {
            VisualElement root = optionMenuDocument.rootVisualElement;
            
            _englishButton = root.Q<Button>("english-text-button");
            _banglaButton = root.Q<Button>("bangla-text-button");
            _backButton = root.Q<Button>("back-button");
            
            MainMenu.OptionButtonClicked += () => optionMenuDocument.sortingOrder = 1;
            
            optionMenuDocument.sortingOrder = -1;
            
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

        private void HandleBackButtonClicked()
        {
            Debug.Log("Back button clicked");
            optionMenuDocument.sortingOrder = -1;
            
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
            
        }
    }
}