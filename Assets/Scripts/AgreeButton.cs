using UnityEngine;

public class AcceptButton : MonoBehaviour
{
    public GameObject objectToDestroy;
    public GameObject uiCanvas;
    
    public void Accept()
    {
        if (objectToDestroy != null)
            Destroy(objectToDestroy);

        if (uiCanvas != null)
            Destroy(uiCanvas);
    }
}
