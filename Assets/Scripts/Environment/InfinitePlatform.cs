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
        [SerializeField] private float resetHeight = 20f;  // Height where platform will reset to
        [SerializeField] private float bottomLimit = 10f;  // Y position after which platform resets

        private GameObject[] platforms;

        private void Start()
        {
            platforms = new[] { platformObject1, platformObject2, platformObject3 };
            InfinitePlatformAsync().Forget();
        }
        
        private async UniTaskVoid InfinitePlatformAsync()
        {
            while (true)
            {
                foreach (var platform in platforms)
                {
                    platform.transform.position -= new Vector3(0f, moveSpeed * Time.deltaTime, 0f);

                    if (platform.transform.position.y <= bottomLimit)
                    {
                        float highestY = GetHighestPlatformY();
                        platform.transform.position = new Vector3(
                            platform.transform.position.x, 
                            highestY + resetHeight, 
                            platform.transform.position.z
                        );
                    }
                }
                
                await UniTask.Yield(); // wait for next frame
            }
        }

        private float GetHighestPlatformY()
        {
            float highest = float.MinValue;
            foreach (var platform in platforms)
            {
                if (platform.transform.position.y > highest)
                {
                    highest = platform.transform.position.y;
                }
            }
            return highest;
        }
    }
}
