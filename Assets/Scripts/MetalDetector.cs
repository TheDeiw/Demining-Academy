using UnityEngine;
using System.Collections;

public class MetalDetector : MonoBehaviour
{
    [Header("References")]
    public AudioSource audioSource;
    public AudioClip beepClip;
    
    [Tooltip("Arraste as 3 esferas na ordem: [0] Longe, [1] Médio, [2] Perto")]
    public MeshRenderer[] lampRenderers;

    [Header("Settings")]
    public string targetTag = "Mine";
    public float maxDetectionDistance = 2.0f;
    
    [ColorUsage(false, false)] // Desativado HDR aqui para você controlar a cor real
    public Color alertColor = Color.red;
    
    [Range(0f, 1f)]
    public float emissionStrength = 0.2f; // Controle fino da potência do brilho

    private Transform currentMine;
    private bool isBeeping = false;
    private Material[] lampMaterials;

    private void Awake()
    {
        if (lampRenderers != null && lampRenderers.Length > 0)
        {
            lampMaterials = new Material[lampRenderers.Length];
            for (int i = 0; i < lampRenderers.Length; i++)
            {
                lampMaterials[i] = lampRenderers[i].material;
                lampMaterials[i].EnableKeyword("_EMISSION");
            }
        }
        ResetFeedback();
    }

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
            ResetFeedback();
        }
    }

    private IEnumerator PlayBeepRoutine()
    {
        isBeeping = true;

        while (currentMine != null)
        {
            float distance = Vector3.Distance(transform.position, currentMine.position);
            
            // Se afastar demais, para tudo
            if (distance > maxDetectionDistance + 0.5f) break;

            if (audioSource != null && beepClip != null)
                audioSource.PlayOneShot(beepClip);

            // t vai de 0 (perto da mina) a 1 (longe da mina)
            float t = Mathf.Clamp01(distance / maxDetectionDistance);
            float proximity = 1.0f - t; // 0 (longe) a 1 (perto)

            UpdateLamps(proximity);

            // A distância controla o tempo do bip: quanto menor a distância, menor o tempo de espera
            float beepInterval = Mathf.Lerp(0.05f, 1.5f, t);
            yield return new WaitForSeconds(beepInterval);
        }

        isBeeping = false;
        ResetFeedback();
    }

    private void UpdateLamps(float proximity)
    {
        // Define quantas das 3 lâmpadas acendem (1, 2 ou 3)
        int lampsToLight = Mathf.CeilToInt(proximity * 3);

        for (int i = 0; i < lampMaterials.Length; i++)
        {
            if (i < lampsToLight)
            {
                // Aplica a cor sem multiplicar por valores HDR altos para não ficar branco
                lampMaterials[i].SetColor("_BaseColor", alertColor);
                
                // A intensidade agora é uma fração pequena da cor original
                Color finalEmission = alertColor * emissionStrength;
                lampMaterials[i].SetColor("_EmissionColor", finalEmission);
            }
            else
            {
                // Lâmpada apagada
                lampMaterials[i].SetColor("_BaseColor", new Color(0.1f, 0.1f, 0.1f));
                lampMaterials[i].SetColor("_EmissionColor", Color.black);
            }
        }
    }

    private void ResetFeedback()
    {
        if (lampMaterials == null) return;
        foreach (var mat in lampMaterials)
        {
            mat.SetColor("_BaseColor", new Color(0.1f, 0.1f, 0.1f));
            mat.SetColor("_EmissionColor", Color.black);
        }
    }
}