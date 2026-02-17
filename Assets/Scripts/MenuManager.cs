using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void LoadLevel1()
    {
        SceneManager.LoadScene("Main Level 1");
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