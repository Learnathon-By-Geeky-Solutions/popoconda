using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Combat
{
    public class Shield : MonoBehaviour
    {
        [Header("Shield Settings")]
        [SerializeField] private int shieldDuration = 2; 
        [SerializeField] private int shieldCooldown = 5; 
        [SerializeField] private GameObject shield;
        
        private bool _isShieldActive;
        private bool _canUseShield = true;
        
        public bool CanUseShield => _canUseShield;

        
        private void OnEnable()
        {
            _isShieldActive = false;
            _canUseShield = true;
        }
        public async UniTask ShieldAsync(CancellationToken cancellationToken)
        {
            if(!_canUseShield) return;
            if (_isShieldActive) return;
            
            _isShieldActive = true;
            shield.SetActive(true);
            Debug.Log("Shield is active!");
            
            await UniTask.Delay(shieldDuration * 1000, cancellationToken: cancellationToken);
            
            // Deactivate the shield
            shield.SetActive(false);
            Debug.Log("Shield is deactivated!");
            _isShieldActive = false;
            
            // Start cooldown
            _canUseShield = false; 
            await UniTask.Delay(shieldCooldown * 1000, cancellationToken: cancellationToken);
            _canUseShield = true; 
        }
    }
    
}