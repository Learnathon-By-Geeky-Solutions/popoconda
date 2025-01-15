using System.Collections;
using UI;
using UnityEngine;

namespace Combat
{
    public class ShootingController : MonoBehaviour
    {
        [SerializeField] private GameObject bulletSpawnPoint;
        [SerializeField] private TrailRenderer bulletTrail;
        [SerializeField] private HudHandler hudHandler;
        [SerializeField] private float timeBetweenShooting, spread, reloadTime;
        [SerializeField] private int magazineSize, bulletsPerTap;
        private bool _isReadyToShoot = true, _isReloading;
        private int _bulletsShot, _bulletsLeft;
        [SerializeField] private int damageAmount = 10;
        private const float MinTravelDistance = 0.5f;
        
        private void Awake()
        {
            _bulletsLeft = magazineSize;
        }
        
        public void FireBullet(Vector3 direction)
        {
            if (_isReadyToShoot && _bulletsLeft > 0 && !_isReloading)
            {
                _bulletsShot = 0;
                StartCoroutine(Shoot(direction));
            }
            else if (_bulletsLeft <= 0)
            {
                Reload();
            }
        }

        private IEnumerator Shoot(Vector3 direction)
        {
            _isReadyToShoot = false;

            while (_bulletsShot < bulletsPerTap && _bulletsLeft > 0)
            {
                _bulletsLeft--;
                _bulletsShot++;
                
                // Update HUD 
                if (gameObject.CompareTag("Player"))
                {
                    hudHandler.UpdateAmmo(_bulletsLeft);
                }
                

                // Apply bullet spread
                float x = Random.Range(-spread, spread);
                float y = Random.Range(-spread, spread);
                Vector3 spreadVector = bulletSpawnPoint.transform.right * x + bulletSpawnPoint.transform.up * y;
                Vector3 shootDirection = (direction + spreadVector).normalized;

                // Perform raycast
                if (Physics.Raycast(bulletSpawnPoint.transform.position, shootDirection, out RaycastHit hit, Mathf.Infinity))
                {
                    
                    Debug.Log("Bullet Hit: " + hit.collider.name);
                    float travelDistance = Vector3.Distance(bulletSpawnPoint.transform.position, hit.point);
                    TrailRenderer trail = Instantiate(bulletTrail, bulletSpawnPoint.transform.position, Quaternion.identity);
                    StartCoroutine(SpawnTrail(trail, hit));

                    if (travelDistance >= MinTravelDistance)
                    {
                        Health health = hit.collider.GetComponent<Health>();
                        health?.TakeDamage(damageAmount);
                    }
                }

                yield return new WaitForSeconds(timeBetweenShooting);
            }

            _isReadyToShoot = true;
        }

        private IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit hit)
        {
            float time = 0;
            Vector3 startPos = bulletSpawnPoint.transform.position;

            while (time < 1)
            {
                trail.transform.position = Vector3.Lerp(startPos, hit.point, time);
                time += Time.deltaTime / trail.time;
                yield return null;
            }

            trail.transform.position = hit.point;
            Destroy(trail.gameObject);
        }

        private void Reload()
        {
            if (_isReloading) return;

            _isReloading = true;
            StartCoroutine(ReloadFinished());
        }

        private IEnumerator ReloadFinished()
        {
            yield return new WaitForSeconds(reloadTime);
            _bulletsLeft = magazineSize;
            _bulletsShot = 0;
            _isReloading = false;
        }
    }
}
