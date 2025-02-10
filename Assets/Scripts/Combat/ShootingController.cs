using UnityEngine;
using Cysharp.Threading.Tasks;
using Weapon;

namespace Combat
{
    public class ShootingController : MonoBehaviour
    {
        [SerializeField] private Transform bulletSpawnPoint;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private GunData gunData;
        
        private int _bulletsLeft;
        private bool _isReloading;
        private bool _canShoot = true;

        public delegate void StatEventWithInt(int value);
        public static event StatEventWithInt OnBulletCountChange;

        private void Awake()
        {
            _bulletsLeft = gunData.MagazineSize;
        }

        public void FireBullet(Vector3 direction)
        {
            if (!_canShoot || _isReloading || _bulletsLeft <= 0) return; // Prevent shooting if conditions aren't met

            _canShoot = false;

            for (int i = 0; i < gunData.BulletsPerTap; i++)
            {
                // Apply spread to the direction
                Vector3 adjustedDirection = direction + new Vector3(
                    Random.Range(-gunData.Spread, gunData.Spread),
                    Random.Range(-gunData.Spread, gunData.Spread),
                    0);

                GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.LookRotation(adjustedDirection));

                Bullet bulletScript = bullet.GetComponent<Bullet>();
                if (bulletScript != null)
                {
                    bulletScript.SetDirection(adjustedDirection.normalized); // Pass the normalized direction
                    bulletScript.SetSpeed(gunData.BulletSpeed);
                    bulletScript.SetDamageAmount(gunData.Damage);
                }

                _bulletsLeft--;
                if (gameObject.CompareTag("Player")) OnBulletCountChange?.Invoke(_bulletsLeft);
            }

            // Delay between shooting to control fire rate
            UniTask.Void(async () =>
            {
                await UniTask.Delay((int)(gunData.TimeBetweenShooting * 1000), cancellationToken: this.GetCancellationTokenOnDestroy());
                if (this) _canShoot = true;
            });

            // If no bullets left, trigger reloading
            if (_bulletsLeft <= 0)
            {
                StartReloading();
            }
        }

        private void StartReloading()
        {
            if (_isReloading) return; // Prevent multiple reloads at the same time

            _isReloading = true;
            UniTask.Void(async () =>
            {
                await UniTask.Delay((int)(gunData.ReloadTime * 1000), cancellationToken: this.GetCancellationTokenOnDestroy());
                if (this)
                {
                    _bulletsLeft = gunData.MagazineSize;
                    if (gameObject.CompareTag("Player")) OnBulletCountChange?.Invoke(_bulletsLeft);
                    _isReloading = false;
                }
            });
        }
    }
}
