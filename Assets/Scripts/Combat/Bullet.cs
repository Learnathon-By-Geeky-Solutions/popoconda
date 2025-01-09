using UnityEngine;

namespace Combat
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float damage = 10f;
        [SerializeField] private float lifeTime = 3f;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Destroy(gameObject, lifeTime);
        }

        // On collision with another collider
        void OnTriggerEnter(Collider collide)
        {
            Health health = collide.gameObject.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }

}
