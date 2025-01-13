using UnityEngine;
using UnityEngine.UIElements;
using Characters;

namespace UI
{
    public class HudHandler : MonoBehaviour
    {
        [SerializeField] private PlayerController playerController;
        [SerializeField] private Boss1Script boss1Script;
        [SerializeField] private UIDocument hudDocument;
        private Label _playerHealthLabel;
        private Label _enemyHealthLabel;
        
        void Start()
        {
            
            VisualElement root = hudDocument.rootVisualElement;
            
            // get label elements
            _playerHealthLabel = root.Q<Label>("player-health");
            _enemyHealthLabel = root.Q<Label>("enemy-health");
        }

        public void UpdateHealth()
        {
            if (playerController&& _playerHealthLabel != null)
            {
                // Update the player health background color width
                _playerHealthLabel.style.width = new Length(
                    (playerController.CurrentHealth / playerController.MaxHealth) * 100, 
                    LengthUnit.Percent);
            }

            if (boss1Script&& _enemyHealthLabel != null)
            {
                // Update the enemy health background color width
                _enemyHealthLabel.style.width = new Length(
                    (boss1Script.CurrentHealth / boss1Script.MaxHealth) * 100,  
                    LengthUnit.Percent);
            }
            
        }
    }
    
        
}

