using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace Environment
{
    public class AlarmLight : MonoBehaviour
    {
        private Light _light;
        [SerializeField] private float blinkSpeed = 5f;
        private CancellationTokenSource _cts;

        void Start()
        {
            _light = GetComponent<Light>();
            _cts = new CancellationTokenSource();
            BlinkLightAsync(_cts.Token).Forget();
        }

        private async UniTaskVoid BlinkLightAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (_light.enabled)
                {
                    _light.intensity = (Mathf.Sin(Time.time * blinkSpeed) + 1) / 2 * 2000;
                }
                await UniTask.Yield(); // Yield to the next frame
            }
        }

        void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }
    }
}