using System;
using Combat;
using UnityEngine;

namespace Characters
{
    [Serializable]
    public class Player
    {
        [SerializeField] private GameObject playerGfx;
        [SerializeField] private GameObject gunRotatePoint;
        
        [SerializeField] private float moveSpeed;
        
        [SerializeField] private float flySpeed;
        [SerializeField] private float jetpackFuelMax;
        private float _jetpackFuel;
        [SerializeField] private float fuelConsumeRate;
        [SerializeField] private float fuelFillRate;
        
        public static event Health.StatEvent OnPlayerDeath;
        
        private bool _isJumping;
        public bool IsAlive { get; set; }
        
        // Constructor
        public void Initialize()
        {
            IsAlive = true;
            _jetpackFuel = jetpackFuelMax;
        }
        
        public void Die()
        {
            IsAlive = false;
            OnPlayerDeath?.Invoke();
            Debug.Log("PlayerDied");
        }
        
        // Public Properties
        
        public GameObject PlayerGfx
        {
            get { return playerGfx; }
            set { playerGfx = value; }
        }
        
        public GameObject GunRotatePoint
        {
            get { return gunRotatePoint; }
            set { gunRotatePoint = value; }
        }
        public float MoveSpeed
        {
            get { return moveSpeed; }
            set { moveSpeed = value; }
        }

        public float FlySpeed
        {
            get { return flySpeed; }
            set { flySpeed = value; }
        }

        public float JetpackFuel
        {
            get { return _jetpackFuel; }
            set { _jetpackFuel = Mathf.Clamp(value, 0, jetpackFuelMax); }
        }
        public float FuelConsumeRate
        {
            get { return fuelConsumeRate; }
            set { fuelConsumeRate = value; }
        }
        public float FuelFillRate
        {
            get { return fuelFillRate; }
            set { fuelFillRate = value; }
        }
        
        public float JetpackFuelMax => jetpackFuelMax;  // Getter for max jetpack fuel
    }
}