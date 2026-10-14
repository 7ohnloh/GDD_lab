using UnityEngine;

public class MagicMushroom : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float popImpulse = 6f;
    public float jumpMultiplier = 1.5f;
    public float sizeMultiplier = 1.3f;
    public AudioSource spawnAudio;     // smb_powerup_appears, played when it pops out
    public AudioClip collectClip;      // smb_powerup, played when Mario eats it

    private Rigidbody2D body;
    private SpriteRenderer sprite;
    private Collider2D col;
    private Vector3 startPosition;
    private int direction = 1;
    private bool collected = false;

    // remember Mario's original stats so restart can undo the powerup
    private PlayerMovement mario;
    private float originalUpSpeed;
    private Vector3 originalScale;

    void Awake()
    {
        // runs on first activation (the object starts disabled until the box is hit)
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        startPosition = transform.position;
    }

    void OnEnable()
    {
        // spawned by the question box
        transform.position = startPosition;
        direction = 1;
        collected = false;
        sprite.enabled = true;
        col.enabled = true;
        body.simulated = true;
        body.linearVelocity = Vector2.zero;
        body.AddForce(Vector2.up * popImpulse, ForceMode2D.Impulse);
        if (spawnAudio != null) spawnAudio.Play();
    }

    void FixedUpdate()
    {
        if (!collected)
            body.linearVelocity = new Vector2(direction * moveSpeed, body.linearVelocity.y);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (collected) return;

        if (other.gameObject.CompareTag("Player"))
        {
            Collect(other.gameObject.GetComponent<PlayerMovement>());
            return;
        }

        // bumped into a wall or pipe from the side: turn around
        if (Mathf.Abs(other.GetContact(0).normal.x) > 0.5f)
            direction = -direction;
    }

    void Collect(PlayerMovement player)
    {
        collected = true;
        // hide instead of disabling so the sound can finish
        sprite.enabled = false;
        col.enabled = false;
        body.simulated = false;
        if (collectClip != null && spawnAudio != null) spawnAudio.PlayOneShot(collectClip);

        if (mario == null)
        {
            mario = player;
            originalUpSpeed = mario.upSpeed;
            originalScale = mario.transform.localScale;
            mario.upSpeed *= jumpMultiplier;
            mario.transform.localScale = originalScale * sizeMultiplier;
        }
    }

    // called by the question box on restart, even while this object is disabled
    public void GameRestart()
    {
        if (mario != null)
        {
            mario.upSpeed = originalUpSpeed;
            mario.transform.localScale = originalScale;
            mario = null;
        }
        gameObject.SetActive(false);
    }
}
