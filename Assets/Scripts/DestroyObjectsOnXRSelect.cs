using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class DestroyOnGrab : MonoBehaviour
{
    [Header("Objects to destroy when this object is grabbed")]
    public GameObject canvasToDestroy;
    public GameObject cubeToDestroy;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;

    void Awake()
    {
        grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        // Cuando lo agarras (grab / select)
        grab.selectEntered.AddListener(OnGrabbed);
    }

    private void OnDestroy()
    {
        // Limpieza (evita warnings raros al salir/recargar escena)
        if (grab != null)
            grab.selectEntered.RemoveListener(OnGrabbed);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (canvasToDestroy != null) Destroy(canvasToDestroy);
        if (cubeToDestroy != null) Destroy(cubeToDestroy);
        Destroy(gameObject); // Opcional: también destruye el objeto que se agarra
    }
}
