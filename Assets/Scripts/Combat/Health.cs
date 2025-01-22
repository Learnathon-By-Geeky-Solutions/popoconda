using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Combat
{
    [Serializable]
    public class Health
    {
        [SerializeField] private int maxHealth;
        [SerializeField] private int regenRate;
        [SerializeField] private int regenDelay;
        private int _currentHealth;
        private bool _isPlayer;

        private CancellationTokenSource _regenCancelToken;

        public delegate void StatEvent();
        public delegate void StatEventWithFloat(float value);
        public StatEvent OnDeath;
        public StatEventWithFloat OnHealthChange;

        public void Initialize(bool isPlayer)
        {
            _currentHealth = maxHealth;
            _isPlayer = isPlayer;
            _regenCancelToken = new CancellationTokenSource();
        }

        public void TakeDamage(int damage)
        {
            _currentHealth -= damage;
            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                OnDeath?.Invoke();
            }
            float healthPercentage = (float)_currentHealth / maxHealth;
            OnHealthChange?.Invoke(healthPercentage);

            // Cancel any ongoing regeneration and start a new one if the player
            _regenCancelToken?.Cancel();
            _regenCancelToken = new CancellationTokenSource();

            if (_isPlayer && _currentHealth > 0)
            {
                RegenHealthAsync(_regenCancelToken.Token).Forget();
            }
        }

        private async UniTaskVoid RegenHealthAsync(CancellationToken cancellationToken)
        {
            while (_currentHealth < maxHealth)
            {
                await UniTask.Delay(regenDelay, cancellationToken: cancellationToken);
                _currentHealth = Mathf.Min(_currentHealth + regenRate, maxHealth);
                float healthPercentage = (float)_currentHealth / maxHealth;
                OnHealthChange?.Invoke(healthPercentage);
            }
        }

        public void StopRegen()
        {
            _regenCancelToken?.Cancel();
        }

        public void DisposeToken()
        {
            _regenCancelToken?.Cancel();
            _regenCancelToken?.Dispose();
        }
    }
}
