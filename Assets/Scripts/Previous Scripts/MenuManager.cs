using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using DeminingAcademy.Infrastructure.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private ISceneLoaderService _sceneLoader;

    // Injecting our global service into a local UI script
    [Inject]
    public void Construct(ISceneLoaderService sceneLoader)
    {
        _sceneLoader = sceneLoader;
    }
    public void LoadLevel1()
    {
        _sceneLoader.LoadSceneAsync("Main Level 1");
    }
    
    public void LoadLevel2()
    {
        SceneManager.LoadScene("level2");
    }
    
    public void LoadLobby()
    {
        SceneManager.LoadScene("Scenes/Test Scenes/Kevin Tests/Lobby");
    }
    
    public void QuitGame()
    {
        Debug.Log("Closing game...");

        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                    Application.Quit();
        #endif
    }
}