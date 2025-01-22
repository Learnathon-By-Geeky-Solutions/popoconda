using UnityEngine;
using UnityEngine.SceneManagement;
using Characters;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        private void OnEnable()
        {
            Enemy.OnBossDeath += LoadNextLevel;
        }
        
        private void OnDisable()
        {
            Enemy.OnBossDeath -= LoadNextLevel;
        }
        
        private static void LoadNextLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            Debug.Log("Loading next level");
        }
    }
}
