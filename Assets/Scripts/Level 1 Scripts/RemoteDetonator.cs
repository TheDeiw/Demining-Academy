using Level_1_Scripts;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit; // Assuming you use XRIT

public class RemoteDetonator : MonoBehaviour
{
    public DisposalArea holeZone; 
    private bool IsExplosionConnected = false;
    
    [SerializeField] private ExecuteFinish gameFinishHandler;
        
    private string mineTag = "Mine";
    private AudioSource clickSound;
    void Start()
    {
        clickSound = GetComponent<AudioSource>();
    }
    
    public void TryDetonate()
    {
        clickSound.Play();
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
        
        if (holeZone.AreAllMinesInHole())
        {
            foreach (GameObject mine in holeZone.minesInHole)
            {
                if (mine != null)
                {
                    MineDangerousHandling mineHandler = mine.GetComponent<MineDangerousHandling>();
                    if (mineHandler != null)            
                        mineHandler.Explode("Win");
                }
            }
            gameFinishHandler.FinishGame(1);
            Debug.Log("Road cleared successfully!");
        }
        else
        {
            Debug.Log("There are still mines outside the disposal pit! Detonation may not clear the road.");
        }
    }
}