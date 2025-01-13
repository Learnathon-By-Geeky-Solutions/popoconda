using System.Collections;
using UnityEngine;

namespace Combat
{
    public class ShootingController : MonoBehaviour
    {
        [SerializeField] private GameObject bulletSpawnPoint;
        [SerializeField] private TrailRenderer bulletTrail;
        
        // bullet force
        [SerializeField] private float shootForce, upwardForce;
        
        // Gun Stats
        [SerializeField] private float timeBetweenShooting, spread, reloadTime, timeBetweenShots;
        [SerializeField] private int magazineSize, bulletsPerTap;
        
        // bool
        private bool _isReadyToShoot, _isReloading;
        
        private int _bulletsLeft, _bulletsShot;
        
        // points
        private Vector3 _shootDirection;

        private const float MinTravelDistance = 0.5f;
        [SerializeField] private int damageAmount = 10;
        
        
        private void Awake()
        {
            _bulletsLeft = magazineSize;
            _isReadyToShoot = true;
        }
        
        public void FireBullet(Vector3 direction, float rotationZ)
        {
            _shootDirection = direction;
            
            _bulletsShot = 0;
            if (_bulletsLeft == 0)
            {
                Reload();
            }
            
            if (_isReadyToShoot && _bulletsLeft > 0 && !_isReloading)
            {
                Shoot();
            }
        }
        
        private void Shoot()
        {
            _isReadyToShoot = false;
            _bulletsLeft--;
            _bulletsShot++;
    
            // Spread calculation
            float x = Random.Range(-spread, spread);
            float y = Random.Range(-spread, spread);
            Vector3 spreadVector = bulletSpawnPoint.transform.right * x + bulletSpawnPoint.transform.up * y;
    
            // Calculate shoot direction with spread
            Vector3 shootDirection = (_shootDirection + spreadVector).normalized;

            // Perform raycast
            RaycastHit hit;
            if (Physics.Raycast(bulletSpawnPoint.transform.position, shootDirection, out hit, Mathf.Infinity))
            {
        
                // Calculate the distance the bullet has traveled
                float travelDistance = Vector3.Distance(bulletSpawnPoint.transform.position, hit.point);
                
                // instantiate bullet trail
                TrailRenderer trail = Instantiate(bulletTrail, bulletSpawnPoint.transform.position, Quaternion.identity);
                
                // start a coroutine
                StartCoroutine(SpawnTrail(trail,hit));

                // Check if the bullet has traveled at least the minimum distance before dealing damage
                if (travelDistance >= MinTravelDistance)
                {
                    // Attempt to get the Health component from the collided object
                    Health health = hit.collider.GetComponent<Health>();
                    if (health)
                    {
                        // Deal damage to the object with the Health component
                        health.TakeDamage(damageAmount);
                    }
                }
            }

            // Check if we should continue shooting
            if (_bulletsShot < bulletsPerTap && _bulletsLeft > 0)
            {
                Invoke(nameof(Shoot), timeBetweenShooting);
            }
            else
            {
                InvokeRepeating(nameof(ResetShot), timeBetweenShots, timeBetweenShooting);
            }
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

        
        private void ResetShot()
        {
            CancelInvoke(nameof(ResetShot));
            _isReadyToShoot = true;
        }
        
        private void Reload()
        {
            if (_isReloading) return;
            
            _isReloading = true;
            Invoke(nameof(ReloadFinished), reloadTime);
        }
        
        private void ReloadFinished()
        {
            _bulletsLeft = magazineSize;
            _bulletsShot = 0;
            _isReloading = false;
        }
    }
}
