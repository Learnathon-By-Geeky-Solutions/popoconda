using System.Threading;
using UnityEngine;
using Combat;
using Cysharp.Threading.Tasks;
using Random = UnityEngine.Random;

namespace Characters
{
    public class Boss1Script : Enemy
    {
        private FireLaser _fireLaser;
        
        private CancellationTokenSource _cancellationTokenSource;

        protected override void Awake()
        {
            base.Awake();
            _fireLaser = GetComponent<FireLaser>();
            _cancellationTokenSource = new CancellationTokenSource();
            PerformActionsAsync(_cancellationTokenSource.Token).Forget();
        }
        
        protected override void OnDestroy()
        {
            // Cancel and dispose the token source
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();

            base.OnDestroy(); // Call base OnDestroy to ensure correct event unsubscription
        }

        private async UniTask PerformActionsAsync(CancellationToken token)
        {
            await UniTask.Delay(1000, cancellationToken: token);
            await FireActionsAsync(token);
        }

        private async UniTask FireActionsAsync(CancellationToken token)
        {
            bool isFiringBullet = Random.Range(0, 3) != 0;
            float fireDuration = isFiringBullet ? Random.Range(15f, 20f) : Random.Range(5f, 7f);
            float startTime = Time.time;

            while (Time.time - startTime < fireDuration)
            {
                if (gameObject == null || !gameObject.activeInHierarchy || token.IsCancellationRequested)
                {
                    return; // Exit if object is destroyed or task is cancelled
                }

                if (isFiringBullet)
                {
                    ShootingController.FireBullet(PlayerDirection);
                }
                else
                {
                    _fireLaser.FireLaserProjectile(PlayerDirection);
                }

                await UniTask.Delay(100, cancellationToken: token);
            }

            if (!token.IsCancellationRequested) 
            {
                await PerformActionsAsync(token); // Repeat action if not cancelled
            }
        }
    }
}
