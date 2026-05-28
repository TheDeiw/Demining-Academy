using DeminingAcademy.Infrastructure.SceneManagement;
using DeminingAcademy.Features.UI.GlobalLoading;
using UnityEngine;
using Zenject;

namespace DeminingAcademy.Infrastructure.Installers
{
    public class ProjectInstaller : MonoInstaller
    {
        [Header("Global Application Settings")]
        [SerializeField] private GlobalAppSettings _appSettings;
        
        [SerializeField] private ScreenFader _screenFader;

        public override void InstallBindings()
        {
            // Bind our global settings
            Container.BindInstance(_appSettings).AsSingle();
            Container.BindInstance(_screenFader).AsSingle();

            // Bind the Scene Loader Service globally
            Container.Bind<ISceneLoaderService>()
                .To<SceneLoaderService>()
                .AsSingle();
        }
    }
}