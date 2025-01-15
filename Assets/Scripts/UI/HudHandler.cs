using UnityEngine;
using UnityEngine.UIElements;
using Characters;
using Combat;

namespace UI
{
    public class HudHandler : MonoBehaviour
    {
        [SerializeField] private PlayerController playerController;
        [SerializeField] private Boss1Script boss1Script;
        [SerializeField] private UIDocument hudDocument;
        private Label _playerHealthLabel;
        private Label _enemyHealthLabel;
        private Label _jetpackFuelLabel;
        private Label _ammoLabel;
        
        void Start()
        {
            
            VisualElement root = hudDocument.rootVisualElement;
            
            // get label elements
            _playerHealthLabel = root.Q<Label>("player-health");
            _enemyHealthLabel = root.Q<Label>("enemy-health");
            _jetpackFuelLabel = root.Q<Label>("jetpack-fuel");
            _ammoLabel = root.Q<Label>("ammo-label");
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
        
        public void UpdateJetpackFuel(float jetpackFuel, float jetpackFuelMax)
        {
            if (playerController && _jetpackFuelLabel != null)
            {
                // Update the jetpack fuel background color width
                _jetpackFuelLabel.style.height = new Length(
                    (jetpackFuel / jetpackFuelMax) * 100, 
                    LengthUnit.Percent);
            }
        }
        public void UpdateAmmo(int bulletsLeft)
        {
            if (playerController && _ammoLabel != null)
            {
                // Update the ammo label
                _ammoLabel.text = bulletsLeft.ToString();
            }
        }
        
    }
    
        
}

