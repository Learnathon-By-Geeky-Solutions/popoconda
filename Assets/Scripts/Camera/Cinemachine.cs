using Dialogue;
using UnityEngine;
using Unity.Cinemachine;

namespace Camera
{
    public class Cinemachine : MonoBehaviour
    {
        private CinemachineCamera _camera;
        private void Awake()
        {
            _camera= GetComponent<CinemachineCamera>();
            
            //_camera.enabled = false;
            
            DialogueManager.OnDialogueEnd += EnableTarget;
        }
        
        private void OnDisable()
        {
            DialogueManager.OnDialogueEnd -= EnableTarget;
        }
        
        private void EnableTarget()
        {
            _camera.enabled = true;
        }
    }
}
