using UnityEngine;

// keeps the camera centred on Mario horizontally, between the start of the level and endLimit
public class CameraController : MonoBehaviour
{
    public Transform player;     // Mario
    public Transform endLimit;   // empty object at the far right end of the level

    private float offset;        // starting gap between camera and Mario
    private float startX;        // camera never goes left of this
    private float endX;          // camera never goes right of this

    void Start()
    {
        Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        float viewportHalfWidth = Mathf.Abs(bottomLeft.x - transform.position.x);
        offset = transform.position.x - player.position.x;
        startX = transform.position.x;
        endX = endLimit.position.x - viewportHalfWidth;
    }

    void LateUpdate()
    {
        float x = Mathf.Clamp(player.position.x + offset, startX, endX);
        transform.position = new Vector3(x, transform.position.y, transform.position.z);
    }
}
