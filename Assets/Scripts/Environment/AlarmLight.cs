using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using UI;

namespace Environment
{
    public class AlarmLight : MonoBehaviour
    {
        private Light _light;
        private AudioSource _audioSource;
        [SerializeField] private float blinkSpeed = 5f;
        private CancellationTokenSource _cts;

        void Start()
        {
            _light = GetComponent<Light>();
            _audioSource = GetComponent<AudioSource>();
            _cts = new CancellationTokenSource();
            BlinkLightAsync(_cts.Token).Forget();
        }

        private void OnEnable()
        {
            PauseMenu.UIEnableEvent += StopBlinking;
            PauseMenu.UIDisableEvent += StartBlinking;
            GameOver.UIEnableEvent += StopBlinking;
            GameOver.UIDisableEvent += StartBlinking;
            GameWin.UIEnableEvent += StopBlinking;
            GameWin.UIDisableEvent += StartBlinking;
        }
        
        private void OnDisable()
        {
            PauseMenu.UIEnableEvent -= StopBlinking;
            PauseMenu.UIDisableEvent -= StartBlinking;
            GameOver.UIEnableEvent -= StopBlinking;
            GameOver.UIDisableEvent -= StartBlinking;
            GameWin.UIEnableEvent -= StopBlinking;
            GameWin.UIDisableEvent -= StartBlinking;
        }
        
        private void StartBlinking()
        {
            _light.enabled = true;
            _audioSource.Play();
        }
        
        private void StopBlinking()
        {
            _light.enabled = false;
            _audioSource.Stop();
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