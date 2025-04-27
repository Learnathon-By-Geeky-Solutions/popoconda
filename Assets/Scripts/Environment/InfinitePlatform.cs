using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Environment
{
    public class InfinitePlatform : MonoBehaviour
    {
        [SerializeField] private GameObject platformObject1;
        [SerializeField] private GameObject platformObject2;
        [SerializeField] private GameObject platformObject3;
        [SerializeField] private float moveSpeed = 2f;

        private GameObject[] platforms;
        private float initialYPositionY;
        private float teleportPositionY;

        private void Start()
        {
            platforms = new[] { platformObject1, platformObject2, platformObject3 };
            initialYPositionY = platformObject1.transform.position.y;  // Store initial Y position of platform 1
            teleportPositionY = platformObject3.transform.position.y; // Store teleport position of platform 3
            
            InfinitePlatformAsync().Forget();
        }

        private async UniTaskVoid InfinitePlatformAsync()
        {
            while (true)
            {
                for (int i = 0; i < platforms.Length - 1; i++) // Loop through platforms, except the last one
                {
                    GameObject currentPlatform = platforms[i];
                    GameObject nextPlatform = platforms[i + 1];

                    // Move the current platform down
                    currentPlatform.transform.position -= new Vector3(0f, moveSpeed * Time.deltaTime, 0f);

                    // Check if the next platform reaches the initial position of the current platform
                    if (nextPlatform.transform.position.y <= initialYPositionY)
                    {
                        TeleportPlatform(currentPlatform);  // Teleport current platform
                        UpdatePositions();  // Update the variables
                    }
                }

                await UniTask.Yield(); // Yield control back to the main thread
            }
        }

        private void TeleportPlatform(GameObject platformToTeleport)
        {
            // Teleport the platform to the teleport position
            platformToTeleport.transform.position = new Vector3(
                platformToTeleport.transform.position.x,
                teleportPositionY,
                platformToTeleport.transform.position.z
            );
        }

        private void UpdatePositions()
        {
            // After teleporting, update the initial and teleport positions for the next platforms
            initialYPositionY = platforms[0].transform.position.y;
            teleportPositionY = platforms[2].transform.position.y;
        }
    }
}
