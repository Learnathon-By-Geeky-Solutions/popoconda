using UnityEngine;
using UnityEngine.SceneManagement;
using Studio23.SS2.AuthSystem.Core;

namespace Game
{
    public class SplashScreenLoader : MonoBehaviour
    {
        private void Start()
        {
            // Wait until AuthenticationManager is initialized
            if (AuthenticationManager.Instance != null)
            {
                AuthenticationManager.Instance.OnAuthSuccess.AddListener(LoadSplashScreen);
            }
            else
            {
                Debug.LogError("AuthenticationManager Instance is null. Make sure it's in the scene and initialized.");
            }
        }

        private void OnDisable()
        {
            if (AuthenticationManager.Instance != null)
            {
                AuthenticationManager.Instance.OnAuthSuccess.RemoveListener(LoadSplashScreen);
            }
        }

        private static void LoadSplashScreen(int result)
        {
            SceneManager.LoadScene(result + 1);
        }
    }
}