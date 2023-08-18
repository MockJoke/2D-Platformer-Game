using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Camera cam;
    [SerializeField] private Vector3 minBounds;
    [SerializeField] private Vector3 maxBounds;
    [SerializeField, Range(0, 1)] private float smoothTime = 0.25f;
    private Vector3 offset;
    private Vector3 lastPosition;
    private Rigidbody2D rb;
    
    private void Awake()
    {
        offset = transform.position - target.position;
        rb = target.parent.GetComponent<PlayerMovement>().playerRB;

        if (cam == null)
            cam = gameObject.GetComponent<Camera>();
    }
    
    private void LateUpdate()
    {
        Follow();
    }
    
    private void Follow()
    {
        Vector3 CameraPos = new Vector3(target.position.x + offset.x, target.position.y, transform.position.z);
        
        // Clamp camera position within specified bounds
        CameraPos.x = Mathf.Clamp(CameraPos.x, minBounds.x, maxBounds.x);
        CameraPos.y = Mathf.Clamp(CameraPos.y, minBounds.y, maxBounds.y);
        
        Vector3 currVelocity = rb.velocity;
        transform.position = Vector3.SmoothDamp(transform.position, CameraPos, ref currVelocity, smoothTime);
    }
}
