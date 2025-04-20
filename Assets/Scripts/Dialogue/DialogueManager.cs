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

        private VisualElement _dialogueBoxLeft, _dialogueBoxRight;
        private Label _characterNameLeft, _characterNameRight, _dialogueTextLeft, _dialogueTextRight;

        [SerializeField] private DialogueList dialogueList;
        private Dialogue _currentDialogue;
        private int _currentDialogueIndex;

        public delegate void StatEvent();
        public static event StatEvent OnDialogueStart, OnDialogueEnd;

        private void Awake()
        {
            OnDialogueStart?.Invoke();
            var root = dialogueDocument.rootVisualElement;

            _dialogueBoxLeft = root.Q<VisualElement>("DialogueBox-1");
            _dialogueBoxRight = root.Q<VisualElement>("DialogueBox-2");
            _characterNameLeft = root.Q<Label>("Character-Name-1");
            _characterNameRight = root.Q<Label>("Character-Name-2");
            _dialogueTextLeft = root.Q<Label>("Dialogue-body-1");
            _dialogueTextRight = root.Q<Label>("Dialogue-body-2");
        }

        private void OnEnable()
        {
            SceneManager.OnLevelLoaded += LoadDialogue;
            InputManager.OnNextPressed += ShowNextDialogue;
        }

        private void OnDisable()
        {
            SceneManager.OnLevelLoaded -= LoadDialogue;
            InputManager.OnNextPressed -= ShowNextDialogue;
        }

        private void LoadDialogue(int levelIndex)
        {
            _currentDialogueIndex = 0;

            dialogueList.Levels[levelIndex].LoadAssetAsync<Dialogue>().Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    _currentDialogue = handle.Result;
                    ShowDialogue();
                    Addressables.Release(dialogueList.Levels[levelIndex].Asset);
                }
                else
                {
                    Debug.LogError("Failed to load dialogue!");
                }
            };
        }

        private void ShowNextDialogue()
        {
            if (_currentDialogueIndex < _currentDialogue.Dialogues.Length - 1)
            {
                _currentDialogueIndex++;
                ShowDialogue();
            }
            else
            {
                OnDialogueEnd?.Invoke();
            }
        }

        private void ShowDialogue()
        {
            var dialogueData = _currentDialogue.Dialogues[_currentDialogueIndex];
            int speakerID = dialogueData.speakerID;
            string speakerName = _currentDialogue.CharacterName[speakerID];
            Color speakerColor = _currentDialogue.CharacterColor[speakerID];
            string dialogueText = dialogueData.dialogueText.GetLocalizedString();

            // Toggle visibility based on speaker
            bool isRightSpeaker = speakerID == 1;
            _dialogueBoxLeft.style.display = isRightSpeaker ? DisplayStyle.None : DisplayStyle.Flex;
            _dialogueBoxRight.style.display = isRightSpeaker ? DisplayStyle.Flex : DisplayStyle.None;

            // Assign values to appropriate dialogue box
            if (isRightSpeaker)
            {
                _characterNameRight.text = speakerName;
                _characterNameRight.style.color = speakerColor;
                DisplayDialogueGradually(_dialogueTextRight, dialogueText);
            }
            else
            {
                _characterNameLeft.text = speakerName;
                _characterNameLeft.style.color = speakerColor;
                DisplayDialogueGradually(_dialogueTextLeft, dialogueText);
            }
        }

        private void DisplayDialogueGradually(Label dialogueLabel, string fullText)
        {
            dialogueLabel.text = "";
            string[] words = fullText.Split(' ');
            int wordIndex = 0;

            dialogueDocument.rootVisualElement.schedule.Execute(() =>
            {
                if (wordIndex < words.Length)
                {
                    dialogueLabel.text += (wordIndex == 0 ? "" : " ") + words[wordIndex];
                    wordIndex++;
                }
            }).Every(250); // Adjust delay for effect speed
        }
    }
}
