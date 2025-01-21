using System;
using Configs;
using Cysharp.Threading.Tasks;
using Game.Scripts.Configs;
using Game.Scripts.Configs.Level;
using Game.Scripts.Core.CurrencyService;
using Game.Scripts.Infrastructure.GameStateMachine;
using Game.Scripts.Infrastructure.LevelStateMachin;
using Game.Scripts.Infrastructure.Services;
using Game.Scripts.Infrastructure.Services.Level;
using Game.Scripts.UI.Screens.Base.Screens;
using Game.Scripts.UI.Widgets;
using Game.Scripts.UI.Widgets.CurrencyCounter;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.Screens.Popups
{
    public class MissionStartInfoPopup : Popup
    {
        [SerializeField] private TMP_Text _levelNumInfo;
        [SerializeField] private TMP_Text _levelMission;
        [SerializeField] private Button _playButton;
        
        [SerializeField] private UpgradeProgressBarUI _upgradeProgressBarUI;
        [SerializeField] private UpgradeTutorialPanel _upgradeTutorialPanel;
        
        [SerializeField]private MissionDescriptionLineUI[] _missionDescriptionLines;
        
        [Header("Upgrade Buttons")]
        [SerializeField] private GameObject _upgradeButtonsHolder;
        [SerializeField] private UpgradeView _carUpgradeView;
        [SerializeField] private UpgradeView _stuntUpgradeView;
        [SerializeField] private UpgradeView _earningsUpgradeView;
        
        [Header("Currency View")]
        [SerializeField] private CurrencyView _coinCurrencyView;
        private CurrencyViewPresenter _coinCurrencyViewPresenter;
        
        private MissionService _missionService;
        private DataService _dataService;
        private LevelStateMachine _levelStateMachine;
        private GameStateMachine _gameStateMachine;
        private UIConfig _uiConfig;
        private LevelStarsService _levelStarsService;
        private LevelPackService _levelPackService;
        private LevelDataService _levelDataService;
        
        private UpgradePresenter _carUpgradePresenter;
        private UpgradePresenter _stuntUpgradePresenter;
        private UpgradePresenter _earningsUpgradePresenter;

        [Inject] private DiContainer _container;
        private LevelsRepository _levelsRepository;
        private LevelsProgressService _levelsProgressService;
        private MissionRepository _missionRepository;

        [Inject]
        public void Construct(MissionService missionService, 
            DataService dataService,
            LevelStateMachine levelStateMachine,
            GameStateMachine gameStateMachine,
            UIConfig uiConfig, 
            LevelsProgressService levelsProgressService, 
            LevelStarsService levelStarsService,
            LevelPackService levelPackService,
            LevelDataService levelDataService,
            CurrencyService currencyService, 
            VibrationService vibrationService,
            LevelsRepository levelsRepository,
            MissionRepository missionRepository)
        { 
            _missionRepository = missionRepository;
            _levelsProgressService = levelsProgressService;
            _levelsRepository = levelsRepository;
            _levelDataService = levelDataService;
            _levelPackService = levelPackService;
            _levelStarsService = levelStarsService;
            _uiConfig = uiConfig;
            _gameStateMachine = gameStateMachine;
            _levelStateMachine = levelStateMachine;
            _dataService = dataService;
            _missionService = missionService;
            
            _container.Inject(_upgradeProgressBarUI);

            _playButton.onClick.AddListener(G_StartLevel);
            
            _coinCurrencyViewPresenter = new CurrencyViewPresenter(currencyService, _coinCurrencyView.CurrencyType, vibrationService, _coinCurrencyView.AnimationParameterConfig);
            _coinCurrencyView.Construct(_coinCurrencyViewPresenter);     

            InitializeUpgradeButtons();
        }

        public override void OnCreate()
        {
            _coinCurrencyView.Initialize();
            _coinCurrencyViewPresenter.Initialize();
            base.OnCreate();
        }

        public override UniTask OnOpenStart()
        {
            var levelPackIndex = _levelPackService.LevelPackIndex;
            var levelIndexGlobal = _levelDataService.GetGlobalLevelId(_levelDataService.LevelId, levelPackIndex);
            var levelName =  _levelDataService.GetCurrentLevelData().LevelName ;
            _levelNumInfo.text = $"{levelIndexGlobal}. {levelName}";
            //_levelNumInfo.text = $" {_levelPackService.LevelPackIndex + 1}.{_levelDataService.LevelId + 1}. {_levelDataService.GetCurrentLevelData().LevelName}";

            SetupMissionLines(_levelPackService.LevelPackIndex, _levelDataService.LevelId);
            
            //var starsOnLevel = _levelStarsService.GetCountOfStarsOnLevel(_levelPackService.GetCurrentLevelPack(), _levelDataService.GetCurrentLevelData());
            //SetupPopup(_missionService.GetCurrentMissionConfigs(), starsOnLevel);

            TryShowUpgradeButtons();
            
            _levelStateMachine.FireTrigger(ELevelState.LevelMission);

            return base.OnOpenStart();
        }
        private void SetupMissionLines(int levelPackIndex, int levelId)
        {
            var missionConfigs = _missionService.GetCurrentMissionConfigs();
            var levelProgress = _levelsProgressService.GetLevelProgress(levelPackIndex, levelId);
                
            var mission= _missionRepository.MissionSettings[missionConfigs[0].EMissionType];
            _levelMission.text = mission.TextForMissionInfoDescriptionLineUI;
            
            for (var i = 0; i < _missionDescriptionLines.Length; i++)
            {
                var missionDescriptionLine = _missionDescriptionLines[i];
                var missionConfig = missionConfigs[i];
                var isCompleted = levelProgress.MissionProgress[i];
                var missionSetting = _missionRepository.MissionSettings[missionConfig.EMissionType];
                missionDescriptionLine.SetupLine(missionSetting, missionConfig, isCompleted);
            }
        }

        private void TryShowUpgradeButtons()
        {
            var isShowUpgradeButtons = _dataService.Upgrades.IsShowUpgradeButtons.Value || _levelsRepository.IsDebugMode == true;
            
            _upgradeButtonsHolder.gameObject.SetActive(isShowUpgradeButtons);
            
            _carUpgradePresenter.OnOpenStart();
            _stuntUpgradePresenter.OnOpenStart();
            _earningsUpgradePresenter.OnOpenStart();
            
            var isShowUpgradeTutorial = _dataService.Tutorial.IsUpgradeTutorialShowed.Value;
            if(isShowUpgradeTutorial == false && isShowUpgradeButtons)
                ShowUpgradeButtonTutorial();
        }

        private void ShowUpgradeButtonTutorial()
        {
            _upgradeTutorialPanel.gameObject.SetActive(true);
            _dataService.Tutorial.IsUpgradeTutorialShowed.Value = true;
        }

        public void InitializeUpgradeButtons()
        {
            // Апгрейд машины (Car)
            var carUpgradeModel = new UpgradeModel();
            _container.Inject(carUpgradeModel);
            _carUpgradePresenter = new UpgradePresenter(_carUpgradeView, carUpgradeModel, EUpgradeType.Car);

            // Апгрейд трюков (Stunt)
            var stuntUpgradeModel = new UpgradeModel();
            _container.Inject(stuntUpgradeModel);
            _stuntUpgradePresenter = new UpgradePresenter(_stuntUpgradeView, stuntUpgradeModel, EUpgradeType.Stunt);

            // Апгрейд заработка (Earnings)
            var earningsUpgradeModel = new UpgradeModel();
            _container.Inject(earningsUpgradeModel);
            _earningsUpgradePresenter = new UpgradePresenter(_earningsUpgradeView, earningsUpgradeModel, EUpgradeType.Income);
            
            _upgradeProgressBarUI.Initialize();
        }
        public override UniTask OnCloseComplete()
        {
            return base.OnCloseComplete();
        }
        
        // Метод для настройки попапа на основе конфигурации миссии.
        /*public void SetupPopup(MissionConfig missionConfig, int currentProgress)
        {
            for (int i = 0; i < 3; i++)
            {
                UpdateMissionText(missionConfig, i);
            }
            UpdateStars(currentProgress);
        }

        // Метод для обновления текста цели миссии в зависимости от индекса (1 звезда, 2 звезды, 3 звезды)
        public void UpdateMissionText(MissionConfig missionConfig, int starIndex)
        {
            int targetValue = GetStarTargetValue(missionConfig, starIndex);

            // Заменяем {x} в шаблоне на конкретное значение
            string updatedDescription = missionConfig.Description.Replace("{x}", targetValue.ToString());

            // Обновляем текстовое поле для текущей миссии
            _missionsDescription[starIndex].text = updatedDescription;
        }

        // Метод для получения целевого значения для каждой звезды (1 звезда, 2 звезды, 3 звезды)
        private int GetStarTargetValue(MissionConfig missionConfig, int starIndex)
        {
            switch (starIndex)
            {
                case 0: return missionConfig.OneStarTargetValue;
                case 1: return missionConfig.TwoStarTargetValue;
                case 2: return missionConfig.ThreeStarTargetValue;
                default: throw new ArgumentOutOfRangeException(nameof(starIndex), "Некорректный индекс звезды");
            }
        }*/

        /*
        private void UpdateStars(int currentProgress)
        {
            for (int i = 0; i < _stars.Length; i++)
            {
                var isActive = i < currentProgress;
                _stars[i].sprite = isActive ? _uiConfig.ActiveStar : _uiConfig.InactiveStar;
            }
        }
        */

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && IsPointerOverUI() == false)
            {
                G_StartLevel();
            }
        }
        public bool IsPointerOverUI()
        {
            if (EventSystem.current == null) 
                return false;
        
            // Создаем список объектов, которые попали под курсор/палец
            PointerEventData eventData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            // Список объектов, попавших под raycast
            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            // Проверяем, есть ли объекты под курсором
            return results.Count > 1;
        }
        private void G_StartLevel()
        {
            _levelStateMachine.FireTrigger(ELevelState.Play);
            //_gameStateMachine.FireTrigger(EGameState.Level);
        }
        private void G_Close()
        {
            //_gameStateMachine.FireTrigger(EGameState.Level);
        }
    }
}