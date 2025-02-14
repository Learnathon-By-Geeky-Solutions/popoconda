using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Threading;

namespace Combat
{
    public class FireLaser : MonoBehaviour
    {
        [SerializeField] private GameObject laserSpawnPoint;
        [SerializeField] private GameObject laserPrefab;
        [SerializeField] private int damageAmount;
        [SerializeField] private float maxLaserDistance;
        
        private bool _isFiring;
        private LineRenderer _laserLineRenderer;
        private CancellationTokenSource _cancellationTokenSource;

        public static event Bullet.HitHandler OnLaserHit;

        private void Awake()
        {
            _laserLineRenderer = null;
        }
        
        private void OnDisable()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }

        // <<summary>>
        // <<param name="direction">>The direction in which the laser projectile will be fired<</param>>
        // <<summary>>
        public void FireLaserProjectile(Vector3 direction)
        {
            if (_isFiring) return;
            
            _isFiring = true;
            _cancellationTokenSource = new CancellationTokenSource();

            _laserLineRenderer = Instantiate(laserPrefab, laserSpawnPoint.transform.position, Quaternion.identity)
                                 .GetComponent<LineRenderer>();
            _laserLineRenderer.enabled = true;

            ShootLaserAsync(direction, _cancellationTokenSource.Token).Forget();
        }
        
        private async UniTask ShootLaserAsync(Vector3 direction, CancellationToken token)
        {
            float fireDuration = 1f;
            float elapsedTime = 0f;

            while (elapsedTime < fireDuration)
            {
                if (token.IsCancellationRequested)
                {
                    StopFiring();
                    return;
                }

                UpdateLaserPositions(direction);
                elapsedTime += Time.unscaledDeltaTime; // Use unscaled time to avoid being affected by timeScale

                await UniTask.WaitForSeconds(Time.unscaledDeltaTime, ignoreTimeScale: true, cancellationToken: token);
            }

            StopFiring();
        }


        private void UpdateLaserPositions(Vector3 direction)
        {
            if (_laserLineRenderer == null || laserSpawnPoint == null) return;

            _laserLineRenderer.SetPosition(0, laserSpawnPoint.transform.position);
            Vector3 endPoint = laserSpawnPoint.transform.position + direction * maxLaserDistance;

            if (Physics.Raycast(laserSpawnPoint.transform.position, direction, out RaycastHit hit, maxLaserDistance))
            {
                _laserLineRenderer.SetPosition(1, hit.point);
                OnLaserHit?.Invoke(damageAmount, hit.collider.gameObject);
            }
            else
            {
                _laserLineRenderer.SetPosition(1, endPoint);
            }
        }

        private void StopFiring()
        {
            _isFiring = false;
            if (_laserLineRenderer != null)
            {
                _laserLineRenderer.enabled = false;
            }
        }
    }
}
