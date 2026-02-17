using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFloorButton : MonoBehaviour
{
    [Header("Scene to load (must be in Build Settings)")]
    public string sceneName;

    [Header("Who can trigger it")]
    public string playerTag = "Player";

    [Header("Optional: avoid double trigger")]
    public float cooldownSeconds = 1f;

    private bool canTrigger = true;

    private void OnTriggerEnter(Collider other)
    {
        if (!canTrigger) return;
        if (!other.CompareTag(playerTag)) return;

        canTrigger = false;
        SceneManager.LoadScene(sceneName);
    }

    private void OnTriggerExit(Collider other)
    {
        // opcional: si quieres que se pueda reusar al salir
        // canTrigger = true;
        Invoke(nameof(ResetTrigger), cooldownSeconds);
    }

    private void ResetTrigger()
    {
        canTrigger = true;
    }
}
