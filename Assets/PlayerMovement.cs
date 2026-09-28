using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 10;
    public float maxSpeed = 20;
    public float upSpeed = 10;
    private bool onGroundState = true;
    private Rigidbody2D marioBody;

    private SpriteRenderer marioSprite;
    private bool faceRightState = true;

    public TextMeshProUGUI scoreText;
    public GameObject enemies;
    public JumpOverGoomba jumpOverGoomba;

    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverScoreText;
    public GameObject alwaysVisibleRestartButton;

    // new field
    public GameObject staticEnvironment;

    void Start()
    {
        Application.targetFrameRate = 30;
        marioBody = GetComponent<Rigidbody2D>();
        marioSprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (Input.GetKeyDown("a") && faceRightState)
        {
            faceRightState = false;
            marioSprite.flipX = true;
        }

        if (Input.GetKeyDown("d") && !faceRightState)
        {
            faceRightState = true;
            marioSprite.flipX = false;
        }
    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");

        if (Mathf.Abs(moveHorizontal) > 0)
        {
            Vector2 movement = new Vector2(moveHorizontal, 0);
            if (marioBody.linearVelocity.magnitude < maxSpeed)
                marioBody.AddForce(movement * speed);
        }

        if (Input.GetKeyUp("a") || Input.GetKeyUp("d"))
        {
            marioBody.linearVelocity = Vector2.zero;
        }

        if (Input.GetKeyDown("space") && onGroundState)
        {
            marioBody.AddForce(Vector2.up * upSpeed, ForceMode2D.Impulse);
            onGroundState = false;
        }
    }

    // Ground = layer 3, Enemies = layer 6, Obstacles = layer 7
    int collisionLayerMask = (1 << 3) | (1 << 6) | (1 << 7);

    void OnCollisionEnter2D(Collision2D col)
    {
        if (((collisionLayerMask & (1 << col.gameObject.layer)) > 0) && !onGroundState)
            onGroundState = true;
    }

    // updated: also hides enemies and static environment
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Collided with goomba!");
            Time.timeScale = 0.0f;
            gameOverPanel.SetActive(true);
            gameOverScoreText.text = "Score: " + jumpOverGoomba.score.ToString();
            alwaysVisibleRestartButton.SetActive(false);
            enemies.SetActive(false);
            staticEnvironment.SetActive(false);
        }
    }

    // updated: also re-shows enemies and static environment
    public void RestartButtonCallback(int input)
    {
        Debug.Log("Restart!");
        ResetGame();
        Time.timeScale = 1.0f;
        gameOverPanel.SetActive(false);
        alwaysVisibleRestartButton.SetActive(true);
        enemies.SetActive(true);
        staticEnvironment.SetActive(true);
    }

    public void ResetGame()
    {
        marioBody.transform.position = new Vector3(-22.46f, -1.45f, 0.0f);
        faceRightState = true;
        marioSprite.flipX = false;
        scoreText.text = "Score: 0";

        foreach (Transform eachChild in enemies.transform)
        {
            eachChild.localPosition = eachChild.GetComponent<EnemyMovement>().startPosition;
        }

        jumpOverGoomba.score = 0;
    }
}