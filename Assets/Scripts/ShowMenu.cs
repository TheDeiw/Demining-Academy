using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShowMenu : MonoBehaviour
{
    public Transform player;
    public float playerDistance = 3.0f;
    public GameObject canvasMenu;
    public InputActionProperty showButton;

    // Update is called once per frame
    void Update()
    {
        if (showButton.action.WasPressedThisFrame())
        {
            canvasMenu.SetActive(value: !canvasMenu.activeSelf);

            canvasMenu.transform.position = player.position + new Vector3(player.forward.x, 0, player.forward.z).normalized*playerDistance;
        }
        
        canvasMenu.transform.LookAt(worldPosition: new Vector3(player.position.x, canvasMenu.transform.position.y, player.position.z));
        canvasMenu.transform.forward *= -1;
    }
}
