using System.Threading.Tasks;
using UnityEngine;
using Characters;
using Studio23.SS2.SaveSystem.Core;


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
        
        private void Start()
        {
            InitializeSaveSystem();
        }

        private static async Task InitializeSaveSystem()
        {
            await SaveSystem.Instance.Initialize();
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
            if(_playerTransform == null)
            {
                return Vector3.zero;
            }
            return _playerTransform.position;
        }
        
        public static void ClearPlayerTransform()
        {
            _playerTransform = null;
        }

        private static void Win()
        {
            WinEvent?.Invoke();
            DisableHudEvent?.Invoke();
        }

        private static void Lose()
        {
            LoseEvent?.Invoke();
            DisableHudEvent?.Invoke();
        }
    }
}