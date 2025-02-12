using UnityEngine;
using Characters;

namespace Animation
{
    public class AnimStateController : MonoBehaviour
    {
        private Animator _animator;
        private static readonly int Direction = Animator.StringToHash("Direction");

        private float _currentDirection;  // Store the current smoothed direction
        [SerializeField] private float smoothTime = 0.1f; // Adjust for smoothness (lower = faster)

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            PlayerController.OnPlayerMove += MoveAnimation;
        }

        private void OnDisable()
        {
            PlayerController.OnPlayerMove -= MoveAnimation;
        }

        private void MoveAnimation(float targetDirection)
        {
            // Smoothly transition between the current and target direction
            _currentDirection = Mathf.Lerp(_currentDirection, targetDirection, smoothTime);

            // Apply the smoothed value to the animator
            _animator.SetFloat(Direction, _currentDirection);
        }
    }
}