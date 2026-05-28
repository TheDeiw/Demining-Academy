using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

using Zenject;
using DeminingAcademy.Infrastructure.SceneManagement;

public class VRLevelManager : MonoBehaviour
{
    [Header("Configurações de UI")]
    [Tooltip("Arraste aqui o novo Canvas que você criou")]
    public GameObject menuPanel;

    [Header("Configurações de Input")]
    [Tooltip("Selecione a ação 'Menu' ou 'PrimaryButton' do controle")]
    public InputActionProperty toggleMenuAction;

    [Header("Posicionamento")]
    public bool recenterOnOpen = true;
    public Transform headTransform; // Arraste a Main Camera
    public float distanceFromPlayer = 2.0f;
    
    private ISceneLoaderService _sceneLoader;

    // Injecting our global service into a local UI script
    [Inject]
    public void Construct(ISceneLoaderService sceneLoader)
    {
        _sceneLoader = sceneLoader;
    }
    
    private void Start()
    {
        // Começa fechado
        if (menuPanel != null)
            menuPanel.SetActive(false);
    }

    private void Update()
    {
        // Verifica o clique no botão físico do controle
        if (toggleMenuAction.action != null && toggleMenuAction.action.WasPressedThisFrame())
        {
            ToggleMenu();
        }
    }

    public void ToggleMenu()
    {
        if (menuPanel == null) return;

        bool isActive = !menuPanel.activeSelf;
        menuPanel.SetActive(isActive);

        
        if (isActive && recenterOnOpen && headTransform != null)
        {
            PositionMenuInFront();
        }
    }

    private void PositionMenuInFront()
    {
        Vector3 targetPosition = headTransform.position + (headTransform.forward * distanceFromPlayer);
        targetPosition.y = headTransform.position.y; 

        menuPanel.transform.position = targetPosition;
        menuPanel.transform.LookAt(new Vector3(headTransform.position.x, menuPanel.transform.position.y, headTransform.position.z));
        menuPanel.transform.Rotate(0, 180, 0);
    }

    public void ChangeLevel(string sceneName)
    {
        Debug.Log("Carregando cena: " + sceneName);
        _sceneLoader.LoadSceneAsync(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}