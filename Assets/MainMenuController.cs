using UnityEngine;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    public IntVariable gameScore;
    public TextMeshProUGUI highscoreText;

    void Start()
    {
        gameScore.LoadHighestValue();
        ShowHighscore();
    }

    // hook this up to the Reset Highscore button's OnClick
    public void ResetHighscore()
    {
        gameScore.ResetHighestValue();
        ShowHighscore();
    }

    void ShowHighscore()
    {
        highscoreText.text = "TOP- " + gameScore.previousHighestValue.ToString("D6");
    }
}
