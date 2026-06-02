using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SpawnFlagOnInteract : MonoBehaviour
{
    [Header("Prefab a spawnear")]
    public GameObject flagPrefab;

    [Header("Dónde spawnear (posición + rotación)")]
    public Transform spawnPoint;

    [Header("Opcional: si quieres que sea hijo del cubo")]
    public bool parentToThis = false;

    private bool hasSpawned = false;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable interactable;

    private void Awake()
    {
        interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();
        if (interactable == null)
        {
            Debug.LogError("Falta un XR Interactable (XR Simple/Grab Interactable) en este objeto.");
        }
    }

    private void OnEnable()
    {
        if (interactable != null)
            interactable.selectEntered.AddListener(OnSelected);
        // Si prefieres “activar” en vez de select, dímelo y lo cambiamos.
    }

    private void OnDisable()
    {
        if (interactable != null)
            interactable.selectEntered.RemoveListener(OnSelected);
    }

    private void OnSelected(SelectEnterEventArgs args)
    {
        if (hasSpawned) return;
        if (flagPrefab == null || spawnPoint == null) return;

        hasSpawned = true;

        GameObject flag = Instantiate(
            flagPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        if (parentToThis)
            flag.transform.SetParent(transform, true);
    }
}
