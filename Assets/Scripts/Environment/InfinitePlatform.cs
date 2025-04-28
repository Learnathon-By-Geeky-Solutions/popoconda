using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Environment
{
    public class InfinitePlatform : MonoBehaviour
    {
        [SerializeField] private GameObject platformObject1;
        [SerializeField] private GameObject platformObject2;
        [SerializeField] private float moveSpeed = 2f;

        private GameObject[] platforms;
        private float initialYPosition;
        private float teleportYPosition;
        private float platformHeight;

        private void Start()
        {
            platforms = new[] { platformObject1, platformObject2 };
            
            // Store the initial positions
            initialYPosition = platformObject1.transform.position.y; 
            teleportYPosition = platformObject2.transform.position.y;
            
            // Calculate platform height (assuming both platforms have the same height)
            platformHeight = teleportYPosition - initialYPosition;
            
            InfinitePlatformAsync().Forget();
        }

        private async UniTaskVoid InfinitePlatformAsync()
        {
            while (true)
            {
                // Move both platforms downward
                foreach (var platform in platforms)
                {
                    Vector3 position = platform.transform.position;
                    position.y -= moveSpeed * Time.deltaTime;
                    platform.transform.position = position;
                    
                    // If platform goes below the initial position, teleport it to the top
                    if (position.y < initialYPosition)
                    {
                        // Find the highest platform's Y position
                        float highestY = FindHighestPlatformY();
                        
                        // Place this platform above the highest one with proper spacing
                        position.y = highestY + platformHeight;
                        platform.transform.position = position;
                    }
                }

                await UniTask.Yield(); // Yield control back to the main thread
            }
        }
        
        private float FindHighestPlatformY()
        {
            float highestY = float.MinValue;
            foreach (var platform in platforms)
            {
                if (platform.transform.position.y > highestY)
                {
                    highestY = platform.transform.position.y;
                }
            }
            return highestY;
        }
    }
}