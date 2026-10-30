using System.Collections;
using UnityEngine;

public class FireballController : MonoBehaviour
{
    [SerializeField] private float scaleSpeed = 1.0f;

    void Start()
    {
        StartCoroutine(ScaleAndDestroyCoroutine());
        GetComponent<AudioSource>().Play();
    }

    private IEnumerator ScaleAndDestroyCoroutine()
    {
        // Wait for 2 seconds
        yield return new WaitForSecondsRealtime(2);
        // Gradually scale down the GameObject
        while (transform.localScale.x > 0.01f)
        {
            transform.localScale -= Vector3.one * scaleSpeed * Time.deltaTime;
            yield return null;
        }

        // Ensure the GameObject is completely scaled down
        transform.localScale = Vector3.zero;

        // Destroy the GameObject
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            // squash the goomba the same way a stomp does (also adds score)
            collision.gameObject.GetComponent<EnemyMovement>()?.Stomp();
            // destroy self
            Destroy(gameObject);
        }
    }

    // the goomba's collider is a trigger, so a plain collision never fires
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyMovement>()?.Stomp();
            Destroy(gameObject);
        }
    }
}
