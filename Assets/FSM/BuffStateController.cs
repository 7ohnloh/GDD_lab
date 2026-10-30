using System.Collections;
using UnityEngine;

// Second FSM on Mario, separate from MarioStateController: handles timed buffs (the star).
// Default --(SuperStar collected)--> Invincible --(after a few seconds)--> Default
public class BuffStateController : StateController
{
    public PowerupType currentBuffType = PowerupType.Default;
    public AudioSource backgroundMusic;   // paused while the star music plays
    public AudioSource starmanMusic;      // 05-starman
    public float colourCycleSpeed = 4f;   // rainbow cycles per second

    public bool isStarman = false;        // PlayerMovement checks this: touching an enemy kills it
    private SpriteRenderer spriteRenderer;
    private Coroutine rainbow;

    public override void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        base.Start();
        GameRestart(); // clear buff in the beginning, go to start state
    }

    // response to OnGameRestart
    public void GameRestart()
    {
        currentBuffType = PowerupType.Default;
        TransitionToState(startState); // leaving Invincible runs ExitStarman
    }

    public void SetBuff(PowerupType buff)
    {
        currentBuffType = buff;
    }

    // called by StartStarmanAction when entering the Invincible buff state
    public void StartStarman()
    {
        isStarman = true;
        if (rainbow != null) StopCoroutine(rainbow);
        rainbow = StartCoroutine(RainbowColours());
        if (backgroundMusic != null) backgroundMusic.Pause();
        if (starmanMusic != null) starmanMusic.Play();
    }

    // called by ExitStarmanAction when leaving the Invincible buff state
    public void ExitStarman()
    {
        isStarman = false;
        if (rainbow != null) StopCoroutine(rainbow);
        rainbow = null;
        if (spriteRenderer != null) spriteRenderer.color = Color.white;
        if (starmanMusic != null) starmanMusic.Stop();
        if (backgroundMusic != null) backgroundMusic.UnPause();
    }

    private IEnumerator RainbowColours()
    {
        while (true)
        {
            float hue = (Time.time * colourCycleSpeed) % 1f;
            spriteRenderer.color = Color.HSVToRGB(hue, 0.6f, 1f);
            yield return null;
        }
    }
}
