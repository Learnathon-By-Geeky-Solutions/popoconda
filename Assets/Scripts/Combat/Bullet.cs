using UnityEngine;

namespace Combat
{
    public class Bullet : MonoBehaviour
    {
        private float _speed;
        private Vector3 _direction;
        private Rigidbody _rigidbody;
        private int _damageAmount;
        
        public delegate void HitHandler(int damage, GameObject hitObject);
        public static event HitHandler OnBulletHit;
        
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            Destroy(gameObject, 3f);
        }
        
        public void SetSpeed(float bulletSpeed)
        {
            _speed = bulletSpeed;
        }
        
        public void SetDirection(Vector3 bulletDirection)
        {
            _direction = bulletDirection;
        }
        
        public void SetDamageAmount(int damage)
        {
            _damageAmount = damage;
        }
        
        private void FixedUpdate()
        {
            _rigidbody.linearVelocity = _direction * _speed;
        }

        private void OnTriggerEnter(Collider other)
        {
            OnBulletHit?.Invoke(_damageAmount, other.gameObject);
            Destroy(gameObject);
        }
    }
}