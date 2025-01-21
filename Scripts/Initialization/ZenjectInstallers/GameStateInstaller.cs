using Game.Scripts.Infrastructure.GameStateMachine;
using Game.Scripts.Infrastructure.GameStateMachine.States;
using Zenject;

namespace Game.Scripts.Initialization.ZenjectInstallers
{
    public class GameStateInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<GameStateMachine>().AsSingle();

            Container.Bind<GameLoadingState>().AsSingle();
            Container.Bind<LoadingLevelState>().AsSingle();
            Container.Bind<LobbyState>().AsSingle();
            Container.Bind<LevelState>().AsSingle();
        }
    }
}