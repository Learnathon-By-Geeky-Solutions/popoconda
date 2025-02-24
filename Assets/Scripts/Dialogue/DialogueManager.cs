using UnityEngine;
using UnityEngine.UIElements;
using Scene;
using Game;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        [SerializeField] private UIDocument dialogueDocument;
        private Label _characterName;
        private Label _dialogueText;
        
        
        public DialogueList dialogueList;
        private Dialogue _currentDialogue;
        
        private string _currentSpeaker;
        private string _currentDialogueText;
        
        private int _currentDialogueIndex;
        
        public delegate void StatEvent();
        
        public static event StatEvent OnDialogueStart;
        public static event StatEvent OnDialogueEnd;
        private void Awake()
        {
            OnDialogueStart?.Invoke();
            
            VisualElement root = dialogueDocument.rootVisualElement;
            
            _characterName = root.Q<Label>("Caracter-Name");
            _dialogueText = root.Q<Label>("Dialogue-body");
        }

        private void OnEnable()
        {
            SceneManager.OnLevelLoaded += LoadDialogue;
            InputManager.OnNextPressed += ShowNextDialogue;
            
            Debug.Log("Dialogue Manager Enabled");
        }

        private void OnDestroy()
        {
            SceneManager.OnLevelLoaded -= LoadDialogue;
            InputManager.OnNextPressed -= ShowNextDialogue;
        }

        private void LoadDialogue(int levelIndex)
        {
            // Reset dialogue index to start from the first dialogue
            _currentDialogueIndex = 0;

            // Load the asset asynchronously
            dialogueList.Levels[levelIndex].LoadAssetAsync<Dialogue>().Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    _currentDialogue = handle.Result; // Assign the loaded asset

                    UpdateDialogueText();
                    Addressables.Release(dialogueList.Levels[levelIndex].Asset); // Release the asset after loading
                }
                else
                {
                    Debug.LogError("Failed to load dialogue!");
                }
            };
        }
        
        private void ShowNextDialogue()
        {
            if (_currentDialogueIndex < _currentDialogue.dialogues.Length - 1)
            {
                _currentDialogueIndex++;
                _currentSpeaker = _currentDialogue.characterName[_currentDialogue.dialogues[_currentDialogueIndex].speakerID];
                _currentDialogueText = _currentDialogue.dialogues[_currentDialogueIndex].dialogueText.GetLocalizedString();
                
                _characterName.text = _currentSpeaker;
                _characterName.style.color = _currentDialogue.characterColor[_currentDialogue.dialogues[_currentDialogueIndex].speakerID];
                
                DisplayDialogueGradually(_currentDialogueText);
            }
            else
            {
                OnDialogueEnd?.Invoke();
            }
        }
        
        private void UpdateDialogueText()
        {
            // Ensure there are dialogues before trying to access them
            if (_currentDialogue.dialogues.Length > 0)
            {
                _currentSpeaker = _currentDialogue.characterName[_currentDialogue.dialogues[0].speakerID];

                // Asynchronous localization
                _currentDialogue.dialogues[0].dialogueText.StringChanged += localizedText =>
                {
                    _currentDialogueText = localizedText;
                    UpdateDialogueUI();
                };

                Debug.Log("Current Speaker: " + _currentSpeaker);
            }
            else
            {
                Debug.LogWarning("No dialogues found in the loaded dialogue asset!");
            }
        }
        
        // Separate method to update the UI
        private void UpdateDialogueUI()
        {
            _characterName.text = _currentSpeaker;
            _characterName.style.color = _currentDialogue.characterColor[_currentDialogue.dialogues[_currentDialogueIndex].speakerID];

            // Start displaying dialogue word by word
            DisplayDialogueGradually(_currentDialogueText);
        }
        
        private void DisplayDialogueGradually(string fullText)
        {
            _dialogueText.text = ""; // Clear text before starting

            string[] words = fullText.Split(' '); // Split dialogue into words
            int wordIndex = 0;

            // Schedule a repeating task to add words one by one
            dialogueDocument.rootVisualElement.schedule.Execute(() =>
            {
                if (wordIndex < words.Length)
                {
                    _dialogueText.text += (wordIndex == 0 ? "" : " ") + words[wordIndex];
                    wordIndex++;
                }
            }).Every(250); // Adjust delay (in milliseconds) for word appearance speed
        }
    }
    
}