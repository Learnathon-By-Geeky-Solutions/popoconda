using UnityEngine;

namespace Weapon
{
    [CreateAssetMenu(fileName = "GunData", menuName = "Scriptable Objects/GunData")]

    public class GunData : ScriptableObject
    {
        public float spread;
        public float timeBetweenShooting;
        public int magazineSize;
        public int bulletsPerTap;
        public float reloadTime;
        public float bulletSpeed;
        public int damage;
    }
}