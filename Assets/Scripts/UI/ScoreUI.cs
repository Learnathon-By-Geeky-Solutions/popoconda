using UnityEngine;
using TMPro;
using Game;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    private void OnEnable()
    {
        ScoreManager.OnScoreChanged += UpdateScore;
    }

    private void OnDisable()
    {
        ScoreManager.OnScoreChanged -= UpdateScore;
    }

    private void UpdateScore(int score)
    {
        scoreText.text = "Score: " + score;
    }
}