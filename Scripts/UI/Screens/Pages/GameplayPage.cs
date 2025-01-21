using System;
using System.Collections.Generic;
using Configs;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Scripts.Infrastructure.Services;
using Game.Scripts.Infrastructure.Services.Car;
using Game.Scripts.Infrastructure.Services.Player;
using Game.Scripts.UI.Screens.Base.Screens;
using Game.Scripts.UI.Screens.Popups;
using Game.Scripts.UI.Screens.Serviсes;
using Game.Scripts.UI.Widgets;
using PG;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.Screens.Pages
{
    public class GameplayPage : Page, IDisposable
    {
        [SerializeField] private DamageTextUI _damageTextUI;
        [SerializeField] private ScoreCounterUI _scoreCounterUI;
        [SerializeField] private MissionProgressBarUIGameplay _missionProgressBarUI;
        [SerializeField] private CarSpeedometerPanelUI _carSpeedometerPanelUI;
        [SerializeField] private BoneDamageUI _boneDamageUI;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private GameObject _holdToRideTutorial;
        [SerializeField] private PraisePhraseUI _praisePhraseUI;
        [SerializeField] private AirTimeTextUI _airTimeTextUI;
        [SerializeField] private EnergyBarUI _energyBarUI;
       // [SerializeField] private TapGameUI  _tapGameUI ;
        
        [Inject]private DiContainer _container;
        
        private GameConfig _gameConfig;
        private CarService _carService;
        private PopupService _popupService;
        private PlayerFlightLaunchService _playerFlightLaunchService;
        private MissionService _missionService;

        [Inject]
        public void Construct(PlayerDamageService playerDamageService, 
            GameConfig gameConfig, 
            MissionService missionService,
            UIConfig uiConfig,
            CarService carService,
            PopupService popupService,
            PlayerFlightLaunchService playerFlightLaunchService)
        {
            _missionService = missionService;
            _playerFlightLaunchService = playerFlightLaunchService;
            _popupService = popupService;
            _carService = carService;
            _gameConfig = gameConfig;
            
            _container.Inject(_missionProgressBarUI);
            _container.Inject(_damageTextUI);
            _container.Inject(_scoreCounterUI);
            _container.Inject(_boneDamageUI);
            _container.Inject(_carSpeedometerPanelUI);
            _container.Inject(_airTimeTextUI);
            _container.Inject(_energyBarUI);
            _container.Inject(_praisePhraseUI);
            
            //_container.Inject(_tapGameUI);

            _playerFlightLaunchService.OnPlayerFlyStart += OnPlayerFlyStartHandler;
            _missionService.OnTotalMissionValueChange += HandleMissionsValueChange;

        }

        private void HandleMissionsValueChange(List<MissionProgress> missionProgresses) => 
            _missionProgressBarUI.UpdateProgress(missionProgresses);

        private void OnPlayerFlyStartHandler()
        {
            _boneDamageUI.gameObject.SetActive(true);
            _carSpeedometerPanelUI.gameObject.SetActive(false);
        }

        public override void OnCreate()
        {
            _settingsButton.onClick.AddListener(SettingsButtonClicked);
            _scoreCounterUI.Subscribe();
            _praisePhraseUI.Subscribe();
            base.OnCreate();
        }
        
        public override UniTask OnOpenStart()
        {
            var activeMissionsOnLevel = _missionService.GetActiveMissionsOnCurrentLevel();
            var isNeedProgressBar = activeMissionsOnLevel.Count > 0;
            _missionProgressBarUI.SetActive(isNeedProgressBar);
            _missionProgressBarUI.OnOpenStart(_missionService.GetCurrentMissionConfigs(), activeMissionsOnLevel);
            
           // _tapGameUI.OnOpenStart();
            _carSpeedometerPanelUI.Initialize();
            _praisePhraseUI.Clear();
            _scoreCounterUI.Clear();

            _airTimeTextUI.OnOpenStart();
            
            _boneDamageUI.Reset();
            
            _damageTextUI.UpdateDamageText(0);
            _damageTextUI.ChangeFlyDistance(0);
            
            _energyBarUI.SetEnergyBarActive(false);

            _boneDamageUI.gameObject.SetActive(false);
            _holdToRideTutorial.gameObject.SetActive(true);
            _carSpeedometerPanelUI.gameObject.SetActive(true);

            return base.OnOpenStart();
        }

        private void Update()
        {
            if (_holdToRideTutorial.gameObject.activeSelf)
            {
                if (Input.GetMouseButton(0))
                {
                    _holdToRideTutorial.gameObject.SetActive(false);
                }
            }
        }

        private void SettingsButtonClicked()
        {
            _popupService.ShowScreen<SettingsPopup>();
        }

        public override UniTask OnCloseComplete()
        {
            _carSpeedometerPanelUI.Clear();
            _energyBarUI.Clear();
            return base.OnCloseComplete();
        }

        public void Dispose()
        {
            _scoreCounterUI.Unsubscribe();
            _missionService.OnTotalMissionValueChange -= HandleMissionsValueChange;
            _playerFlightLaunchService.OnPlayerFlyStart -= OnPlayerFlyStartHandler;
        }
    }
}