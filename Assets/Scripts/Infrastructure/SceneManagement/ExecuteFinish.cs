using UnityEngine;

namespace Level_1_Scripts
{
    public class ExecuteFinish : MonoBehaviour
    {
        [SerializeField] GameObject winCanvas;
        [SerializeField] GameObject loseCanvas;
        [SerializeField] GameObject player;
        
        [SerializeField] AudioSource welcomeAudio;
        [SerializeField] AudioSource backgroundAudio;
        [SerializeField] AudioSource loseAudio;

        void Start()
        {
            winCanvas.SetActive(false);
            loseCanvas.SetActive(false);
        }

        public void FinishGame(int result)
        {
            if (result == 0)
            {
                loseCanvas.SetActive(true);
                loseAudio.Play();
            }
            else if (result == 1)
            {
                winCanvas.SetActive(true);
            }
            var moveProvider = player.GetComponent<UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement.ContinuousMoveProvider>();
            if (moveProvider != null)
                moveProvider.enabled = false;
            welcomeAudio.Stop();
            backgroundAudio.Stop();
        }
        
        
    }
}