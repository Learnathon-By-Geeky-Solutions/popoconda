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

        protected override void Awake()
        {
            base.Awake();
            _stunController = GetComponent<StunController>();
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

                    // Random chance to switch to stunning after firing duration
                    if (Random.value < stunChance)
                    {
                        _isStunning = true;
                    }
                }
            }
        }

        private async UniTask FireBulletsForDurationAsync()
        {
            float fireDuration = Random.Range(1, 2);
            float startTime = Time.time;

            while (Time.time - startTime < fireDuration)
            {
                ShootingController.FireBullet(PlayerDirection);
                await UniTask.Delay(500);
            }
            
        }

        private async UniTask StunAndSwitchBackAsync()
        {
            await UniTask.Delay((int)(coreHeatTime * 1000));
            
            _stunController.Stun(); 

            Debug.Log("Stunned. Switching back to firing.");
            _isStunning = false;
        }
    }
}
