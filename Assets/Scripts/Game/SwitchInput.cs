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
            

            GameOver.UIEnableEvent += HandleUiEnable;
            GameOver.UIDisableEvent += HandleUiDisable;
            GameWin.UIEnableEvent += HandleUiEnable;
            GameWin.UIDisableEvent += HandleUiDisable;
            PauseMenu.UIEnableEvent += HandleUiEnable;
            PauseMenu.UIDisableEvent += HandleUiDisable;
        }
        
        private void OnDisable()
        {
            GameOver.UIEnableEvent -= HandleUiEnable;
            GameOver.UIDisableEvent -= HandleUiDisable;
            GameWin.UIEnableEvent -= HandleUiEnable;
            GameWin.UIDisableEvent -= HandleUiDisable;
            PauseMenu.UIEnableEvent -= HandleUiEnable;
            PauseMenu.UIDisableEvent -= HandleUiDisable;
        }
        
        private void HandleUiEnable()
        {
            _playerInput.SwitchCurrentActionMap("UI");
            Debug.Log("Current action map: " + _playerInput.currentActionMap.name);
        }
        
        private void HandleUiDisable()
        {
            _playerInput.SwitchCurrentActionMap("GamePlay");
            Debug.Log("Current action map: " + _playerInput.currentActionMap.name);
        }
    }
}