using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    // events
    public SimpleGameEvent gameStart;      // OnGameStart
    public IntGameEvent scoreChange;       // OnScoreChange
    public SimpleGameEvent gameOver;

    // score lives in a ScriptableObject so it survives scene loads and keeps the highscore
    public IntVariable gameScore;

    void Awake()
    {
        gameScore.LoadHighestValue();
    }

    void Start()
    {
        gameStart.Raise();
        Time.timeScale = 1.0f;
        gameScore.Value = 0;
        SetScore(gameScore.Value);
    }

    // response to OnGameRestart
    public void GameRestart()
    {
        // reset score
        gameScore.Value = 0;
        SetScore(gameScore.Value);
        Time.timeScale = 1.0f;
    }

    // response to OnIncreaseScore
    public void IncreaseScore(int increment)
    {
        gameScore.ApplyChange(increment);
        SetScore(gameScore.Value);
    }

    public void SetScore(int score)
    {
        scoreChange.Raise(score);
    }

    // response to OnDamagePlayer
    public void GameOver()
    {
        PlayerPrefs.Save();
        Time.timeScale = 0.0f;
        gameOver.Raise();
    }
}
