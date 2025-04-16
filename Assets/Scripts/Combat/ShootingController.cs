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
        private AudioSource gunShootSound;

        public delegate void StatEventWithInt(int value);
        public static event StatEventWithInt OnBulletCountChange;

        private void Awake()
        {
            _bulletsLeft = gunData.MagazineSize;
            gunShootSound = GetComponent<AudioSource>(); 
        }

        public void FireBullet(Vector3 direction)
        {
            if (!CanFire()) return;
            
            _canShoot = false;
            ShootBullets(direction);
            
            HandleFireRate();
            
            if (_bulletsLeft <= 0)
            {
                StartReloading();
            }
        }

        private bool CanFire()
        {
            return _canShoot && !_isReloading && _bulletsLeft > 0;
        }

        private void ShootBullets(Vector3 direction)
        {
            for (int i = 0; i < gunData.BulletsPerTap; i++)
            {
                Vector3 adjustedDirection = ApplyBulletSpread(direction);
                SpawnBullet(adjustedDirection);
                gunShootSound.Play();
                _bulletsLeft--;
                UpdateBulletCount();
            }
        }

        private Vector3 ApplyBulletSpread(Vector3 direction)
        {
            return direction + new Vector3(
                Random.Range(-gunData.Spread, gunData.Spread),
                Random.Range(-gunData.Spread, gunData.Spread),
                0);
        }

        private void SpawnBullet(Vector3 direction)
        {
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.LookRotation(direction));
            if (bullet.TryGetComponent(out Bullet bulletScript))
            {
                bulletScript.SetDirection(direction.normalized);
                bulletScript.SetSpeed(gunData.BulletSpeed);
                bulletScript.SetDamageAmount(gunData.Damage);
            }
        }

        // <<summary>>
        // Update the bullet count and invoke the event for UI to update the bullet count
        // <<summary>>
        private void UpdateBulletCount()
        {
            if (gameObject.CompareTag("Player"))
            {
                OnBulletCountChange?.Invoke(_bulletsLeft);
            }
        }

        private void HandleFireRate()
        {
            UniTask.Void(async () =>
            {
                await UniTask.Delay((int)(gunData.TimeBetweenShooting * 1000), cancellationToken: this.GetCancellationTokenOnDestroy());
                if (this) _canShoot = true;
            });
        }

        private void StartReloading()
        {
            if (_isReloading) return;

            _isReloading = true;
            UniTask.Void(async () =>
            {
                await UniTask.Delay((int)(gunData.ReloadTime * 1000), cancellationToken: this.GetCancellationTokenOnDestroy());
                if (this)
                {
                    _bulletsLeft = gunData.MagazineSize;
                    UpdateBulletCount();
                    _isReloading = false;
                }
            });
        }
    }
}
