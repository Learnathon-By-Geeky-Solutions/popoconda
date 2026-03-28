using System.Threading;
using UnityEngine;
using Combat;
using Cysharp.Threading.Tasks;
using Dialogue;
using Random = UnityEngine.Random;

namespace Characters
{
    public class Boss1Script : Enemy
    {
        private FireLaser _fireLaser;
        private CancellationTokenSource _cancellationTokenSource;

        [Header("Laser Settings")]
        [SerializeField] private float laserChargeTime = 1.2f; // ⚠️ warning time
        [SerializeField] private float laserDuration = 3.5f;   // short & deadly

        protected override void Awake()
        {
            DialogueManager.OnDialogueEnd += HandleGameStart;
            base.Awake();

            _fireLaser = GetComponent<FireLaser>();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        protected override void OnDestroy()
        {
            DialogueManager.OnDialogueEnd -= HandleGameStart;

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();

            base.OnDestroy();
        }

        private void HandleGameStart()
        {
            PerformActionsAsync(_cancellationTokenSource.Token).Forget();
        }

        private async UniTask PerformActionsAsync(CancellationToken token)
        {
            await UniTask.Delay(1000, cancellationToken: token);
            await FireActionsAsync(token);
        }

        private async UniTask FireActionsAsync(CancellationToken token)
        {
            bool fireBullets = Random.Range(0, 3) != 0;
            float fireDuration = fireBullets
                ? Random.Range(10f, 14f)  // bullets = pressure
                : laserDuration;          // laser = deadly burst

            float startTime = Time.time;

            // ⚠️ LASER CHARGE WARNING
            if (!fireBullets)
            {
                Debug.Log("Boss1: Charging Laser!");
                await UniTask.Delay((int)(laserChargeTime * 1000), cancellationToken: token);
            }

            while (Time.time - startTime < fireDuration)
            {
                if (!gameObject || !gameObject.activeInHierarchy || token.IsCancellationRequested)
                    return;

                if (fireBullets)
                {
                    ShootingController.FireBullet(PlayerDirection);
                    await UniTask.Delay(150, cancellationToken: token); // slower bullets
                }
                else
                {
                    _fireLaser.FireLaserProjectile(PlayerDirection);
                    await UniTask.Delay(80, cancellationToken: token); // laser ticks fast
                }
            }

            if (!token.IsCancellationRequested)
                await PerformActionsAsync(token);
        }
    }
}

