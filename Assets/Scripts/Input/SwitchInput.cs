using Cutscene;
using Dialogue;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public class SwitchInput : MonoBehaviour
    {
        private PlayerInput _playerInput;
        
        private void OnEnable()
        {
            _playerInput = GetComponent<PlayerInput>();
            Debug.Log("Current action map: " + _playerInput.currentActionMap.name);

            GameOver.UIEnableEvent += EnableUIMap;
            GameOver.UIDisableEvent += EnableGameplayMap;
            GameWin.UIEnableEvent += EnableUIMap;
            GameWin.UIDisableEvent += EnableGameplayMap;
            PauseMenu.UIEnableEvent += EnableUIMap;
            PauseMenu.UIDisableEvent += EnableGameplayMap;
            CutsceneManager.OnCutsceneStart += EnableCutsceneMap;
            CutsceneManager.OnCutsceneEnd += EnableGameplayMap;
            DialogueManager.OnDialogueStart += EnableCutsceneMap;
            DialogueManager.OnDialogueEnd += EnableGameplayMap;
        }
        
        private void OnDisable()
        {
            GameOver.UIEnableEvent -= EnableUIMap;
            GameOver.UIDisableEvent -= EnableGameplayMap;
            GameWin.UIEnableEvent -= EnableUIMap;
            GameWin.UIDisableEvent -= EnableGameplayMap;
            PauseMenu.UIEnableEvent -= EnableUIMap;
            PauseMenu.UIDisableEvent -= EnableGameplayMap;
            CutsceneManager.OnCutsceneStart -= EnableCutsceneMap;
            CutsceneManager.OnCutsceneEnd -= EnableGameplayMap;
            DialogueManager.OnDialogueStart -= EnableCutsceneMap;
            DialogueManager.OnDialogueEnd -= EnableGameplayMap;
        }
        
        private void EnableUIMap()
        {
            _playerInput.SwitchCurrentActionMap("UI");
        }
        
        private void EnableGameplayMap()
        {
            _playerInput.SwitchCurrentActionMap("GamePlay");
        }
        
        private void EnableCutsceneMap()
        {
            _playerInput.SwitchCurrentActionMap("Cutscene");
        }
    }
}