using UnityEngine;
using Characters;


namespace Game
{
    public class GameManager : MonoBehaviour
    {
        private GameObject _player;
        private GameObject _enemy;
        private GameObject _hud;

        public delegate void GameResult();
        public static event GameResult WinEvent;
        public static event GameResult LoseEvent;
        

        private void Start()
        {
            _player = GameObject.FindWithTag("Player");
            _enemy = GameObject.FindWithTag("Enemy");
            _hud = GameObject.FindWithTag("HUD");

            if (_player == null) Debug.LogError("GameManager: No GameObject with tag 'Player' found!");
            if (_enemy == null) Debug.LogError("GameManager: No GameObject with tag 'Enemy' found!");
            if (_hud == null) Debug.LogError("GameManager: No GameObject with tag 'HUD' found!");
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
            if (_player != null) _player.SetActive(false);
            if (_enemy != null) _enemy.SetActive(false);
            if (_hud != null) _hud.SetActive(false);
            WinEvent?.Invoke();
        }

        private void Lose()
        {
            if (_player != null) _player.SetActive(false);
            if (_enemy != null) _enemy.SetActive(false);
            if (_hud != null) _hud.SetActive(false);
            LoseEvent?.Invoke();
        }

    }
}