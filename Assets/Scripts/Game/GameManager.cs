using UnityEngine;
using Characters;


namespace Game
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager Instance { get; set; }
        
        private static Transform _playerTransform;
                
        public delegate void GameResult();
        public static event GameResult WinEvent;
        public static event GameResult LoseEvent;
        
        public static event GameResult DisableHudEvent;
        

        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            Enemy.OnBossDeath += Win;
            Player.OnPlayerDeath += Lose;
        }

        private void OnDisable()
        {
            Enemy.OnBossDeath -= Win;
            Player.OnPlayerDeath -= Lose;
        }
        
        public static void SetPlayerTransform(Transform playerTransform)
        {
            if (_playerTransform == null)
            {
                _playerTransform = playerTransform;
            }
        }
        
        public static Vector3 GetPlayerPosition()
        {
            return _playerTransform.position;
        }

        private void Win()
        {
            WinEvent?.Invoke();
            DisableHudEvent?.Invoke();
        }

        private void Lose()
        {
            LoseEvent?.Invoke();
            DisableHudEvent?.Invoke();
        }

    }
}