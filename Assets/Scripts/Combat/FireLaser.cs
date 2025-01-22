using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Combat
{
    public class FireLaser : MonoBehaviour
    {
        [SerializeField] private GameObject laserSpawnPoint;
        [SerializeField] private GameObject laserPrefab; // The prefab with the LineRenderer
        [SerializeField] private int damageAmount;
        [SerializeField] private float maxLaserDistance;
        private bool _isFiring;
        private LineRenderer _laserLineRenderer;

        public static event Bullet.HitHandler OnLaserHit;

        private void Awake()
        {
            _laserLineRenderer = null;
        }

        public void FireLaserProjectile(Vector3 direction)
        {
            if (_isFiring) return;
            _isFiring = true;
            
            _laserLineRenderer = Instantiate(laserPrefab, laserSpawnPoint.transform.position, Quaternion.identity).GetComponent<LineRenderer>();
            _laserLineRenderer.enabled = true;

            ShootLaserAsync(direction).Forget();
        }

        private async UniTask ShootLaserAsync(Vector3 direction)
        {
            float fireDuration = 1f;
            float elapsedTime = 0f;

            while (elapsedTime < fireDuration)
            {
                if (_laserLineRenderer == null || laserSpawnPoint == null)
                {
                    StopFiring();
                    return;
                }

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

                elapsedTime += Time.deltaTime;
                await UniTask.Yield();
            }

            StopFiring();
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
