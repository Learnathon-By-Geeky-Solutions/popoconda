using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UIElements;

namespace Game
{
    public class LocalizationManager : MonoBehaviour
    {
        public static LocalizationManager Instance { get; private set; }

        [Header("UI Documents")]
        public List<UIDocument> uiDocuments; // Assign all UIDocuments in Inspector

        private enum Language { English, Bangla }  // Enum for language selection
        [SerializeField] private Language selectedLanguage = Language.English;

        private string _currentLanguage;
        private readonly Dictionary<string, LocalizedString> _localizedStrings = new Dictionary<string, LocalizedString>();

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
            _currentLanguage = LocalizationSettings.SelectedLocale.Identifier.Code;
            Debug.Log("Current Language: " + _currentLanguage);

            LocalizationSettings.SelectedLocaleChanged += UpdateAllUI;
            UpdateAllUI(LocalizationSettings.SelectedLocale);
        }

        private void Update()
        {
            string newLangCode = selectedLanguage == Language.English ? "en" : "bn";
            if (_currentLanguage != newLangCode)
            {
                SetLanguage(newLangCode);
            }
        }

        public void SetLanguage(string languageCode)
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale(languageCode);
            _currentLanguage = languageCode;
            UpdateAllUI(LocalizationSettings.SelectedLocale);  // Force UI Update
        }

        private void UpdateAllUI(Locale locale)
        {
            bool isBangla = locale.Identifier.Code == "bn";

            foreach (var uiDocument in uiDocuments)
            {
                var root = uiDocument.rootVisualElement;
                var buttons = root.Query<Button>().ToList();

                foreach (var button in buttons)
                {
                    string buttonKey = button.name;

                    if (!_localizedStrings.ContainsKey(buttonKey))
                    {
                        LocalizedString localizedString = new LocalizedString("language", buttonKey);
                        localizedString.StringChanged += (value) => button.text = value;
                        _localizedStrings[buttonKey] = localizedString;
                    }

                    button.text = _localizedStrings[buttonKey].GetLocalizedString();

                    // Remove both font classes before applying the correct one
                    button.RemoveFromClassList("bangla-font");
                    button.RemoveFromClassList("english-font");

                    // Apply correct class
                    button.AddToClassList(isBangla ? "bangla-font" : "english-font");

                    // Force UI repaint
                    button.MarkDirtyRepaint();
                }
            }
        }
    }
}
