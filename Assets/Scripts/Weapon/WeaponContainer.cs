using Characters;
using Interface;
using UnityEngine;

namespace Weapon
{
    public class WeaponContainer : MonoBehaviour, IInteractable
    {
        [SerializeField] private GameObject weaponHolder;
        
        private GameObject _currentWeaponPrefab;
        
        [SerializeField] private GameObject[] weaponPrefab;
        
        private int _currentWeaponIndex;
        
        // Delegate for weapon change
        public delegate void StatEventWithInt(int state);
        public static event StatEventWithInt OnWeaponEquip;
        
        public void Interact()
        {
            if (_currentWeaponPrefab == null)
            {
                Debug.Log("No weapon available in the container.");
                return;
            }
            
            EquipWeapon();
        }

        private void OnEnable()
        {
            PlayerController.OnBossStateChange += ChangeContainedWeapon;
            Hero.OnHeroDeath += EquipPistol;
        }
        
        private void OnDisable()
        {
            PlayerController.OnBossStateChange -= ChangeContainedWeapon;
            Hero.OnHeroDeath -= EquipPistol;
        }

        private void ChangeContainedWeapon(int state)
        {
            if (state < weaponPrefab.Length)
            {
                _currentWeaponPrefab = weaponPrefab[state];
                _currentWeaponIndex = state;
                Debug.Log("Weapon unlocked: " + _currentWeaponPrefab.name);
            }
            else
            {
                Debug.Log("No weapon available for this state.");
                _currentWeaponPrefab = null;
            }
        }

        private void EquipWeapon()
        {
            // Destroy the current weapon the player is holding
            foreach (Transform child in weaponHolder.transform)
            {
                Destroy(child.gameObject);
            }

            // Instantiate a new weapon from the prefab
            GameObject equippedWeapon = Instantiate(_currentWeaponPrefab, weaponHolder.transform);
            equippedWeapon.transform.SetParent(weaponHolder.transform, false);
            
            OnWeaponEquip?.Invoke(_currentWeaponIndex);

            Debug.Log("Equipped Weapon: " + equippedWeapon.name);
        }

        private void EquipPistol()
        {
            // Destroy the current weapon the player is holding
            foreach (Transform child in weaponHolder.transform)
            {
                Destroy(child.gameObject);
            }

            // Equip Pistol after Hero Death
            GameObject equippedWeapon = Instantiate(weaponPrefab[0], weaponHolder.transform);
            equippedWeapon.transform.SetParent(weaponHolder.transform, false);
            
            OnWeaponEquip?.Invoke(0);
        }
    }
    
}
