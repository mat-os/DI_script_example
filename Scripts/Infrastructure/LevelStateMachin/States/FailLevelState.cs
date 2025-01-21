using Configs;
using DG.Tweening;
using Game.Scripts.Core.StateMachine;
using Game.Scripts.Infrastructure.Services;
using Game.Scripts.Infrastructure.Services.Analytics;
using Game.Scripts.Infrastructure.Services.Level;

namespace Game.Scripts.Infrastructure.LevelStateMachin.States
{
    public class FailLevelState : State<ELevelState>
    {
        private GameConfig _gameConfig;
        private InputService _inputService;
        private AnalyticService _analyticService;
        private DataService _dataService;
        private LevelDataService _levelDataService;
        private LevelPackService _levelPackService;

        FailLevelState(GameConfig gameConfig,
            InputService inputService,
            AnalyticService analyticService,
            DataService dataService,
            LevelDataService levelDataService,
            LevelPackService levelPackService)
        {
            _levelPackService = levelPackService;
            _levelDataService = levelDataService;
            _dataService = dataService;
            _analyticService = analyticService;
            _inputService = inputService;
            _gameConfig = gameConfig;
        }

        public override void OnEnter(ITriggerResponder<ELevelState> stateMachine)
        {
            _inputService.EnableInput(false);
            
            _analyticService.LogLevelFail(_levelDataService.LevelId, _levelPackService.LevelPackIndex);
            _dataService.Level.AttemptNumber.Value++;
            
            base.OnEnter(stateMachine);
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}