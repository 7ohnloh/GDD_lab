using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    // events
    public UnityEvent gameStart;
    public UnityEvent gameRestart;
    public UnityEvent<int> scoreChange;
    public UnityEvent gameOver;

    // score lives in a ScriptableObject so it survives scene loads and keeps the highscore
    public IntVariable gameScore;

    void Awake()
    {
        gameScore.LoadHighestValue();
    }

    void Start()
    {
        gameStart.Invoke();
        Time.timeScale = 1.0f;
        gameScore.Value = 0;
        SetScore(gameScore.Value);
    }

    public void GameRestart()
    {
        // reset score
        gameScore.Value = 0;
        SetScore(gameScore.Value);
        gameRestart.Invoke();
        Time.timeScale = 1.0f;
    }

    public void IncreaseScore(int increment)
    {
        gameScore.ApplyChange(increment);
        SetScore(gameScore.Value);
    }

    public void SetScore(int score)
    {
        scoreChange.Invoke(score);
    }

    public void GameOver()
    {
        PlayerPrefs.Save();
        Time.timeScale = 0.0f;
        gameOver.Invoke();
    }
}
