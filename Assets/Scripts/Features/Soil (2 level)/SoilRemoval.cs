using UnityEngine;

public class SoilRemoval : MonoBehaviour
{
    public GameObject dustParticlePrefab; // Prefab for dust particles when soil is removed
    public float health = 100f;           // Health of the soil layer, decreases as it's removed
    public float removalSpeed = 20f;      // Speed at which soil is removed

    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is the player's tool (you can tag it as "PlayerTool")
        if (other.CompareTag("PlayerTool"))
        {
            RemoveSoil();
        }
    }

    void RemoveSoil()
    {
        health -= removalSpeed;

        // Instantiate dust particles at the soil's position
        if (dustParticlePrefab != null)
        {
            Instantiate(dustParticlePrefab, transform.position, Quaternion.identity);
        }

        // Optionally, you can also scale down the soil object to visually represent the removal
        transform.localScale *= 0.8f;

        // Check if the soil layer is completely removed
        if (health <= 0)
        {
            Debug.Log("Cleaned!");

            MineManager manager = Object.FindFirstObjectByType<MineManager>();
            if (manager != null)
            {
                manager.OnMineCleared();
            }


            Destroy(gameObject);
        }
    }
}