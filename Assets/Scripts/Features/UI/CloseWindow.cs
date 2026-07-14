using UnityEngine;

namespace DeminingAcademy.Features.UI
{
    public class CloseWindow : MonoBehaviour
    {
        [SerializeField] private GameObject player;
        [SerializeField] private GameObject infoCanvas;
        [SerializeField] private AudioSource welcomeSound;
        private void Start()
        {
            var moveProvider = player.GetComponent<UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement.ContinuousMoveProvider>();
            if (moveProvider != null)
                moveProvider.enabled = false;
            infoCanvas.SetActive(true);
            welcomeSound.Play();
        }

        public void CloseInfoWindow()
        {
            var moveProvider = player.GetComponent<UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement.ContinuousMoveProvider>();
            if (moveProvider != null)
                moveProvider.enabled = true;
            infoCanvas.SetActive(false);
            welcomeSound.Stop();
        }
    }
}
