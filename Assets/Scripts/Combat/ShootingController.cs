using UnityEngine;
using UnityEngine.InputSystem;

namespace Combat
{
    public class ShootingController : MonoBehaviour
    {
        public InputActionReference positionAction;

        private Camera _playerCamera;
        private Vector3 _mousePosition;
        [SerializeField] private GameObject player;
        [SerializeField] private GameObject gun;
        [SerializeField] private GameObject bulletPrefab;
        public float bulletSpeed;
        public float fireRate;
        private float _nextFireTime;

        void Start()
        {
            _playerCamera = Camera.main; // Simplified to use the main camera directly
        }

        void Update()
        {
            // Get the mouse position in screen space and convert it to world space
            Vector2 screenPosition = positionAction.action.ReadValue<Vector2>();
            Ray ray = _playerCamera.ScreenPointToRay(screenPosition);
            
            // Assuming the gun or player is on a specific plane (e.g., Z = 0)
            Plane gunPlane = new Plane(Vector3.forward, gun.transform.position);

            if (gunPlane.Raycast(ray, out float distance))
            {
                _mousePosition = ray.GetPoint(distance);
            }

            Vector3 difference = _mousePosition - gun.transform.position;
            float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);

            // Flip the Player GameObject based on the rotation
            if (rotationZ > 90 || rotationZ < -90)
            {
                player.transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                player.transform.localScale = new Vector3(1, 1, 1);
            }

            // Shoot only if the left mouse button is pressed and the fire rate timer has elapsed
            if (Mouse.current.leftButton.isPressed && Time.time >= _nextFireTime)
            {
                FireBullet(difference, rotationZ);
                _nextFireTime = Time.time + fireRate;
            }
        }

        void FireBullet(Vector3 direction, float rotationZ)
        {
            GameObject bullet = Instantiate(bulletPrefab, gun.transform.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody>().linearVelocity = direction.normalized * bulletSpeed;
            bullet.transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);
        }
    }
}
