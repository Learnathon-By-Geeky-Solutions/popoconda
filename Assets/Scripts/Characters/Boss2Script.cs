using System.Threading;
using Combat;
using Cysharp.Threading.Tasks;
using Dialogue;
using UnityEngine;

namespace Characters
{
    public class Boss2Script : Enemy
    {
        private StunController _stunController;

        [Header("Stun Settings")]
        [SerializeField] private float stunChance = 0.35f;
        [SerializeField] private float stunChargeTime = 1.2f; // ⚠️ warning
        [SerializeField] private float stunCooldown = 2f;     // punish window

        private bool _isStunning;
        private CancellationToken _token;

        protected override void Awake()
        {
            DialogueManager.OnDialogueEnd += HandleGameStart;
            base.Awake();

            _stunController = GetComponent<StunController>();
            _token = this.GetCancellationTokenOnDestroy();
        }

        protected override void OnDestroy()
        {
            DialogueManager.OnDialogueEnd -= HandleGameStart;
            base.OnDestroy();
        }

        private void HandleGameStart()
        {
            PerformActionsAsync().Forget();
        }

        private async UniTask PerformActionsAsync()
        {
            while (this && gameObject.activeInHierarchy)
            {
                await FireBulletsAsync();

                if (Random.value < stunChance && !_isStunning)
                {
                    _isStunning = true;
                    await ChargeAndStunAsync();
                }
            }
        }

        private async UniTask FireBulletsAsync()
        {
            float duration = Random.Range(6f, 9f);
            float start = Time.time;

            while (Time.time - start < duration && this && gameObject.activeInHierarchy)
            {
                ShootingController.FireBullet(PlayerDirection);
                await UniTask.Delay(600, cancellationToken: _token); // slower bullets
            }
        }

        private async UniTask ChargeAndStunAsync()
        {
            Debug.Log("Boss2: Charging stun!");
            await UniTask.Delay((int)(stunChargeTime * 1000), cancellationToken: _token);

            if (!this || !gameObject.activeInHierarchy) return;

            _stunController.Stun();
            Debug.Log("Boss2: Player stunned!");

            // 🔻 punish window
            await UniTask.Delay((int)(stunCooldown * 1000), cancellationToken: _token);
            _isStunning = false;
        }
    }
}