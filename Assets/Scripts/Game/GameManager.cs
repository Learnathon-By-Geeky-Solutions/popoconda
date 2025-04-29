using System.Threading.Tasks;
using UnityEngine;
using Characters;
using Cutscene;
using Studio23.SS2.SaveSystem.Core;


namespace Game
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager Instance { get; set; }
        
        private static Transform _playerTransform;
        private bool _onVerticalPlatform;
        
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
            _onVerticalPlatform = false;
            Hero.OnHeroDeath += Win;
            Player.OnPlayerDeath += Lose;
            CutsceneManager.OnVerticalPlatformEvent += () => _onVerticalPlatform = true;
        }

        private void OnDisable()
        {
            Hero.OnHeroDeath -= Win;
            Player.OnPlayerDeath -= Lose;
        }
        
        public static void SetPlayerTransform(Transform playerTransform)
        {
            if (_playerTransform == null)
            {
                Debug.Log("Player Transform Set");
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
            if(!Instance._onVerticalPlatform) return;
            
            WinEvent?.Invoke();
            DisableHudEvent?.Invoke();
            ResetGameState();
        }

        private static void Lose()
        {
            LoseEvent?.Invoke();
            DisableHudEvent?.Invoke();
            ResetGameState();
        }
        
        public static void ResetGameState()
        {
            if (Instance != null)
            {
                Instance._onVerticalPlatform = false;
            }
        }

    }
}