using System;
using System.Collections.Generic;
using Configs;
using Cysharp.Threading.Tasks;
using Game.Scripts.Configs.Level;
using Game.Scripts.Infrastructure.GameStateMachine;
using Game.Scripts.Infrastructure.LevelStateMachin;
using Game.Scripts.Infrastructure.Services;
using Game.Scripts.Infrastructure.Services.Level;
using Game.Scripts.UI.Screens.Base.Screens;
using Game.Scripts.UI.Screens.Serviсes;
using Game.Scripts.UI.Widgets;
using Game.Scripts.Utils.Debug;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.Screens.Popups
{
    public class LevelPackSelectPopup : Popup
    {
        [SerializeField] private Transform _levelButtonParent; 
        [SerializeField] private LevelSelectButtonUI _levelSelectButtonPrefab; 
        [SerializeField] private Button _closeButton; 
        [SerializeField] private TMP_Text _levelName; 
        
        [Header("Stats")]
        [SerializeField] private TMP_Text  _starsCountText;
        [SerializeField] private TMP_Text  _trophyCountText;
        [SerializeField] private ProgressBarFill _progressBar;
        
        private LevelStarsService _levelStarsService;
        private DataService _dataService;
        private List<LevelSelectButtonUI> _levelButtons = new List<LevelSelectButtonUI>();
        private LevelDataService _levelDataService;
        private LevelPackService _levelPackService;
        private PopupService _popupService;
        private LevelStateMachine _levelStateMachine;
        private GameStateMachine _gameStateMachine;
        private LevelTrophyService _levelTrophyService;
        private LevelsProgressService _levelsProgressService;

        [Inject]
        public void Construct(LevelStarsService levelStarsService, DataService dataService, LevelDataService levelDataService, 
            LevelPackService levelPackService, PopupService popupService, LevelStateMachine levelStateMachine, 
            GameStateMachine gameStateMachine, LevelTrophyService levelTrophyService, LevelsProgressService levelsProgressService)
        {
            _levelsProgressService = levelsProgressService;
            _levelTrophyService = levelTrophyService;
            _gameStateMachine = gameStateMachine;
            _levelStateMachine = levelStateMachine;
            _popupService = popupService;
            _levelPackService = levelPackService;
            _levelDataService = levelDataService;
            _levelStarsService = levelStarsService;
            _dataService = dataService;
            
            _closeButton.onClick.AddListener(OnClickCloseButton);
            _levelButtonParent.Clear();
        }

        private void OnClickCloseButton()
        {
            _popupService.CloseScreen<LevelPackSelectPopup>();
        }

        public override UniTask OnOpenStart()
        {
            LevelPackConfig currentLevelPack = _levelPackService.GetCurrentLevelPack();
            SetupLevelButtons(currentLevelPack);
            _levelName.text = currentLevelPack.Name;
            
            InitStatistics(currentLevelPack);

            return base.OnOpenStart();
        }

        private void InitStatistics(LevelPackConfig currentLevelPack)
        {
            var totalLevelsCount = currentLevelPack.LevelDataConfigs.Count;
            var earnedStars = _levelStarsService.GetCountOfEarnedStarsInPack(currentLevelPack);
            var startOnThisPackTotalCount = totalLevelsCount * 3;
            _starsCountText.text = $"{earnedStars}/{startOnThisPackTotalCount}";
            
            var earnedTrophy = _levelTrophyService.GetCountOfCollectedTrophyInPack(currentLevelPack);
            _trophyCountText.text = $"{earnedTrophy}/{totalLevelsCount}";
            
            _progressBar.SetProgressInstant(earnedStars, startOnThisPackTotalCount);
        }

        private void SetupLevelButtons(LevelPackConfig currentLevelPack)
        {
            foreach (var button in _levelButtons)
            {
                Destroy(button.gameObject);
            }
            _levelButtons.Clear();

            for (int i = 0; i < currentLevelPack.LevelDataConfigs.Count; i++)
            {
                var levelButton = Instantiate(_levelSelectButtonPrefab, _levelButtonParent);
                _levelButtons.Add(levelButton);

                var packId = currentLevelPack.GetPackId();
                var countOfStarsOnLevel = _levelStarsService.GetCountOfStarsOnLevel(packId, i);
                var isEarnTrophyOnLevel = _levelTrophyService.IsEarnTrophyOnLevel(packId, i);
                bool isLocked = IsLevelLocked(i, currentLevelPack);
                var levelData = currentLevelPack.LevelDataConfigs[i];
                levelButton.Setup(i + 1, countOfStarsOnLevel, isLocked, isEarnTrophyOnLevel, levelData.LevelName, levelData.LevelImage);

                int levelIndex = i;
                levelButton.OnClick(() => OnClickLevelButton(isLocked, levelIndex));
            }
        }

        public override UniTask OnCloseComplete()
        {
            return base.OnCloseComplete();
        }

        private void OnClickLevelButton(bool isLocked, int levelIndex)
        {
            if (!isLocked)
            {
                StartLevel(levelIndex);
            }
        }

        private bool IsLevelLocked(int levelIndex, LevelPackConfig levelPackConfig)
        {
            if (Debug.isDebugBuild || Application.isEditor)
            {
                if (_levelPackService.IsAllLevelsUnlocked)
                    return false;
            }
            
            if (levelIndex == 0 && levelPackConfig.GetPackId() == 0)
                return false;

            // Получаем прогресс предыдущего уровня
            var previousLevel = _levelsProgressService.GetPreviousLevel(levelPackConfig.GetPackId(), levelIndex);
            return previousLevel.IsCompleted == false;
            
            /*var countOfStarsOnLevel = _levelStarsService.GetCountOfStarsOnLevel(levelPackConfig.GetPackId(), levelIndex - 1);
            return countOfStarsOnLevel == 0;*/
        }

        private void StartLevel(int levelIndex)
        {
            _levelDataService.SetCurrentLevelData(levelIndex);
            _gameStateMachine.FireTrigger(EGameState.LevelLoading);
        }
    }
}
