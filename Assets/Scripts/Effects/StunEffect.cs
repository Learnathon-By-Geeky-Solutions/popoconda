using Combat;
using UnityEngine;

namespace Effects
{
    public class StunEffect : MonoBehaviour
    {
        private ParticleSystem _particleSystem;

        private void Start()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        private void OnEnable()
        {
            StunController.OnStun += HandleStunEffect;
        }

        private void OnDisable()
        {
            StunController.OnStun -= HandleStunEffect;
        }
        
        private void HandleStunEffect(bool isStunned)
        {
            if (isStunned)
            {
                _particleSystem.Play();
            }
            else
            {
                _particleSystem.Stop();
            }
        }
    }
}
