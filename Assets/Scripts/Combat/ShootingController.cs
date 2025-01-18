using UnityEngine;
using Cysharp.Threading.Tasks; // Import UniTask

namespace Combat
{
    public class ShootingController : MonoBehaviour
    {
        [SerializeField] private Transform bulletSpawnPoint;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private float spread;
        [SerializeField] private float timeBetweenShooting;
        [SerializeField] private int magazineSize;
        [SerializeField] private int bulletsPerTap;
        [SerializeField] private float reloadTime;
        [SerializeField] private float bulletSpeed;
        [SerializeField] private int damage;

        private int _bulletsLeft;
        private bool _isReloading;
        private bool _canShoot = true;
        
        public delegate void StatEventWithInt(int value);
        public static event StatEventWithInt OnBulletCountChange;

        private void Awake()
        {
            _bulletsLeft = magazineSize;
        }
        
        public void FireBullet(Vector3 direction)
        {
            if (!_canShoot || _isReloading || _bulletsLeft <= 0) return; // Prevent shooting if conditions aren't met

            _canShoot = false;

            for (int i = 0; i < bulletsPerTap; i++)
            {
                // Apply spread to the direction
                Vector3 adjustedDirection = direction + new Vector3(
                    Random.Range(-spread, spread),
                    Random.Range(-spread, spread),
                    0);
                
                GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.LookRotation(adjustedDirection));
                
                Bullet bulletScript = bullet.GetComponent<Bullet>();
                if (bulletScript != null)
                {
                    bulletScript.SetDirection(adjustedDirection.normalized); // Pass the normalized direction
                    bulletScript.SetSpeed(bulletSpeed);
                    bulletScript.SetDamageAmount(damage);
                }

                _bulletsLeft--;
                if(gameObject.CompareTag("Player")) OnBulletCountChange?.Invoke(_bulletsLeft);
            }

            // Delay between shooting to control fire rate
            UniTask.Delay((int)(timeBetweenShooting * 1000)).ContinueWith(() => _canShoot = true);

            // If no bullets left, trigger reloading
            if (_bulletsLeft <= 0)
            {
                StartReloading();
            }
        }
        
        private async UniTask StartReloading()
        {
            if (_isReloading) return; // Prevent multiple reloads at the same time

            _isReloading = true;
            await UniTask.Delay((int)(reloadTime * 1000)); // Wait for reload time to complete
            _bulletsLeft = magazineSize;
            if(gameObject.CompareTag("Player")) OnBulletCountChange?.Invoke(_bulletsLeft);
            _isReloading = false;
        }
    }
}