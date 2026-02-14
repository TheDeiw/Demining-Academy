using UnityEngine;
using System.Collections.Generic;

public class DisposalArea : MonoBehaviour
{
    
    public List<GameObject> minesInHole = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Mine") && !minesInHole.Contains(other.gameObject))
        {
            minesInHole.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Mine"))
        {
            minesInHole.Remove(other.gameObject);
        }
    }
}
