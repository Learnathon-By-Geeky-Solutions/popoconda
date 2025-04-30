using System;
using Cutscene;
using UnityEngine;

namespace Combat
{
    [Serializable]
    public class Health
    {
        [SerializeField] private int maxHealth;
        
        private int _currentHealth;

        
        public delegate void StatEvent();
        public delegate void StatEventWithFloat(float value);
        public StatEvent OnDeath;
        public StatEventWithFloat OnHealthChange;

        public void Initialize()
        {
            _currentHealth = maxHealth;
            CutsceneManager.OnCutsceneEnd += UpdateHealth;
        }
        
        private void OnDestroy()
        {
            CutsceneManager.OnCutsceneEnd -= UpdateHealth;
        }
        
        private void UpdateHealth()
        {
            float healthPercentage = (float)_currentHealth / maxHealth;
            OnHealthChange?.Invoke(healthPercentage);
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
        }
        
        public void ResetHealth()
        {
            _currentHealth = maxHealth;
            float healthPercentage = (float)_currentHealth / maxHealth;
            OnHealthChange?.Invoke(healthPercentage);
        }
        
        public void HealthBuff(int value)
        {
            maxHealth += value;
        }
        
    }
}
