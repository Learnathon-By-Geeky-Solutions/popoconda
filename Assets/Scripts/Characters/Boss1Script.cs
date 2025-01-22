using UnityEngine;
using Combat;
using Cysharp.Threading.Tasks;
using Random = UnityEngine.Random;

namespace Characters
{
    public class Boss1Script : Enemy
    {
        private FireLaser _fireLaser;

        protected override void Awake()
        {
            base.Awake();
            _fireLaser = GetComponent<FireLaser>();
            PerformActionsAsync().Forget();
        }

        private async UniTask PerformActionsAsync()
        {
            await UniTask.Delay(1000);
            await FireActionsAsync();
        }

        private async UniTask FireActionsAsync()
        {
            bool isFiringBullet = Random.Range(0, 3) != 0;
            float fireDuration = isFiringBullet ? Random.Range(15f, 20f) : Random.Range(5f, 7f);
            float startTime = Time.time;

            while (Time.time - startTime < fireDuration)
            {
                if (!this || !gameObject.activeInHierarchy)
                {
                    return;
                }
                if (isFiringBullet)
                {
                    ShootingController.FireBullet(PlayerDirection);
                }
                else
                {
                    _fireLaser.FireLaserProjectile(PlayerDirection);
                }
                await UniTask.Delay(100);
            }
            await PerformActionsAsync();
        }
    }
}