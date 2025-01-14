using System.Collections;
using Characters;
using UnityEngine;

namespace Combat
{
    public class FireLaser : MonoBehaviour
    {
        [SerializeField] private GameObject laserSpawnPoint;
        [SerializeField] private LineRenderer laserLineRenderer;
        [SerializeField] private int damageAmount;
        [SerializeField] private float maxLaserDistance;
        private Enemy _enemy;
        private bool _isFiring;

        private void Awake()
        {
            laserLineRenderer.enabled = false;
            _enemy = GetComponent<Enemy>();
        }

        public void FireLaserProjectile(Vector3 direction)
        {
            if (_isFiring) return;
            _isFiring = true;
            laserLineRenderer.enabled = true;
            StartCoroutine(ShootLaser());
        }

        private IEnumerator ShootLaser()
        {
            float fireDuration = 2f; // Duration to fire the laser
            float elapsedTime = 0f;

            while (elapsedTime < fireDuration)
            {
                laserLineRenderer.SetPosition(0, laserSpawnPoint.transform.position);

                if (Physics.Raycast(laserSpawnPoint.transform.position, _enemy.directionToPlayer, out RaycastHit hit, maxLaserDistance))
                {
                    Debug.Log("Laser Hit: " + hit.collider.name);
                    laserLineRenderer.SetPosition(1, hit.point);
                    Health health = hit.collider.GetComponent<Health>();
                    health?.TakeDamage(damageAmount);
                }
                else
                {
                    laserLineRenderer.SetPosition(1, laserSpawnPoint.transform.position + _enemy.directionToPlayer * maxLaserDistance);
                }

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            StopFiring();
        }

        private void StopFiring()
        {
            _isFiring = false;
            laserLineRenderer.enabled = false;
        }
    }
}
