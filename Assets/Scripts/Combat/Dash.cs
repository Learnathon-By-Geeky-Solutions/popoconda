using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace Combat
{
    public class Dash : MonoBehaviour
    {
        [Header("Dash Settings")]
        [SerializeField] private float dashDistance; 
        [SerializeField] private float dashSpeed; 
        [SerializeField] private LayerMask obstacleLayer;

        private bool _isDashing;

        public bool IsDashing => _isDashing; // Public property to check if the boss is dashing
        
        public async UniTask DashAsync(float direction, CancellationToken cancellationToken)
        {
            if (_isDashing) return; // Prevent overlapping dashes

            _isDashing = true;
            Debug.Log("Boss is dashing!");

            // Calculate dash target position (only X-axis)
            Vector3 dashDirection = new Vector3(direction, 0, 0); // Restrict movement to X-axis
            Vector3 dashTargetPosition = transform.position + dashDirection * dashDistance;

            // Check for obstacles in the dash path
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dashDirection, dashDistance, obstacleLayer);
            if (hit.collider != null)
            {
                // Adjust the dash target position to stop before hitting the obstacle
                dashTargetPosition = hit.point - (Vector2)dashDirection * 0.5f; // Add a small offset to avoid collision
            }

            // Perform the dash
            float startTime = Time.time;
            float dashDuration = dashDistance / dashSpeed;
            Vector3 startPosition = transform.position;

            while (Time.time - startTime < dashDuration && !cancellationToken.IsCancellationRequested)
            {
                // Only update the X position
                float newX = Mathf.Lerp(startPosition.x, dashTargetPosition.x, (Time.time - startTime) / dashDuration);
                transform.position = new Vector3(newX, transform.position.y, transform.position.z);
                await UniTask.Yield(cancellationToken);
            }

            // Ensure the boss is exactly at the target position after the dash
            transform.position = new Vector3(dashTargetPosition.x, transform.position.y, transform.position.z);

            _isDashing = false;
            Debug.Log("Boss finished dashing!");
        }
    }
}