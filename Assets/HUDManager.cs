using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI gameOverScoreText;
    public GameObject gameOverPanel;
    public GameObject restartButton; // the always-visible one, not the one in the panel

    public void GameStart()
    {
        // hide gameover panel
        gameOverPanel.SetActive(false);
        restartButton.SetActive(true);
    }

    public void SetScore(int score)
    {
        scoreText.text = "Score: " + score.ToString();
        gameOverScoreText.text = "Score: " + score.ToString();
    }

    public void GameOver()
    {
        gameOverPanel.SetActive(true);
        restartButton.SetActive(false);
    }
}