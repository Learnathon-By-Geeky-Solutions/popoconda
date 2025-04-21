using UnityEngine;

namespace Weapon
{
    [CreateAssetMenu(fileName = "GunData", menuName = "Scriptable Objects/GunData")]
    public class GunData : ScriptableObject
    {
        [SerializeField] private float spread;
        [SerializeField] private float timeBetweenShooting;
        [SerializeField] private int magazineSize;
        [SerializeField] private int bulletsPerTap;
        [SerializeField] private float reloadTime;
        [SerializeField] private float bulletSpeed;
        [SerializeField] private int damage;
        
        public float Spread => spread;
        public float TimeBetweenShooting => timeBetweenShooting;
        public int MagazineSize => magazineSize;
        public int BulletsPerTap => bulletsPerTap;
        public float ReloadTime => reloadTime;
        public float BulletSpeed => bulletSpeed;
        public int Damage => damage;
    }
}