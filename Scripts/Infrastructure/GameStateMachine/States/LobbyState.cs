using Configs;
using Game.Scripts.Core.StateMachine;
using Game.Scripts.Infrastructure.Services;

namespace Game.Scripts.Infrastructure.GameStateMachine.States
{
    public class LobbyState : State<EGameState>
    {
        private GameConfig _gameConfig;
        private DataService _dataService;
        private InputService _inputService;

        public LobbyState(GameConfig gameConfig,
            DataService dataService,
            InputService inputService)
        {
            _inputService = inputService;
            _dataService = dataService;
            _gameConfig = gameConfig;
        }
        
        public override void OnEnter(ITriggerResponder<EGameState> stateMachine)
        {
            _inputService.EnableInput(false);
            base.OnEnter(stateMachine);
        }
        public override void OnExit()
        {
            base.OnExit();
        }
    }
}