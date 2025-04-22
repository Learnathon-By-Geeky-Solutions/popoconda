using UnityEngine;
using Weapon;

namespace Combat
{
    public class BossWeaponLoader : MonoBehaviour
    {
        [SerializeField] private ShootingController shootingController;
        [SerializeField] private GunData[] gunData;

        private void OnEnable()
        {
            WeaponContainer.OnWeaponEquip += ChangeGunData;
        }
        
        private void OnDisable()
        {
            WeaponContainer.OnWeaponEquip -= ChangeGunData;
        }
        
        private void ChangeGunData(int state)
        {
            if (state < gunData.Length)
            {
                shootingController.ChangeGunData(gunData[state]);
            }
            else
            {
                Debug.Log("No weapon available for this state.");
            }
        }
    }


}
