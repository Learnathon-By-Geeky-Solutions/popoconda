using UnityEngine;
using Characters;
using Combat;
using Game;
using UnityEngine.UIElements;

namespace UI
{
    public class HudHandler : MonoBehaviour
    {
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
            
            PlayerController.OnPlayerHealthChange += UpdatePlayerHealth;
            PlayerController.OnJetpackFuelChange += UpdateJetpackFuel;
            Enemy.OnEnemyHealthChange += UpdateEnemyHealth;
            ShootingController.OnBulletCountChange += UpdateAmmo;
            GameManager.DisableHudEvent += DisableHud;
        }

        private void OnDestroy()
        {
            PlayerController.OnPlayerHealthChange -= UpdatePlayerHealth;
            PlayerController.OnJetpackFuelChange -= UpdateJetpackFuel;
            Enemy.OnEnemyHealthChange -= UpdateEnemyHealth;
            ShootingController.OnBulletCountChange -= UpdateAmmo;
            GameManager.DisableHudEvent -= DisableHud;
        }

        private void UpdatePlayerHealth(float currentHealthPercentage)
        {
            if (_playerHealthLabel != null)
            {
                _playerHealthLabel.style.width = new Length(currentHealthPercentage * 100, LengthUnit.Percent);
            }
        }

        private void UpdateEnemyHealth(float currentHealthPercentage)
        {
            if (_enemyHealthLabel != null)
            {
                _enemyHealthLabel.style.width = new Length(currentHealthPercentage * 100, LengthUnit.Percent);
            }
        }
        
        private void UpdateJetpackFuel(float currentFuelPercentage)
        {
            if (_jetpackFuelLabel != null)
            {
                _jetpackFuelLabel.style.height = new Length(currentFuelPercentage * 100, LengthUnit.Percent);
            }
        }

        private void UpdateAmmo(int bulletsLeft)
        {
            if (_ammoLabel != null)
            {
                // Update the ammo label
                _ammoLabel.text = bulletsLeft.ToString();
            }
        }
        
        private void DisableHud()
        {
            gameObject.SetActive(false);
        }
        
    }
    
        
}

