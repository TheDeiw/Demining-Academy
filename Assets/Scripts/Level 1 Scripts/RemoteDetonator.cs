using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit; // Assuming you use XRIT

public class RemoteDetonator : MonoBehaviour
{
    public DisposalArea holeZone; 
    private bool IsExplosionConnected;
    
    [Header("Detonation Settings")]
    public GameObject explosionPrefab;
    public string mineTag = "Mine";
    
    public void TryDetonate()
    {
        if (!IsExplosionConnected)
        {
            Debug.Log("Detonation failed: No charge attached to a mine!");
            return;
        }
        
        int totalMinesInScene = GameObject.FindGameObjectsWithTag(mineTag).Length;
        int minesInHole = holeZone.minesInHole.Count;

        if (minesInHole < totalMinesInScene)
        {
            Debug.Log("WARNING: Not all mines are in the disposal pit! The road is still blocked.");
        }

        ExecuteExplosion();
    }

    public void ChargerConnected()
    {
        IsExplosionConnected = true;
        Debug.Log("Charger connected!");
    }
    
    public void ChargerDisconnected()
    {
        IsExplosionConnected = false;
        Debug.Log("Charger disconnected!");
    }

    private void ExecuteExplosion()
    {
        foreach (GameObject mine in holeZone.minesInHole)
        {
            if (mine != null)
            {
                MineDangerousHandling mineHandler = mine.GetComponent<MineDangerousHandling>();
                if (mineHandler != null)            
                    mineHandler.Explode("Remote detonation triggered!");
            }
        }
        
        Debug.Log("Road cleared successfully!");
    }
}