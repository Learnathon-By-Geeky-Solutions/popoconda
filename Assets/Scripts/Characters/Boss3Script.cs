using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace Characters
{
    public class Boss3Script : Enemy
    {
        private CancellationToken _cancellationToken;

        protected override void Awake()
        {
            base.Awake();
            _cancellationToken = this.GetCancellationTokenOnDestroy();
            PerformActionsAsync().Forget();
        }
        
        private async UniTask PerformActionsAsync()
        {
            while (this && gameObject.activeInHierarchy)
            {
                await FireBulletsForDurationAsync();
            }
        }
        
        private async UniTask FireBulletsForDurationAsync()
        {
            float fireDuration = Random.Range(10, 12);
            float startTime = Time.time;

            while (Time.time - startTime < fireDuration && this && gameObject.activeInHierarchy)
            {
                ShootingController.FireBullet(PlayerDirection);
                await UniTask.Delay(500, cancellationToken: _cancellationToken);
            }
        }
    }
    
}
