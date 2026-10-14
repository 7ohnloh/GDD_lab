using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI gameOverScoreText;
    public TextMeshProUGUI highscoreText;
    public GameObject gameOverPanel;
    public GameObject restartButton; // the always-visible one, not the one in the panel
    public GameObject pauseButton;
    public IntVariable gameScore;

    public void GameStart()
    {
        // hide gameover panel
        gameOverPanel.SetActive(false);
        restartButton.SetActive(true);
        pauseButton.SetActive(true);
    }

    public void SetScore(int score)
    {
        scoreText.text = "Score: " + score.ToString();
        gameOverScoreText.text = "Score: " + score.ToString();
        highscoreText.text = "TOP- " + gameScore.previousHighestValue.ToString("D6");
    }

    public void GameOver()
    {
        gameOverPanel.SetActive(true);
        restartButton.SetActive(false);
        pauseButton.SetActive(false);
    }
}
