using UnityEngine;

namespace Game
{
    public class MainMenuLoader : MonoBehaviour
    {
        public delegate void StatEvent();
        public static event StatEvent MainMenuEvent;
        private void Start()
        {
            MainMenuEvent?.Invoke();
        }
    }
}
