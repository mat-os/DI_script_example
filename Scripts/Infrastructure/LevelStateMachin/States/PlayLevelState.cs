using System.Linq;
using Game.Scripts.Core.StateMachine;
using Game.Scripts.Infrastructure.GameStateMachine.States;
using Game.Scripts.Infrastructure.Services;
using Game.Scripts.Infrastructure.Services.Analytics;
using Game.Scripts.Infrastructure.Services.Level;
using Game.Scripts.Infrastructure.Services.Player;
using Game.Scripts.Utils.Debug;

namespace Game.Scripts.Infrastructure.LevelStateMachin.States
{
    public class PlayLevelState : State<ELevelState>
    {
        private readonly PlayerFlightLandingService _playerFlightLandingService;
        private ITriggerResponder<ELevelState> _stateMachine;
        private MissionService _missionService;
        private FungusDialogueService _fungusDialogueService;
        private LevelDataService _levelDataService;
        private LevelsProgressService _levelsProgressService;
        private LevelPackService _levelPackService;

        public PlayLevelState(PlayerFlightLandingService playerFlightLandingService, MissionService missionService, FungusDialogueService fungusDialogueService,
            LevelDataService levelDataService, LevelsProgressService levelsProgressService, LevelPackService levelPackService)
        {
            _levelPackService = levelPackService;
            _levelsProgressService = levelsProgressService;
            _levelDataService = levelDataService;
            _fungusDialogueService = fungusDialogueService;
            _missionService = missionService;
            _playerFlightLandingService = playerFlightLandingService;
        }

        public override void OnEnter(ITriggerResponder<ELevelState> stateMachine)
        {
            base.OnEnter(stateMachine);
            _stateMachine = stateMachine;
            _playerFlightLandingService.OnPlayerFlyComplete += OnPlayerFlyCompleteHandle;
            
            GlobalEventSystem.Broker.Publish(new StartPlayLevelEvent() {  });
        }


        public override void OnExit()
        {
            _playerFlightLandingService.OnPlayerFlyComplete -= OnPlayerFlyCompleteHandle;

            base.OnExit();
        }
        
        private void OnPlayerFlyCompleteHandle()
        {
            var isAnyMissionCompleted = _missionService.IsAnyCurrentMissionCompleted();
            var previousStarsEarned = _levelsProgressService.GetLevelProgress(_levelPackService.LevelPackIndex, _levelDataService.LevelId).Stars;
            var isNewLevelComplete = previousStarsEarned == 0 && isAnyMissionCompleted;
            
            var endDialogue = _levelDataService.GetCurrentLevelData().FinishLevelDialogueConfig;
            
            if (endDialogue != null && (isNewLevelComplete || endDialogue.IsForceShowDialogue))
            {
                _fungusDialogueService.StartDialogue(endDialogue, OnEndLevelDialogueCompleted);
                _stateMachine.FireTrigger(ELevelState.FinalDialogue);
            }
            else
            {
                _stateMachine.FireTrigger(isAnyMissionCompleted ? ELevelState.Complete : ELevelState.Fail);
            }
        }

        private void OnEndLevelDialogueCompleted()
        {
            _stateMachine.FireTrigger(ELevelState.Complete);
        }
    }
}