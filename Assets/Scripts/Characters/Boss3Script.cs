using Cysharp.Threading.Tasks;
using System.Threading;
using Combat;
using Dialogue;
using UnityEngine;

namespace Characters
{
    public class Boss3Script : Hero
    {
        private EnergyBlast _energyBlast;
        private Dash _dash;
        
        [Header("EnergyBlastSettings")]
        [SerializeField] private float blastChance;
        [SerializeField] private float coreHeatTime;
        
        [Header("DashSettings")]
        [SerializeField] private float dashChance;
        
        private CancellationToken _cancellationToken;

        protected override void Awake()
        {
            DialogueManager.OnDialogueEnd += HandleGameStart;
            base.Awake();
            _cancellationToken = this.GetCancellationTokenOnDestroy();
            _energyBlast = GetComponent<EnergyBlast>();
            _dash = GetComponent<Dash>();
        }
        
        protected override void OnDisable()
        {
            DialogueManager.OnDialogueEnd -= HandleGameStart;
            base.OnDisable();
        }
        
        private void HandleGameStart()
        {
            PerformActionsAsync().Forget();
        }
        
        
        private async UniTask PerformActionsAsync()
        {
            while (this && gameObject.activeInHierarchy)
            {
                await FireBulletsForDurationAsync();
                
                if (Random.value < dashChance && !_dash.IsDashing)
                {
                    float dashDirection = (Random.value < 1f) ? -1 : 1;
                    await _dash.DashAsync(dashDirection, _cancellationToken);
                }
                
                if (Random.value < blastChance)
                {
                    await UseEnergyBlastAsync();
                }
            }
        }
        
        private async UniTask FireBulletsForDurationAsync()
        {
            float fireDuration = Random.Range(10, 12);
            float startTime = Time.time;

            while (Time.time - startTime < fireDuration && this && gameObject.activeInHierarchy && !_dash.IsDashing)
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