using Cysharp.Threading.Tasks;
using Game.Scripts.Core.StateMachine;
using Game.Scripts.Infrastructure.Bootstrapper;
using Game.Scripts.Infrastructure.Services;
using Game.Scripts.Infrastructure.Services.Level;
using Game.Scripts.Infrastructure.Services.Player;
using UnityEngine;

namespace Game.Scripts.Infrastructure.GameStateMachine.States
{
    public class LoadingLevelState : State<EGameState>
    {
        private readonly ICoroutineRunnerService _coroutineRunner;
        private LevelBuilderService _levelBuilderService;
        private MissionService _missionService;
        private LevelDataService _levelDataService;
        private PraiseService _praiseService;
        private DifficultyService _difficultyService;
        private PlayerCollisionPhysicsService _playerCollisionPhysicsService;

        public LoadingLevelState(LevelBuilderService levelBuilderService, MissionService missionService,
            LevelDataService levelDataService, PraiseService praiseService, DifficultyService difficultyService, 
            PlayerCollisionPhysicsService playerCollisionPhysicsService)
        {
            _playerCollisionPhysicsService = playerCollisionPhysicsService;
            _difficultyService = difficultyService;
            _praiseService = praiseService;
            _levelDataService = levelDataService;
            _missionService = missionService;
            _levelBuilderService = levelBuilderService;
        }
        public override async void OnEnter(ITriggerResponder<EGameState> stateMachine)
        {
            base.OnEnter(stateMachine);

            CoroutineRunner.Instance.StopAllCoroutines();
            
            await UniTask.DelayFrame(1);
            
            _difficultyService.ApplyDifficulty();
            _playerCollisionPhysicsService.SetupDifficulties();
            
            await _levelBuilderService.CreateCurrentLevel();
            
            Debug.Log("_missionService.InitializeCurrentLevelMission");
            _missionService.InitializeCurrentLevelMission(_levelDataService.GetCurrentLevelData());
            _praiseService.Clear();
            
            Debug.Log("stateMachine.FireTrigger(EGameState.Level)");
            stateMachine.FireTrigger(EGameState.Level);
        }
    }
}