using UnityEngine;

namespace Characters
{
    public class Enemy : MonoBehaviour
    {
        public void MoveTowardsPlayer(Vector3 direction, float distance)
        {
            if (distance >= 16f)
            {
                transform.position += new Vector3(direction.x * (0.3f * Time.deltaTime), 0, 0);
            }
        }

        
    }
}


