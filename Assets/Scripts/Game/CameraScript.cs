using UnityEngine;

namespace Game
{
    public class CameraScript : MonoBehaviour
    {
        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
