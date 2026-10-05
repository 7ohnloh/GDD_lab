using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    public AudioMixer mixer;

    void Start()
    {
        // snapshot transitions must keep running while Time.timeScale = 0 on game over
        mixer.updateMode = AudioMixerUpdateMode.UnscaledTime;
    }

    public void GameOver()
    {
        mixer.FindSnapshot("GameOver").TransitionTo(0.5f);
    }

    public void GameRestart()
    {
        mixer.FindSnapshot("Gameplay").TransitionTo(0.5f);
    }
}