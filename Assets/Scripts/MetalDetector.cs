using UnityEngine;
using System.Collections;

public class MetalDetector : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip beepClip;
    
    [Header("Detection Parameters")]
    public string targetTag = "Mine";
    public float maxDetectionDistance = 1.5f;
    
    [Tooltip("Fastest interval when touching the mine (Very fast)")]
    public float minBeepInterval = 0.02f; 
    
    [Tooltip("Slowest interval when first detected (Increased for slower start)")]
    public float maxBeepInterval = 1.5f; // Aumentado para ser bem lento no início

    private Transform currentMine;
    private bool isBeeping = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            currentMine = other.transform;
            if (!isBeeping && gameObject.activeInHierarchy)
            {
                StartCoroutine(PlayBeepRoutine());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            currentMine = null;
        }
    }

    private IEnumerator PlayBeepRoutine()
    {
        isBeeping = true;

        while (currentMine != null)
        {
            float distance = Vector3.Distance(transform.position, currentMine.position);

            if (distance > maxDetectionDistance)
            {
                break; 
            }

            if (audioSource != null && beepClip != null)
            {
                audioSource.PlayOneShot(beepClip);
            }

            // Normalizing distance (0 to 1)
            float t = Mathf.Clamp01(distance / maxDetectionDistance);
            
            // Using a higher Power (3) makes the curve "steeper".
            // It stays at maxBeepInterval for much longer before accelerating.
            float acceleratedT = Mathf.Pow(t, 3); 

            float currentInterval = Mathf.Lerp(minBeepInterval, maxBeepInterval, acceleratedT);

            yield return new WaitForSeconds(currentInterval);
        }

        isBeeping = false;

        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    private void OnDisable()
    {
        currentMine = null;
        isBeeping = false;
        StopAllCoroutines();
    }
}