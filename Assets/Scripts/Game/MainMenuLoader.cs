using UnityEngine;

namespace Game
{
    public class MainMenuLoader : MonoBehaviour
    {
        public delegate void StatEvent();
        public static event StatEvent MainMenuEvent;
        
        public static void LoadMainMenu()
        {
            MainMenuEvent?.Invoke();
        }
    }
}
