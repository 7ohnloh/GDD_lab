using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyMovement : MonoBehaviour
{
    private float originalX;
    private float maxOffset = 5.0f;
    private float enemyPatroltime = 2.0f;
    private int moveRight = -1;
    private Vector2 velocity;
    private Rigidbody2D enemyBody;

    public Vector3 startPosition;

    // stomp
    public UnityEvent<int> stomped;
    private Animator goombaAnimator;
    private Collider2D goombaCollider;
    private bool isStomped = false;

    private SpriteRenderer goombaSprite;

    void Start()
    {
        enemyBody = GetComponent<Rigidbody2D>();
        goombaAnimator = GetComponent<Animator>();
        goombaCollider = GetComponent<Collider2D>();
        originalX = transform.position.x;
        startPosition = transform.localPosition;
        ComputeVelocity();
        goombaSprite = GetComponent<SpriteRenderer>();
    }

    void ComputeVelocity()
    {
        velocity = new Vector2((moveRight) * maxOffset / enemyPatroltime, 0);
    }

    void Movegoomba()
    {
        enemyBody.MovePosition(enemyBody.position + velocity * Time.fixedDeltaTime);
    }

    void FixedUpdate()
    {
        if (isStomped) return; // squashed goombas don't walk

        if (Mathf.Abs(enemyBody.position.x - originalX) < maxOffset)
        {
            Movegoomba();
        }
        else
        {
            moveRight *= -1;
            ComputeVelocity();
            Movegoomba();
        }
    }

    public void Stomp()
    {
        if (isStomped) return;
        isStomped = true;
        goombaCollider.enabled = false;
        goombaAnimator.SetTrigger("onStomped");
        stomped.Invoke(1);
        StartCoroutine(HideAfterDelay());
    }


    public void GameRestart()
    {
        StopAllCoroutines();
        goombaSprite.enabled = true;
        transform.localPosition = startPosition;
        originalX = transform.position.x;
        moveRight = -1;
        ComputeVelocity();

        if (isStomped)
        {
            isStomped = false;
            goombaCollider.enabled = true;
            // an inactive Animator resets to GoombaWalk by itself when re-enabled
            if (goombaAnimator.isActiveAndEnabled)
                goombaAnimator.SetTrigger("gameRestart");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.gameObject.name);
    }


    IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);
        goombaSprite.enabled = false;
    }
}