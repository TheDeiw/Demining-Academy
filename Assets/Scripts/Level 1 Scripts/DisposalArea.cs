using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class DisposalArea : MonoBehaviour
{
    [SerializeField] TMP_Text counterText;
    public List<GameObject> minesInHole = new List<GameObject>();
    
    private int counter = 0;

    void Start()
    {
        UpdateCounterText();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Mine") && !minesInHole.Contains(other.gameObject))
        {
            minesInHole.Add(other.gameObject);
            counter++;
            UpdateCounterText();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Mine"))
        {
            minesInHole.Remove(other.gameObject);
            counter--;
            UpdateCounterText();
        }
    }
    
    private void UpdateCounterText()
    {
        counterText.text = counter + " / " + GameObject.FindGameObjectsWithTag("Mine").Length;
    }
    
    public bool AreAllMinesInHole()
    {
        return minesInHole.Count == GameObject.FindGameObjectsWithTag("Mine").Length;
    }
}
