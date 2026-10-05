using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 10;
    public float maxSpeed = 20;
    public float upSpeed = 10;
    private bool onGroundState = true;
    private Rigidbody2D marioBody;

    private SpriteRenderer marioSprite;
    private bool faceRightState = true;

    public GameManager gameManager;
    public AudioSource marioAudio;

    private bool alive = true;
    private bool moving = false;
    private bool jumpedState = false;

    void Start()
    {
        Application.targetFrameRate = 30;
        marioBody = GetComponent<Rigidbody2D>();
        marioSprite = GetComponent<SpriteRenderer>();
    }

    void FlipMarioSprite(int value)
    {
        if (value == -1 && faceRightState)
        {
            faceRightState = false;
            marioSprite.flipX = true;
        }
        else if (value == 1 && !faceRightState)
        {
            faceRightState = true;
            marioSprite.flipX = false;
        }
    }

    void FixedUpdate()
    {
        if (alive && moving)
        {
            Move(faceRightState == true ? 1 : -1);
        }
    }

    void Move(int value)
    {
        Vector2 movement = new Vector2(value, 0);
        // check if it doesn't go beyond maxSpeed
        if (marioBody.linearVelocity.magnitude < maxSpeed)
            marioBody.AddForce(movement * speed);
    }

    public void MoveCheck(int value)
    {
        if (value == 0)
        {
            moving = false;
        }
        else
        {
            FlipMarioSprite(value);
            moving = true;
            Move(value);
        }
    }

    public void Jump()
    {
        if (alive && onGroundState)
        {
            marioBody.AddForce(Vector2.up * upSpeed, ForceMode2D.Impulse);
            marioAudio.Play();
            onGroundState = false;
            jumpedState = true;
        }
    }

    public void JumpHold()
    {
        if (alive && jumpedState)
        {
            // jump higher
            marioBody.AddForce(Vector2.up * upSpeed * 30, ForceMode2D.Force);
            jumpedState = false;
        }
    }

    // Ground = layer 3, Enemies = layer 6, Obstacles = layer 7
    int collisionLayerMask = (1 << 3) | (1 << 6) | (1 << 7);

    void OnCollisionEnter2D(Collision2D col)
    {
        if (((collisionLayerMask & (1 << col.gameObject.layer)) > 0) && !onGroundState)
            onGroundState = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy") && alive)
        {
            bool falling = marioBody.linearVelocity.y < 0;
            bool above = transform.position.y > other.transform.position.y + 0.5f;

            if (falling && above)
            {
                // stomp: squash the goomba and bounce mario up a little
                other.GetComponent<EnemyMovement>().Stomp();
                marioBody.linearVelocity = new Vector2(marioBody.linearVelocity.x, 0);
                marioBody.AddForce(Vector2.up * upSpeed * 0.5f, ForceMode2D.Impulse);
            }
            else
            {
                Debug.Log("Collided with goomba!");
                alive = false;
                gameManager.GameOver();
            }
        }
    }

    public void GameRestart()
    {
        // reset position
        marioBody.transform.position = new Vector3(-22.46f, -1.45f, 0.0f);
        marioBody.linearVelocity = Vector2.zero;
        // reset sprite direction
        faceRightState = true;
        marioSprite.flipX = false;
        moving = false;
        alive = true;
    }
}