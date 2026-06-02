using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using DeminingAcademy.Features.UI.GlobalLoading;

namespace DeminingAcademy.Infrastructure.SceneManagement
{
    public class SceneLoaderService : ISceneLoaderService
    {
        private readonly ScreenFader _fader;
        private bool _isLoading;

        private AsyncOperationHandle<SceneInstance> _activeSceneHandle;
        
        public SceneLoaderService(ScreenFader fader)
        {
            _fader = fader;
        }

        public async UniTask LoadSceneAsync(string sceneName, CancellationToken ct = default)
        {
            if (_isLoading)
            {
                Debug.LogWarning($"[SceneLoaderService] Already loading, skipping: {sceneName}");
                return;
            }
            _isLoading = true;

            Camera cam = Camera.main;
            if (!cam)
            {
                Debug.LogError("[SceneLoaderService] Camera.main is null");
                _isLoading = false;
                return;
            }
            
            _fader.AttachToCamera(cam);
            await _fader.FadeOutAsync();
            
            // Показуємо loading UI і запускаємо завантаження у фоні
            //_fader.ShowLoadingUI(true);

            // Load scene from Addressables
            var loadHandle = Addressables.LoadSceneAsync(
                sceneName, 
                LoadSceneMode.Single, 
                activateOnLoad: false
            );
            
            await loadHandle.Task;
            if (loadHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"[SceneLoaderService] CRITICAL: Addressables failed to load '{sceneName}'. Check Addressables Groups!");
                await _fader.FadeInAsync();
                _fader.Detach();
                _isLoading = false;
                return;
            }
            
            _fader.Detach();
            
            var previousHandle = _activeSceneHandle;

            await loadHandle.Result.ActivateAsync().ToUniTask(cancellationToken: ct);
            _activeSceneHandle = loadHandle;
            
            if (previousHandle.IsValid())
                await Addressables.UnloadSceneAsync(previousHandle);
            
            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            timeoutCts.CancelAfter(TimeSpan.FromSeconds(5f));

            try
            {
                await UniTask.WaitUntil(() => Camera.main != null, cancellationToken: timeoutCts.Token);
                await UniTask.DelayFrame(2, PlayerLoopTiming.Update, cancellationToken: timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                Debug.LogError("[SceneLoaderService] Timeout: Camera.main not found after 5s. Does new scene have a MainCamera?");
                _isLoading = false;
                return;
            }
            
            _fader.AttachToCamera(Camera.main);
            await _fader.FadeInAsync();

            _isLoading = false;
            Debug.Log($"[SceneLoaderService] Successfully loaded: {sceneName}");
        }

        public async UniTask ReloadCurrentSceneAsync()
        {
            await LoadSceneAsync(SceneManager.GetActiveScene().name);
        }
    }
}