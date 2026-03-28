using UnityEngine;
using Cysharp.Threading.Tasks;
using Combat;

namespace Characters
{
    public class BossPhaseController : MonoBehaviour
    {
        [SerializeField] private float threshold = 0.5f;
        [SerializeField] private SpriteRenderer bossRenderer;
        [SerializeField] private ParticleSystem phaseEffect;
        private bool _done;

        private void OnEnable() => GetComponent<Health>().OnHealthChange += (val) => {
            if (!_done && val <= threshold) TriggerPhase();
        };

        private void TriggerPhase()
        {
            _done = true;
            if (phaseEffect) phaseEffect.Play();
            bossRenderer.color = Color.red;
            // Add Screen Shake logic here if desired
        }
    }
}