using Cysharp.Threading.Tasks;
using System.Threading;
using Combat;
using UnityEngine;

namespace Characters
{
    public class Boss3Script : Enemy
    {
        private EnergyBlast _energyBlast;
        
        [Header("EnergyBlastSettings")]
        [SerializeField] private float blastChance = 0.5f; // 20% chance to use energy blast
        [SerializeField] private float coreHeatTime = 2f; // Time to "charge" the energy blast
        
        private CancellationToken _cancellationToken;

        protected override void Awake()
        {
            base.Awake();
            _cancellationToken = this.GetCancellationTokenOnDestroy();
            _energyBlast = GetComponent<EnergyBlast>(); // Ensure EnergyBlast component is attached
            PerformActionsAsync().Forget();
        }
        
        private async UniTask PerformActionsAsync()
        {
            while (this && gameObject.activeInHierarchy)
            {
                await FireBulletsForDurationAsync();
                
                // Randomly decide whether to use energy blast
                if (Random.value < blastChance)
                {
                    await UseEnergyBlastAsync();
                }
            }
        }
        
        private async UniTask FireBulletsForDurationAsync()
        {
            float fireDuration = Random.Range(1, 2);
            float startTime = Time.time;

            while (Time.time - startTime < fireDuration && this && gameObject.activeInHierarchy)
            {
                ShootingController.FireBullet(PlayerDirection);
                await UniTask.Delay(500, cancellationToken: _cancellationToken);
            }
        }

        private async UniTask UseEnergyBlastAsync()
        {
            // Simulate a "charging" phase for the energy blast
            Debug.Log("Boss 3 is charging energy blast...");
            await UniTask.Delay((int)(coreHeatTime * 1000), cancellationToken: _cancellationToken);

            // Trigger the energy blast
            if (_energyBlast != null)
            {
                _energyBlast.Blast();
                Debug.Log("Boss 3 used Energy Blast!");
                await UniTask.Delay(500, cancellationToken: _cancellationToken);
            }
        }
    }
}