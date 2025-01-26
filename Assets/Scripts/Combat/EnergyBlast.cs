using UnityEngine;

namespace Combat
{
    public class EnergyBlast : MonoBehaviour
    {
        //[SerializeField] private GameObject blastCore;
        [SerializeField] private float blastRadius;
        [SerializeField] private int blastDamage;
        
        private Transform _playerTransform;
        
        public delegate void BlastEventWithDamage(int damage);
        public static event BlastEventWithDamage OnEnergyBlastHit;
        
        private void Awake()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                _playerTransform = player.transform;
            }
        }
        
        public void Blast()
        {
            if (_playerTransform == null)
            {
                Debug.LogWarning("Player not found, cannot blast.");
                return;
            }

            // Check if the player is within the blast radius
            float distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);
            if (distanceToPlayer <= blastRadius)
            {
                Debug.Log("Player in range, applying blast!");
                
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
