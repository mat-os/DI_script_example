using Game.Scripts.UI.Screens.Base;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Initialization.ZenjectInstallers
{
    public class FactoryInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindFactory<Transform, Object, BaseScreen, BaseScreen.Factory>()
                .FromFactory<BaseScreenFactory>();
        }
    }
}