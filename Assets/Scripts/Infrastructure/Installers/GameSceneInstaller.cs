using DeminingAcademy.Core;
using DeminingAcademy.Infrastructure.VRModeSwitch;
using Zenject;

namespace DeminingAcademy.Infrastructure.Installers
{
    public class GameSceneInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IModeSwitcherService>()
                .To<ModeSwitcherService>()
                .FromComponentInHierarchy()
                .AsSingle();
            
            Container.BindInterfacesAndSelfTo<GameManager>()
                .AsSingle()
                .NonLazy();
        }
    }
}