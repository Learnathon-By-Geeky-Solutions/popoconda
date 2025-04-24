using UnityEngine;
using UnityEngine.UIElements;
using Cysharp.Threading.Tasks;
using System;
using Characters;

namespace UI
{
    public class DamageScreen : MonoBehaviour
    {
        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private float fadeDuration = 1f;
    
        private VisualElement damageOverlay;
        private IDisposable hitSubscription;
    
        private void OnEnable()
        {
            damageOverlay = uiDocument.rootVisualElement.Q<VisualElement>("DamageEffect"); // Assuming this is your element ID
    
            SetOverlayAlpha(0); // Hide on enable
    
            // Subscribe to player hit event (adjust depending on your event system)
            PlayerController.OnPlayerHit += HandlePlayerHit;
        }
    
        private void OnDisable()
        {
            PlayerController.OnPlayerHit -= HandlePlayerHit;
        }
    
        private void HandlePlayerHit()
        {
            ShowEffectAsync().Forget();
        }
    
        private async UniTaskVoid ShowEffectAsync()
        {
            SetOverlayAlpha(1); // Show fully
    
            float elapsed = 0f;
    
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1, 0, elapsed / fadeDuration);
                SetOverlayAlpha(alpha);
                await UniTask.Yield(PlayerLoopTiming.Update);
            }
    
            SetOverlayAlpha(0); // Ensure it's fully hidden at the end
        }
    
        private void SetOverlayAlpha(float alpha)
        {
            if (damageOverlay != null)
            {
                damageOverlay.style.opacity = alpha;
            }
        }
    }

}
