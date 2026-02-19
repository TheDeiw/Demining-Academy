using Level_1_Scripts;
using UnityEngine;

public class MineDangerousHandling : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private ExecuteFinish gameFinishHandler;
    
    [Header("Sensitivity Settings")]
    [Tooltip("The maximum impact force the mine can withstand before detonating.")]
    public float impactThreshold = 5f;
    
    [Tooltip("The maximum acceleration (shaking) the mine can withstand before detonating.")]
    public float shakeThreshold = 10f;

    [Header("Effects")]
    [Tooltip("Prefab for the explosion visual and sound effects.")]
    public GameObject explosionPrefab;
    
    [Header("Audio")]
    public GameObject explosionSound;
    
    private Rigidbody rb;
    private Vector3 lastVelocity;
    private bool isExploded = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (isExploded) return;

        // Calculate acceleration (change in velocity per unit of time)
        Vector3 acceleration = (rb.linearVelocity - lastVelocity) / Time.fixedDeltaTime;
        lastVelocity = rb.linearVelocity;

        // Check if the player is shaking the mine too violently
        if (acceleration.magnitude > shakeThreshold)
        {
            Explode("Excessive shaking or sudden movement!");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isExploded) return;

        // Check the impact force (relative velocity magnitude)
        // relativeVelocity.magnitude represents the speed of the collision impact
        if (collision.relativeVelocity.magnitude > impactThreshold)
        {
            Explode("Hard impact with a surface!");
        }
    }

    public void Explode(string reason)
    {
        isExploded = true;
        Debug.Log("MINE DETONATED: " + reason);
        
        // Spawn the explosion visual effect at the mine's current position
        if (explosionPrefab != null)
        {
            Instantiate(explosionSound, transform.position, transform.rotation);
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }
        // Logic for mission failure or game over
        // Example: FindObjectOfType<GameManager>().OnMineExploded();

        // Remove the mine object from the scene
        Destroy(gameObject);
        
        if (reason != "Win")
        {
            Debug.Log(reason);
            gameFinishHandler.FinishGame(0);
        }
    }
}
