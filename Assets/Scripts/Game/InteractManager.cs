using Interface;
using UnityEngine;

namespace Game
{
    public class InteractManager : MonoBehaviour
    {
        private IInteractable _currentInteractable;

        private void OnEnable()
        {
            InputManager.OnInteractPressed += Interact;
        }
        
        private void OnDisable()
        {
            InputManager.OnInteractPressed -= Interact;
        }

        private void OnTriggerEnter(Collider other)
        {
            _currentInteractable = other.gameObject.GetComponent<IInteractable>();
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.GetComponent<IInteractable>() != null)
            {
                _currentInteractable = null;
            }
        }
        
        private void Interact()
        {
            if (_currentInteractable != null)
            {
                _currentInteractable.Interact();
                Debug.Log("Interacted");
            }
        }
        
    }
}
