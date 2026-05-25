using UnityEngine;
using DeminingAcademy.Core;
using DeminingAcademy.Infrastructure;
using Zenject;

namespace DeminingAcademy.Installers
{
    public class GameSceneInstaller : MonoInstaller
    {
        [SerializeField] private GlobalAppSettings _appSettings;
        public override void InstallBindings()
        {
            Container.Bind<GlobalAppSettings>().AsSingle();
            
            // 1. Реєструємо ModeSwitcherService
            // FromComponentInHierarchy() означає: "Знайди об'єкт з цим скриптом прямо на сцені Unity"
            Container.Bind<IModeSwitcherService>()
                .To<ModeSwitcherService>()
                .FromComponentInHierarchy()
                .AsSingle();

            // 2. Реєструємо GameManager
            // BindInterfacesAndSelfTo каже: "Зареєструй його як GameManager і як IInitializable"
            // NonLazy() каже: "Створи його ОДРАЗУ при запуску сцени, не чекай, поки його хтось попросить"
            Container.BindInterfacesAndSelfTo<GameManager>()
                .AsSingle()
                .NonLazy();
        }
    }
}