using Game.Scripts.Configs.Level;
using Game.Scripts.Core.StateMachine;
using Game.Scripts.Infrastructure.Services;
using Game.Scripts.Infrastructure.Services.Analytics;
using Game.Scripts.Infrastructure.Services.Car;
using Game.Scripts.Infrastructure.Services.Level;
using Game.Scripts.Infrastructure.Services.Player;
using Game.Scripts.UI.Screens.Popups;
using Game.Scripts.UI.Screens.Serviсes;

namespace Game.Scripts.Infrastructure.GameStateMachine.States
{
    public class LevelState : State<EGameState>
    {
        private PlayerFlightTrackerService _playerFlightTrackerService;
        private PlayerDamageService _playerDamageService;
        private InputService _inputService;
        private CarInputService _carInputService;
        private EnergyService _energyService;
        private PlayerFlightLaunchService _playerFlightLaunchService;
        private PlayerFlightLandingService _playerFlightLandingService;
        private LevelBuilderService _levelBuilderService;
        private FungusDialogueService _fungusDialogueService;
        private LevelDataService _levelDataService;
        private PlayerComboService _playerComboService;
        private PlayerScoreService _playerScoreService;
        private SlowMotionService _slowMotionService;
        private PlayerFlightControlService _playerFlightControlService;
        private PlayerFlightLineDirectionService _playerFlightLineDirectionService;
        private LevelStarsService _levelStarsService;
        private PopupService _popupService;
        private AnalyticService _analyticService;
        private LevelPackService _levelPackService;
        private PlayerAirTimeCounterService _airTimeCounterService;
        private PlayerJumpService _playerJumpService;
        private PlayerDragControlService _playerDragControlService;
        private PlayerSwipeService _playerSwipeService;
        private GlobalMissionService _globalMissionService;

        public LevelState(PlayerFlightTrackerService playerFlightTrackerService,
            PlayerFlightLaunchService playerFlightLaunchService,
            PlayerFlightLandingService playerFlightLandingService,
            PlayerDamageService playerDamageService,
            InputService inputService,
            CarInputService carInputService,
            EnergyService energyService,
            LevelBuilderService levelBuilderService,
            FungusDialogueService fungusDialogueService,
            LevelDataService levelDataService,
            PlayerComboService playerComboService,
            PlayerScoreService playerScoreService,
            SlowMotionService slowMotionService,
            PlayerFlightControlService playerFlightControlService,
            PlayerFlightLineDirectionService playerFlightLineDirectionService,
            LevelStarsService levelStarsService,
            PopupService popupService,
            AnalyticService analyticService,
            LevelPackService levelPackService,
            PlayerAirTimeCounterService airTimeCounterService,
            PlayerJumpService playerJumpService,
            PlayerDragControlService playerDragControlService,
            PlayerSwipeService playerSwipeService,
            GlobalMissionService globalMissionService)
        {
            _globalMissionService = globalMissionService;
            _playerSwipeService = playerSwipeService;
            _playerDragControlService = playerDragControlService;
            _playerJumpService = playerJumpService;
            _airTimeCounterService = airTimeCounterService;
            _levelPackService = levelPackService;
            _analyticService = analyticService;
            _popupService = popupService;
            _levelStarsService = levelStarsService;
            _playerFlightLineDirectionService = playerFlightLineDirectionService;
            _playerFlightControlService = playerFlightControlService;
            _slowMotionService = slowMotionService;
            _playerScoreService = playerScoreService;
            _playerComboService = playerComboService;
            _levelDataService = levelDataService;
            _fungusDialogueService = fungusDialogueService;
            _levelBuilderService = levelBuilderService;
            _playerFlightLandingService = playerFlightLandingService;
            _playerFlightLaunchService = playerFlightLaunchService;
            _energyService = energyService;
            _carInputService = carInputService;
            _inputService = inputService;
            _playerDamageService = playerDamageService;
            _playerFlightTrackerService = playerFlightTrackerService;
        }

        public override void OnEnter(ITriggerResponder<EGameState> stateMachine)
        {
            _inputService.EnableInput(true);
            _carInputService.SetIsReadInput(true);
            _energyService.Initialize();
            
            _fungusDialogueService.StartDialogue(_levelDataService.GetCurrentLevelData().DialogueConfig, OnStartLevelDialogueCompleted);
            
            _playerComboService.Clear();
            _playerScoreService.Clear();
            _levelStarsService.Clear();
            _airTimeCounterService.Clear();
            _playerSwipeService.Clear();
            
            if (_levelDataService.IsStartFromLevelZero)
            {
                _playerFlightControlService.SetIsPlayerCanJump(false);
            }

            _analyticService.LogLevelStart(_levelDataService.LevelId, _levelPackService.LevelPackIndex);
            
            base.OnEnter(stateMachine);
        }

        private void OnStartLevelDialogueCompleted()
        {
            _carInputService.SetIsReadInput(true);
            _popupService.ShowScreen<MissionStartInfoPopup>();
        }

        public override void OnExit()
        {
            _globalMissionService.SaveAllProgress();
            
            _inputService.EnableInput(false);
            _carInputService.SetIsReadInput(false);

            _playerFlightTrackerService.Clear();
            _playerFlightLaunchService.Clear();
            _playerFlightLandingService.Clear();
            _playerDamageService.Clear();
            _playerFlightControlService.Clear();
            _playerJumpService.Clear();
            _playerDragControlService.Clear();
            _playerFlightLineDirectionService.Clear();
            
            _slowMotionService.Clear();
            
            _levelBuilderService.ClearLevel();
            
            base.OnExit();
        }
    }
}