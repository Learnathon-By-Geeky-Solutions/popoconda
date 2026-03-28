using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace Characters
{
    public class BossHealthBar : MonoBehaviour
    {
        [SerializeField] private Image frontBar;
        [SerializeField] private Image backBar;
        [SerializeField] private float animationSpeed = 0.5f;

        private CancellationTokenSource _cts;

        private void OnEnable()
        {
            _cts = new CancellationTokenSource();
            Enemy.OnEnemyHealthChange += UpdateBar;
        }

        private void OnDisable()
        {
            Enemy.OnEnemyHealthChange -= UpdateBar;
            _cts?.Cancel();
        }

        private void UpdateBar(float normalizedHealth)
        {
            frontBar.fillAmount = normalizedHealth;
            AnimateBackBar(normalizedHealth, _cts.Token).Forget();
        }

        private async UniTaskVoid AnimateBackBar(float target, CancellationToken token)
        {
            await UniTask.Delay(500, cancellationToken: token);
            while (backBar.fillAmount > target && !token.IsCancellationRequested)
            {
                backBar.fillAmount = Mathf.MoveTowards(backBar.fillAmount, target, animationSpeed * Time.deltaTime);
                await UniTask.Yield(token);
            }
        }
    }
}