using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class NotifyGrabOnce : MonoBehaviour
{
    public GrabTrackerManager manager;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;
    private bool alreadyGrabbed = false;

    void Awake()
    {
        grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        grab.selectEntered.AddListener(OnGrab);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        if (alreadyGrabbed) return;

        alreadyGrabbed = true;

        if (manager != null)
            manager.RegisterGrab();
    }
}
