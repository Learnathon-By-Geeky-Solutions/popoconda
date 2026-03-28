using UnityEngine;
using Combat;
using Game;

public class EnemyScore : MonoBehaviour
{
    [SerializeField] private int scoreValue = 100;

    private Health _health;

    private void Awake()
    {
        _health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        if (_health != null)
            _health.OnDeath += GiveScore;
    }

    private void OnDisable()
    {
        if (_health != null)
            _health.OnDeath -= GiveScore;
    }

    private void GiveScore()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.AddScore(scoreValue);
    }
}