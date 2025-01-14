using UnityEngine;

namespace Characters
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] protected GameObject player;
        [SerializeField] protected GameObject gun;
        
        
        public Vector3 directionToPlayer;
        public float rotationZ;
        public float distanceToPlayer;
        
        public void GetPlayerPosition()
        {
            distanceToPlayer = Mathf.Abs(transform.position.x - player.transform.position.x);
            directionToPlayer = player.transform.position - gun.transform.position;
            rotationZ = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        }
        
        public void MoveTowardsPlayer()
        {
            
            if (distanceToPlayer > 12f)
            {
                Vector3 targetPosition = new Vector3(player.transform.position.x, transform.position.y, transform.position.z);
                
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, 1f * Time.deltaTime);
            }
        }
    }
}


