using Combat;
using UnityEngine;

namespace Effects
{
    public class BlastEffect : MonoBehaviour
    {
        private ParticleSystem _particleSystem;

        private void Start()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        private void OnEnable()
        {
            EnergyBlast.OnEnergyBlast += HandleBlastEffect;
        }

        private void OnDisable()
        {
            EnergyBlast.OnEnergyBlast -= HandleBlastEffect;
        }
        
        private void HandleBlastEffect()
        {
            _particleSystem.Play();
        }
        
    }
}