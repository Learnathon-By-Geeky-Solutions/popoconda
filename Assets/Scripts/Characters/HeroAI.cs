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
        private bool _canShield;
        private int _bossState;
        private bool _pauseState;
        
        private CancellationTokenSource _cancellationTokenSource;
        
        public new delegate void StatEvent();
        public static event StatEvent OnHeroSurvive;
        
        
        protected override void OnEnable()
        {
            CutsceneManager.OnCutsceneStart += HandleGamePause;
            CutsceneManager.OnCutsceneEnd += HandleGameStart;
            PlayerController.OnBulletShoot += HandleReaction;
            WeaponContainer.OnWeaponEquip += UpdateBossState;
            CutsceneManager.OnBlastEvent += ApplyDeath;
            
            _cancellationTokenSource = new CancellationTokenSource();
            
            base.OnEnable();
            if(_bossState >= 1) _canDash = true;
            if(_bossState >= 2) _canShield = true;
        }
        
        protected override void OnDisable()
        {
            CutsceneManager.OnCutsceneStart -= HandleGamePause;
            CutsceneManager.OnCutsceneEnd -= HandleGameStart;
            PlayerController.OnBulletShoot -= HandleReaction;
            WeaponContainer.OnWeaponEquip -= UpdateBossState;
            CutsceneManager.OnBlastEvent -= ApplyDeath;
            
            _cancellationTokenSource.Cancel(); // Cancel any ongoing tasks
            _cancellationTokenSource.Dispose(); // Dispose of the CancellationTokenSource

            base.OnDisable(); // Call base OnDestroy to ensure correct event unsubscription
        }
        
        private void HandleGameStart()
        {
            Debug.Log("Game Started");
            PerformActionsAsync(_cancellationTokenSource.Token).Forget();
            _pauseState = false;
        }

        private void HandleGamePause()
        {
            _pauseState = true;
        }

        private async UniTask PerformActionsAsync(CancellationToken token)
        {
            await UniTask.Delay(1000, cancellationToken: token);
            await FireActionsAsync(token);
        }

        private async UniTask FireActionsAsync(CancellationToken token)
        {
            if (!ShootingController) return;
            if (_pauseState) return;
            ShootingController.FireBullet(PlayerDirection);

            if (!token.IsCancellationRequested) 
            {
                await PerformActionsAsync(token); // Repeat action if not cancelled
            }
        }
        
        private void HandleReaction()
        {
            if(_canShield && Shield.CanUseShield)
            {
                Shield.ShieldAsync(_cancellationTokenSource.Token).Forget();
            }
            else if(_canDash && Dash.CanDash)
            {
                float randomDirection = Random.Range(-3f, 3f);
                Dash.DashAsync(randomDirection, _cancellationTokenSource.Token).Forget();
            }
            else
            {
                Debug.Log("No action available");
            }
        }

        private void UpdateBossState(int state)
        {
            if (state <= _bossState) return;
            _bossState = state;
        }
        
        private async void ApplyDeath()
        {
            if(_bossState < 3)
            {
                await UniTask.Delay(1000, cancellationToken: _cancellationTokenSource.Token);
                _bossState = 3;
                _pauseState = true;
                ApplyHeroDeath();
            }
            else
            {
                Shield.ShieldAsync(_cancellationTokenSource.Token).Forget();
                OnHeroSurvive?.Invoke();
            }
        }
        
    }
}
