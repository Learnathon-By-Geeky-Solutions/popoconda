using UnityEngine;
using UnityEngine.SceneManagement;
using Characters;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        private void OnEnable()
        {
            Boss1Script.OnBoss1Death += LoadNextLevel;
        }
        
        private void OnDisable()
        {
            Boss1Script.OnBoss1Death -= LoadNextLevel;
        }
        
        private static void LoadNextLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            Debug.Log("Loading next level");
        }
    }
}
