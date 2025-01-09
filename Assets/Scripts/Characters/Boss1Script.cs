using UnityEngine;

namespace Characters
{
    public class Boss1Script : MonoBehaviour
    {
        public GameObject player;
        private GameObject _gunRotatePoint;
        public GameObject gun;
        public GameObject bulletPrefab;
        public float bulletSpeed;
        public float fireRate;
        private float _nextFireTime;

    
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _gunRotatePoint = gameObject;
        }
        // Update is called once per frame
        void Update()
        {
            // Get the direction to the player
            Vector3 directionToPlayer = player.transform.position - gun.transform.position;

            // Calculate the rotation angle
            float rotationZ = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

            // Rotate the gun towards the player
            _gunRotatePoint.transform.rotation  = Quaternion.Euler(0f, 0f, rotationZ);

            // Shoot at the player at regular intervals
            if (Time.time >= _nextFireTime)
            {
                FireBullet(directionToPlayer, rotationZ);
                _nextFireTime = Time.time + fireRate; // Set the time for the next shot
            }
        }

        void FireBullet(Vector3 direction, float rotationZ)
        {
            // Fire the bullet
            GameObject bullet = Instantiate(bulletPrefab, gun.transform.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody>().linearVelocity = direction.normalized * bulletSpeed;
            bullet.transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);
        }
    }
}
