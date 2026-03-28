using UnityEngine;
using System;

namespace Game
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance;

        private int _score;

        public static event Action<int> OnScoreChanged;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void AddScore(int value)
        {
            _score += value;
            OnScoreChanged?.Invoke(_score);
        }

        public int GetScore()
        {
            return _score;
        }
    }
}
