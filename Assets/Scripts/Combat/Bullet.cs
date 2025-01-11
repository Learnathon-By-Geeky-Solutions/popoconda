using UnityEngine;

namespace Combat
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private int damage = 10;
        [SerializeField] private float lifeTime = 3f;
        private Vector3 _startPosition;
        private const float MinTravelDistance = 0.5f;

        void Start()
        {
            _startPosition = transform.position; // Store the initial position of the bullet
            Destroy(gameObject, lifeTime);
        }

        void OnTriggerEnter(Collider collide)
        {
            float travelDistance = Vector3.Distance(_startPosition, transform.position);

            if (travelDistance >= MinTravelDistance)
            {
                Health health = collide.gameObject.GetComponent<Health>();
                if (health != null)
                {
                    health.TakeDamage(damage);
                }
            }

            Destroy(gameObject); // Destroy the bullet regardless of damage dealt
        }
    }
}