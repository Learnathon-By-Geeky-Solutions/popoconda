using UnityEngine;
using Game;

namespace Combat
{
    public class EnergyBlast : MonoBehaviour
    {
        [SerializeField] private float blastRadius;
        [SerializeField] private int blastDamage;
        
        private Vector3 _playerPosition;
        
        public delegate void StatEvent();
        public delegate void StatEventWithInt(int damage);
        
        public static event StatEvent OnEnergyBlast;
        public static event StatEventWithInt OnEnergyBlastHit;
        
        private void Awake()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                _playerPosition = GameManager.GetPlayerPosition();
            }
        }
        
        public void Blast()
        {

            // Check if the player is within the blast radius
            float distanceToPlayer = Vector3.Distance(transform.position, _playerPosition);
            if (distanceToPlayer <= blastRadius)
            {
                Debug.Log("Player in range, applying blast!");
                
                OnEnergyBlast?.Invoke();
                // Apply damage to the player
                OnEnergyBlastHit?.Invoke(blastDamage);
            }
            else
            {
                Debug.Log("Player out of range, blast skipped.");
            }
        }
    }
    
}
