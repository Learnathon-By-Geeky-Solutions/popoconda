using UnityEngine;

namespace Characters
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] protected GameObject player;
        [SerializeField] protected GameObject gun;
        
        public Vector3 directionToPlayer;
        public float rotationZ;
        // player distance
        public float distanceToPlayer;
        
        public void GetPlayerPosition()
        {
            distanceToPlayer = Vector3.Distance(player.transform.position, gun.transform.position);
            directionToPlayer = player.transform.position - gun.transform.position;
            rotationZ = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        }
    }
}


