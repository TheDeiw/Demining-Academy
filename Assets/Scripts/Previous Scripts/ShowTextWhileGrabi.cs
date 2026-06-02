using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ShowTextOnGrab : MonoBehaviour
{
    public GameObject textObject; 

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        if (textObject != null)
            textObject.SetActive(true);
    }

    void OnRelease(SelectExitEventArgs args)
    {
        if (textObject != null)
            textObject.SetActive(false);
    }
}
