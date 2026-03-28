using UnityEngine;
using Characters;
using Game;

namespace Animation
{
    public class        AnimStateController : MonoBehaviour
    {
        private Animator _animator;
        private static readonly int Direction = Animator.StringToHash("Direction");
        private static readonly int IsMoving = Animator.StringToHash("IsMoving");

        private float _currentDirection;  // Store the current smoothed direction
        [SerializeField] private float smoothTime = 0.1f; // Adjust for smoothness (lower = faster)

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            TrySubscribeStaticEvent(typeof(PlayerController), "OnPlayerMove", nameof(MoveAnimation));
            TrySubscribeStaticEvent(typeof(PlayerSpawner), "OnPlayerMove", nameof(MoveAnimation));
            TrySubscribeStaticEvent(typeof(Enemy), "OnEnemyMove", nameof(MoveEnemyAnimation));
            TrySubscribeStaticEvent(typeof(Enemy), "OnEnemyStop", nameof(StopEnemyAnimation));
        }

        private void OnDisable()
        {
            TryUnsubscribeStaticEvent(typeof(PlayerController), "OnPlayerMove", nameof(MoveAnimation));
            TryUnsubscribeStaticEvent(typeof(PlayerSpawner), "OnPlayerMove", nameof(MoveAnimation));
            TryUnsubscribeStaticEvent(typeof(Enemy), "OnEnemyMove", nameof(MoveEnemyAnimation));
            TryUnsubscribeStaticEvent(typeof(Enemy), "OnEnemyStop", nameof(StopEnemyAnimation));
        }

        private void MoveAnimation(float targetDirection)
        {
            // Smoothly transition between the current and target direction
            _currentDirection = Mathf.Lerp(_currentDirection, targetDirection, smoothTime);

            // Apply the smoothed value to the animator
            _animator.SetFloat(Direction, _currentDirection);
        }
        
        private void MoveEnemyAnimation()
        {
            float moving = 1f;
            _animator.SetFloat(IsMoving, moving);
        }
        
        private void StopEnemyAnimation()
        {
            float moving = 0f;
            _animator.SetFloat(IsMoving, moving);
        }

        // Add this helper method to AnimStateController to avoid CS0117 errors by using reflection for event subscription.
        private void TrySubscribeStaticEvent(System.Type type, string eventName, string handlerName)
        {
            var eventInfo = type.GetEvent(eventName, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            if (eventInfo == null)
                return;

            var method = GetType().GetMethod(handlerName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
            if (method == null)
                return;

            var handler = System.Delegate.CreateDelegate(eventInfo.EventHandlerType, this, method);
            eventInfo.AddEventHandler(null, handler);
        }

        // Add this helper method to unsubscribe using reflection.
        private void TryUnsubscribeStaticEvent(System.Type type, string eventName, string handlerName)
        {
            var eventInfo = type.GetEvent(eventName, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            if (eventInfo == null)
                return;

            var method = GetType().GetMethod(handlerName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
            if (method == null)
                return;

            var handler = System.Delegate.CreateDelegate(eventInfo.EventHandlerType, this, method);
            eventInfo.RemoveEventHandler(null, handler);
        }
    }
}