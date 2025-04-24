using System.Threading;
using Cutscene;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Weapon;
using Random = UnityEngine.Random;

namespace Characters
{
    public class HeroAI : Hero
    {
        private bool _canDash;
        private int _bossState;
        
        private CancellationTokenSource _cancellationTokenSource;
        
        
        protected override void OnEnable()
        {
            CutsceneManager.OnCutsceneEnd += HandleGameStart;
            PlayerController.OnBulletShoot += HandleDash;
            WeaponContainer.OnWeaponEquip += UpdateBossState;
            
            _cancellationTokenSource = new CancellationTokenSource();
            
            base.OnEnable();
            if(_bossState >= 1) _canDash = true;
        }
        
        protected override void OnDisable()
        {
            CutsceneManager.OnCutsceneEnd -= HandleGameStart;
            PlayerController.OnBulletShoot -= HandleDash;
            WeaponContainer.OnWeaponEquip -= UpdateBossState;
            
            _cancellationTokenSource.Cancel(); // Cancel any ongoing tasks
            _cancellationTokenSource.Dispose(); // Dispose of the CancellationTokenSource

            base.OnDisable(); // Call base OnDestroy to ensure correct event unsubscription
        }
        
        private void HandleGameStart()
        {
            Debug.Log("Game Started");
            PerformActionsAsync(_cancellationTokenSource.Token).Forget();
        }

        private async UniTask PerformActionsAsync(CancellationToken token)
        {
            await UniTask.Delay(1000, cancellationToken: token);
            await FireActionsAsync(token);
        }

        private async UniTask FireActionsAsync(CancellationToken token)
        {
            if (!ShootingController) return;
            ShootingController.FireBullet(PlayerDirection);

            if (!token.IsCancellationRequested) 
            {
                await PerformActionsAsync(token); // Repeat action if not cancelled
            }
        }
        
        private void HandleDash()
        {
            if (_canDash)
            {
                float dashDirection = (Random.value < 1f) ? -1 : 1;
                Dash.DashAsync(dashDirection, _cancellationTokenSource.Token).Forget();
            }
        }

        private void UpdateBossState(int state)
        {
            if (state <= _bossState) return;
            _bossState = state;
        }
        
    }
}
