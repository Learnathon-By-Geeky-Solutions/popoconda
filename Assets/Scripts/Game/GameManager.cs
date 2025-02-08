using UnityEngine;
using Characters;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        private GameObject _player;
        private GameObject _enemy;
        private GameObject _hud;

        public delegate void GameResult();
        public static event GameResult WinEvent;
        public static event GameResult LoseEvent;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // Keep across scenes
            }
            else
            {
                Destroy(gameObject); // Destroy duplicate instances
            }
        }

        private void Start()
        {
            _player = GameObject.FindWithTag("Player");
            _enemy = GameObject.FindWithTag("Enemy");
            _hud = GameObject.FindWithTag("HUD");
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

        private void Win()
        {
            _player.SetActive(false);
            _enemy.SetActive(false);
            _hud.SetActive(false);
            WinEvent?.Invoke();
        }

        private void Lose()
        {
            _player.SetActive(false);
            _enemy.SetActive(false);
            _hud.SetActive(false);
            LoseEvent?.Invoke();
        }
    }
}