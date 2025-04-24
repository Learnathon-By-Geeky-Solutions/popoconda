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
        
        private ProgressBar _playerHealthBar;
        private ProgressBar _enemyHealthBar;
        private ProgressBar _jetpackFuelBar;
        private Label _ammoLabel;
        
        void Start()
        {
            VisualElement root = hudDocument.rootVisualElement;
            
            // get label elements
            _playerHealthBar = root.Q<ProgressBar>("player-health");
            _enemyHealthBar = root.Q<ProgressBar>("enemy-health");
            _jetpackFuelBar = root.Q<ProgressBar>("jetpack-fuel");
            _ammoLabel = root.Q<Label>("ammo-label");
            
            PlayerController.OnPlayerHealthChange += UpdatePlayerHealth;
            PlayerController.OnJetpackFuelChange += UpdateJetpackFuel;
            Hero.OnHeroHealthChange += UpdateEnemyHealth;
            ShootingController.OnBulletCountChange += UpdateAmmo;
            GameManager.DisableHudEvent += DisableHud;
        }

        private void OnDestroy()
        {
            PlayerController.OnPlayerHealthChange -= UpdatePlayerHealth;
            PlayerController.OnJetpackFuelChange -= UpdateJetpackFuel;
            Hero.OnHeroHealthChange -= UpdateEnemyHealth;
            ShootingController.OnBulletCountChange -= UpdateAmmo;
            GameManager.DisableHudEvent -= DisableHud;
        }

        private void UpdatePlayerHealth(float currentHealthPercentage)
        {
            if (_playerHealthBar != null)
            {
                _playerHealthBar.value = currentHealthPercentage * 100;
            }
        }

        private void UpdateEnemyHealth(float currentHealthPercentage)
        {
            if (_enemyHealthBar != null)
            {
                _enemyHealthBar.value = currentHealthPercentage * 100;
            }
        }
        
        private void UpdateJetpackFuel(float currentFuelPercentage)
        {
            if (_jetpackFuelBar != null)
            {
                _jetpackFuelBar.value = currentFuelPercentage * 100;
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

