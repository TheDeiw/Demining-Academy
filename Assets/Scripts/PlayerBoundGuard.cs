using UnityEngine;

public class PlayerBoundGuard : MonoBehaviour
{
    public float killPlaneY = -5f; // Y-coordinate of the kill plane
    public Transform respawnPoint; // Reference to the respawn point transform

    void Update()
    {
        if (transform.position.y < killPlaneY)
        {
            // Reset player position to respawn point
            transform.position = respawnPoint.position;
            Debug.Log("Reset");
        }
    }
}