using UnityEngine;

public class ObjectRespawner : MonoBehaviour
{
    public float thresholdY = -10f; // Reset when falling below this Y value
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Rigidbody rb;

    void Start()
    {
        // Record the initial position and rotation of the object
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (transform.position.y < thresholdY)
        {
            ResetPosition();
        }
    }

    public void ResetPosition()
    {
        // Reset velocity if Rigidbody exists
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Reset position and rotation
        transform.position = initialPosition;
        transform.rotation = initialRotation;

        Debug.Log("Reset");
    }
}