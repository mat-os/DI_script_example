using Configs;
using Game.Scripts.Configs.Level;
using Game.Scripts.Core.StateMachine;
using Game.Scripts.Infrastructure.Services;
using Game.Scripts.Infrastructure.Services.Analytics;
using Game.Scripts.Infrastructure.Services.Level;
using Game.Scripts.Infrastructure.Services.Player;
using Game.Scripts.Utils.Debug;
using UnityEngine;

namespace Game.Scripts.Infrastructure.LevelStateMachin.States
{
    public class CompleteLevelState : State<ELevelState>
    {
        private readonly AnalyticService _analyticsService;
        private GameConfig _gameConfig;
        private DataService _dataService;
        private LevelsRepository _levelsRepository;
        private LevelsProgressService _levelsProgressService;
        private LevelPackService _levelPackService;
        private LevelDataService _levelDataService;
        private LevelTrophyService _levelTrophyService;
        private MissionService _missionService;
        private LevelStarsService _levelStarsService;
        private InputService _inputService;
        private AnalyticService _analyticService;
        private PlayerScoreService _scoreService;
        private UIConfig _uiConfig;

        public CompleteLevelState(GameConfig gameConfig,
            DataService dataService,
            LevelsRepository levelsRepository,
            LevelsProgressService levelsProgressService,
            LevelPackService levelPackService,
            LevelDataService levelDataService,
            LevelTrophyService levelTrophyService,
            MissionService missionService,
            LevelStarsService levelStarsService,
            InputService inputService,
            AnalyticService analyticService,
            PlayerScoreService scoreService,
            UIConfig uiConfig)
        {
            _uiConfig = uiConfig;
            _scoreService = scoreService;
            _analyticService = analyticService;
            _inputService = inputService;
            _levelStarsService = levelStarsService;
            _missionService = missionService;
            _levelTrophyService = levelTrophyService;
            _levelDataService = levelDataService;
            _levelPackService = levelPackService;
            _levelsProgressService = levelsProgressService;
            _levelsRepository = levelsRepository;
            _dataService = dataService;
            _gameConfig = gameConfig;
        }

        public override  void  OnEnter(ITriggerResponder<ELevelState> stateMachine)
        {
            base.OnEnter(stateMachine);
            
            _inputService.EnableInput(false);
            
            var currentLevelPack = _levelPackService.GetCurrentLevelPack();
            var currentPackId = currentLevelPack.GetPackId();
            var currentLevelId = _levelDataService.LevelId;

            if (_dataService.Level.IsCompleteLevelZero.Value == false && _levelsRepository.IsStartFromLevelZero)
            {
                _dataService.Level.IsCompleteLevelZero.Value = true;
            }
            else
            {
                UpgradeLevelProgressData(currentPackId, currentLevelId);
            }
            
            _dataService.Level.AttemptNumber.Value = 0;
        }

        private void UpgradeLevelProgressData(int currentPackId, int currentLevelId)
        {
            var previousStarsEarned = _levelsProgressService.GetLevelProgress(currentPackId, currentLevelId).Stars;
            var newStarsEarned = _missionService.GetCountOfEarnedStarsOnCurrentLevel();
            
            var isTrophyCollected = _levelTrophyService.IsTrophyCollected;
            
            var isItNewLevelComplete = previousStarsEarned == 0 && newStarsEarned > 0;

            _levelStarsService.SetRewardForStars(previousStarsEarned, newStarsEarned);
            
            var levelMissionsProgress = _missionService.GetCurrentMissionsProgress();
            
            _levelsProgressService.UpdateLevelProgress(currentPackId, currentLevelId, newStarsEarned, isTrophyCollected, levelMissionsProgress);

            if (isItNewLevelComplete)
            {
                _dataService.Level.TotalLevelComplete.Value ++;
                CustomDebugLog.Log("[SAVE] Increase Total Level Complete " + _dataService.Level.TotalLevelComplete.Value);

                if (_dataService.Upgrades.IsShowUpgradeButtons.Value == false)
                {
                    if (_dataService.Level.TotalLevelComplete.Value >= _uiConfig.LevelToShowUpgradeButtons)
                    {
                        _dataService.Upgrades.IsShowUpgradeButtons.Value = true;
                    }
                }

            }
            
            _analyticService.LogLevelComplete(currentLevelId, currentPackId, newStarsEarned, _scoreService.GetTotalScore());
            CustomDebugLog.Log("STARS " + previousStarsEarned + " and " + newStarsEarned);
        }

        public override void OnExit()
        {
            base.OnExit();
            
            _levelTrophyService.Clear();
        }
    }
}