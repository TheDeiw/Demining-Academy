using UnityEngine;

public class SoilRemoval : MonoBehaviour
{
    [SerializeField] private GameObject dustParticlePrefab; // Prefab for dust particles when soil is removed
    [SerializeField] private float health = 100f;           // Health of the soil layer, decreases as it's removed
    [SerializeField] private float removalSpeed = 20f;      // Speed at which soil is removed
    [SerializeField] private GameObject confetti;


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

        if (dustParticlePrefab != null)
        {
            Instantiate(dustParticlePrefab, transform.position, Quaternion.identity);
        }

        transform.localScale *= 0.8f;

        if (health <= 0)
        {
            if (confetti != null)
            {
                Instantiate(confetti, transform.position, Quaternion.identity);
            }

            MineManager manager = Object.FindFirstObjectByType<MineManager>();
            if (manager != null)
            {
                manager.OnMineCleared();
            }


            Destroy(gameObject);
        }
    }
}