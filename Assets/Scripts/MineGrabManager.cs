using UnityEngine;

public class GrabTrackerManager : MonoBehaviour
{
    public int totalObjects = 3;          // cuantos objetos deben agarrarse
    public GameObject objectToDestroy;    // objeto que desaparecerá
    public GameObject objetToDestroy2;    // objeto que desaparecerá

    private int grabbedCount = 0;

    public void RegisterGrab()
    {
        grabbedCount++;

        Debug.Log("Objetos agarrados: " + grabbedCount);

        if (grabbedCount >= totalObjects)
        {
            if (objectToDestroy != null)
                Destroy(objectToDestroy);
            if (objetToDestroy2 != null)
                Destroy(objetToDestroy2);
        }
    }
}
