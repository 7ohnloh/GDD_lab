using System.Collections;
using UnityEngine;

public class BounceOnHitFromBelow : MonoBehaviour
{
    public float bounceImpulse = 5f;   // how hard the box pops up
    public float settleTime = 1f;      // seconds to let the spring wobble before locking
    public GameObject coin;            // optional: leave empty for a brick without a coin

    private Rigidbody2D body;
    private Animator animator;         // optional: only the question box has one
    private Vector2 startPosition;
    private bool used = false;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        startPosition = body.position;
        // locked by default so Mario standing on top doesn't push it down
        body.constraints = RigidbodyConstraints2D.FreezeAll;
        if (coin != null) coin.SetActive(false);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (used || !col.gameObject.CompareTag("Player")) return;

        Vector2 normal = col.GetContact(0).normal;
        Debug.Log("Box hit, normal = " + normal);

        // normal points from Mario into the box, so up means hit from below
        if (normal.y > 0.5f)
        {
            used = true;
            body.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            body.AddForce(Vector2.up * bounceImpulse, ForceMode2D.Impulse);
            StartCoroutine(LockAfterBounce());

            if (animator != null) animator.SetTrigger("hit");
            if (coin != null) coin.SetActive(true);
        }
    }

    IEnumerator LockAfterBounce()
    {
        yield return new WaitForSeconds(settleTime);
        body.constraints = RigidbodyConstraints2D.FreezeAll;
        body.linearVelocity = Vector2.zero;
        // snap back so it doesn't freeze mid-wobble
        body.position = startPosition;
    }
}
