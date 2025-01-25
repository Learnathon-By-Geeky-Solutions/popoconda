using System.Threading;
using Combat;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Characters
{
    public class Boss2Script : Enemy
    {
        private StunController _stunController;

        [Header("Stun Settings")]
        [SerializeField] private float stunChance;
        [SerializeField] private float coreHeatTime;

        private bool _isStunning;

        private CancellationToken _cancellationToken;

        protected override void Awake()
        {
            base.Awake();
            _stunController = GetComponent<StunController>();
            _cancellationToken = this.GetCancellationTokenOnDestroy();
            PerformActionsAsync().Forget();
        }

        private async UniTask PerformActionsAsync()
        {
            while (this && gameObject.activeInHierarchy)
            {
                if (_isStunning)
                {
                    await StunAndSwitchBackAsync();
                }
                else
                {
                    await FireBulletsForDurationAsync();

                    if (Random.value < stunChance)
                    {
                        _isStunning = true;
                    }
                }
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

        private async UniTask StunAndSwitchBackAsync()
        {
            await UniTask.Delay((int)(coreHeatTime * 1000), cancellationToken: _cancellationToken);

            if (this && gameObject.activeInHierarchy)
            {
                await _stunController.Stun();
                Debug.Log("Stunned. Switching back to firing.");
            }

            _isStunning = false;
        }
    }
}
