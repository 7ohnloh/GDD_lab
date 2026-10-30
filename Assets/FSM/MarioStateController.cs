using System;
using System.Collections;
using UnityEngine;

public class MarioStateController : StateController
{
    public PowerupType currentPowerupType = PowerupType.Default;
    public MarioState shouldBeNextState = MarioState.Default;
    public float flickerInterval = 0.1f;   // seconds between blinks while invincible
    private SpriteRenderer spriteRenderer;

    public override void Start()
    {
        base.Start();
        GameRestart(); // clear powerup in the beginning, go to start state
    }

    // response to OnGameRestart
    public void GameRestart()
    {
        // clear powerup
        currentPowerupType = PowerupType.Default;
        // remove fireballs still flying, so they can't hit the respawned goombas
        foreach (GameObject fireball in GameObject.FindGameObjectsWithTag("Fireball"))
            Destroy(fireball);
        // set the start state
        TransitionToState(startState);
    }

    // called by ActionManager when Z is pressed; runs this state's Attack actions
    public void Fire()
    {
        this.currentState.DoEventTriggeredActions(this, ActionType.Attack);
    }

    public void SetPowerup(PowerupType i)
    {
        currentPowerupType = i;
    }

    public void SetRendererToFlicker()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(BlinkSpriteRenderer());
    }

    private IEnumerator BlinkSpriteRenderer()
    {
        // blink while we're still in the invincible state
        while (string.Equals(currentState.name, "InvincibleSmallMario", StringComparison.OrdinalIgnoreCase))
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(flickerInterval);
        }
        spriteRenderer.enabled = true;
    }
}
