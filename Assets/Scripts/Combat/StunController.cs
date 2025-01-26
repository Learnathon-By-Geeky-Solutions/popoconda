using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Combat
{
    public class StunController : MonoBehaviour
    {
        [Header("Stun Settings")]
        [SerializeField] private float stunDuration;
        [SerializeField] private float stunRadius;  
        
        public delegate void StunEvent(bool isStunned);
        
        public static event StunEvent OnStun;

        private Transform _playerTransform;

        private void Awake()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                _playerTransform = player.transform;
            }
        }

        public async UniTask Stun()
        {
            if (_playerTransform == null)
            {
                Debug.LogWarning("Player not found, cannot stun.");
                return;
            }

            // Check if the player is within the stun radius
            float distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);
            if (distanceToPlayer <= stunRadius)
            {
                Debug.Log("Player in range, applying stun!");
                
                OnStun?.Invoke(true);
                Debug.Log("Player stunned!");

                // Wait for the stun duration
                await UniTask.Delay((int)(stunDuration * 1000));
                
                OnStun?.Invoke(false);
                Debug.Log("Player un-stunned!");
            }
            else
            {
                Debug.Log("Player out of range, stun skipped.");
            }
        }

        private void OnDrawGizmos()
        {
            // Visualize the stun radius in the Scene view
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, stunRadius);
        }
    }
}
