using Cysharp.Threading.Tasks;
using Zenject;
using DeminingAcademy.Infrastructure.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DeminingAcademy
{
    public class RestartTest : MonoBehaviour
    {
        [Inject] private ISceneLoaderService _sceneLoader;

        private void Update()
        {
            // wasPressedThisFrame = спрацьовує ОДИН РАЗ при натисканні
            if (Keyboard.current.rightAltKey.wasPressedThisFrame)
            {
                _sceneLoader.LoadSceneAsync("Main Level 1").Forget();
            }
        }
    }
}