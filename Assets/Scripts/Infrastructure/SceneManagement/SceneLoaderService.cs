using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using DeminingAcademy.Features.UI.GlobalLoading;

namespace DeminingAcademy.Infrastructure.SceneManagement
{
    public class SceneLoaderService : ISceneLoaderService
    {
        private readonly ScreenFader _fader;
        private bool _isLoading;

        public SceneLoaderService(ScreenFader fader)
        {
            _fader = fader;
        }

        public async UniTask LoadSceneAsync(string sceneName)
        {
            if (_isLoading)
            {
                Debug.LogWarning($"[SceneLoaderService] Already loading, skipping: {sceneName}");
                return;
            }
            _isLoading = true;

            Camera cam = Camera.main;
            if (cam == null)
            {
                Debug.LogError("[SceneLoaderService] Camera.main is null");
                _isLoading = false;
                return;
            }

            // 1. Fade out → чорний екран
            _fader.AttachToCamera(cam);
            await _fader.FadeOutAsync();

            // 3. Завантажуємо сцену у фоні
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
            op.allowSceneActivation = false;

            while (op.progress < 0.9f)
                await UniTask.Yield();
            
            _fader.Detach();
            // 4. Активуємо сцену
            op.allowSceneActivation = true;
            await op.ToUniTask();

            await UniTask.WaitUntil(() => Camera.main != null);
            
            _fader.AttachToCamera(Camera.main);
            await _fader.FadeInAsync();

            _isLoading = false;
            Debug.Log($"[SceneLoaderService] Loaded: {sceneName}");
        }

        public async UniTask ReloadCurrentSceneAsync()
        {
            await LoadSceneAsync(SceneManager.GetActiveScene().name);
        }
    }
}