using UnityEngine;
using TMPro;

public class PauseButtonController : MonoBehaviour
{
    public AudioSource backgroundMusic;
    public AudioSource pauseSound;     // optional: smb_pause
    public TextMeshProUGUI label;

    private bool isPaused = false;

    void Start()
    {
        // unpause if the game is restarted while paused
        FindAnyObjectByType<GameManager>().gameRestart.AddListener(Resume);
        label.text = "PAUSE";
    }

    // hook this up to the button's OnClick
    public void ButtonClick()
    {
        if (isPaused) Resume();
        else Pause();
    }

    void Pause()
    {
        isPaused = true;
        Time.timeScale = 0.0f;
        backgroundMusic.Pause();
        if (pauseSound != null) pauseSound.Play();
        label.text = "RESUME";
    }

    void Resume()
    {
        isPaused = false;
        Time.timeScale = 1.0f;
        backgroundMusic.UnPause();
        label.text = "PAUSE";
    }
}
