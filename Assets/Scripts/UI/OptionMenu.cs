using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace UI
{
    public class OptionMenu : MonoBehaviour
    {
        [SerializeField] private UIDocument optionMenuDocument;
        private DropdownField _languageToggle;
        
        private Button _backButton;
        
        public delegate void StatEvent();
        public static event StatEvent BackButtonClicked;
        
        private void OnEnable()
        {
            VisualElement root = optionMenuDocument.rootVisualElement;
            _languageToggle = root.Q<DropdownField>("language-toggle");
            _backButton = root.Q<Button>("back-button");

            var choices = _languageToggle.choices;

            if (LocalizationSettings.SelectedLocale.LocaleName == "Bangla (bn)")
            {
                _languageToggle.value = choices[0];
            }
            else
            {
                _languageToggle.value = choices[1];
            }

            _languageToggle.RegisterValueChangedCallback(v=>
            {
                
                if (v.newValue == choices[0])
                {
                   SetLocale("bn");
                }
                else if (v.newValue == choices[1])
                {
                    SetLocale("en");
                }
            });
            _backButton.clicked += HandleBackButtonClicked;
        }
        
        private void OnDisable()
        {
            _backButton.clicked -= HandleBackButtonClicked;
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