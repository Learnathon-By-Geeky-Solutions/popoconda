using Cutscene;
using Dialogue;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    public class SwitchInput : MonoBehaviour
    {
        private PlayerInput _playerInput;
        
        private void OnEnable()
        {
            _playerInput = GetComponent<PlayerInput>();
            Debug.Log("Current action map: " + _playerInput.currentActionMap.name);
            

            GameOver.UIEnableEvent += EnableUIMap;
            GameOver.UIDisableEvent += EnableGamplayMap;
            GameWin.UIEnableEvent += EnableUIMap;
            GameWin.UIDisableEvent += EnableGamplayMap;
            PauseMenu.UIEnableEvent += EnableUIMap;
            PauseMenu.UIDisableEvent += EnableGamplayMap;
            CutsceneManager.OnCutsceneStart += EnableCutsceneMap;
            CutsceneManager.OnCutsceneEnd += EnableGamplayMap;
            DialogueManager.OnDialogueStart += EnableCutsceneMap;
            DialogueManager.OnDialogueEnd += EnableGamplayMap;
        }
        
        private void OnDisable()
        {
            GameOver.UIEnableEvent -= EnableUIMap;
            GameOver.UIDisableEvent -= EnableGamplayMap;
            GameWin.UIEnableEvent -= EnableUIMap;
            GameWin.UIDisableEvent -= EnableGamplayMap;
            PauseMenu.UIEnableEvent -= EnableUIMap;
            PauseMenu.UIDisableEvent -= EnableGamplayMap;
            CutsceneManager.OnCutsceneStart -= EnableCutsceneMap;
            CutsceneManager.OnCutsceneEnd -= EnableGamplayMap;
            DialogueManager.OnDialogueStart -= EnableCutsceneMap;
            DialogueManager.OnDialogueEnd -= EnableGamplayMap;
        }
        
        private void EnableUIMap()
        {
            _playerInput.SwitchCurrentActionMap("UI");
            Debug.Log("Current action map: " + _playerInput.currentActionMap.name);
        }
        
        private void EnableGamplayMap()
        {
            _playerInput.SwitchCurrentActionMap("GamePlay");
            Debug.Log("Current action map: " + _playerInput.currentActionMap.name);
        }
        
        private void EnableCutsceneMap()
        {
            _playerInput.SwitchCurrentActionMap("Cutscene");
            Debug.Log("Current action map: " + _playerInput.currentActionMap.name);
        }
    }
}