using UnityEngine;
using Characters;
using Combat;
using Cutscene;
using Game;
using UnityEngine.UIElements;

namespace UI
{
    public class HudHandler : MonoBehaviour
    {
        [SerializeField] private UIDocument hudDocument;
        
        private ProgressBar _playerHealthBar;
        private ProgressBar _heroHealthBar;
        private ProgressBar _jetpackFuelBar;
        private Label _ammoLabel;
        private VisualElement _playerHealthBarBackground;
        
        void Start()
        {
            VisualElement root = hudDocument.rootVisualElement;
            
            // get label elements
            _playerHealthBar = root.Q<ProgressBar>("player-health");
            _heroHealthBar = root.Q<ProgressBar>("hero-health");
            _jetpackFuelBar = root.Q<ProgressBar>("jetpack-fuel");
            _ammoLabel = root.Q<Label>("ammo-label");
            _playerHealthBarBackground =_playerHealthBar.Q(className: "unity-progress-bar__progress");
            
            PlayerController.OnPlayerHealthChange += UpdatePlayerHealth;
            PlayerController.OnJetpackFuelChange += UpdateJetpackFuel;
            Hero.OnHeroHealthChange += UpdateHeroHealth;
            ShootingController.OnBulletCountChange += UpdateAmmo;
            GameManager.DisableHudEvent += DisableHud;
        }

        private void OnDestroy()
        {
            PlayerController.OnPlayerHealthChange -= UpdatePlayerHealth;
            PlayerController.OnJetpackFuelChange -= UpdateJetpackFuel;
            Hero.OnHeroHealthChange -= UpdateHeroHealth;
            ShootingController.OnBulletCountChange -= UpdateAmmo;
            GameManager.DisableHudEvent -= DisableHud;
        }

        private void UpdatePlayerHealth(float currentHealthPercentage)
        {
            if (_playerHealthBar != null)
            {
                
                _playerHealthBar.value = currentHealthPercentage * 100;
                if (_playerHealthBar.value >= 75)
                {
                    _playerHealthBarBackground.style.backgroundColor = new StyleColor(new Color(0.0f, 1.0f, 0.0f));
                }
                else if (_playerHealthBar.value >=50)
                {
                    _playerHealthBarBackground.style.backgroundColor = new StyleColor(new Color(1.0f, 1.0f, 0.0f));
                }
                else if (_playerHealthBar.value >= 25)
                {
                    _playerHealthBarBackground.style.backgroundColor = new StyleColor(new Color(1.0f, .5f, 0.0f));
                }
                else
                {
                    _playerHealthBarBackground.style.backgroundColor = new StyleColor(new Color(1.0f, 0.0f, 0.0f));
                }
            }
        }

        private void UpdateHeroHealth(float currentHealthPercentage)
        {
            if (_heroHealthBar != null)
            {
                _heroHealthBar.value = currentHealthPercentage * 100;
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

