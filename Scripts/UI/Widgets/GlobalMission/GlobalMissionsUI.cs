using System;
using System.Collections.Generic;
using System.Linq;
using AssetKits.ParticleImage;
using Coffee.UIEffects;
using Configs;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Scripts.Configs;
using Game.Scripts.Configs.Level;
using Game.Scripts.Core.CurrencyService;
using Game.Scripts.Infrastructure.GameStateMachine;
using Game.Scripts.Infrastructure.LevelStateMachin;
using Game.Scripts.Infrastructure.Services;
using Game.Scripts.Infrastructure.Services.Analytics;
using Game.Scripts.Infrastructure.Services.Level;
using Game.Scripts.Infrastructure.Services.Player;
using Game.Scripts.Infrastructure.States;
using Game.Scripts.UI.FxService;
using Game.Scripts.UI.Screens.Base.Screens;
using Game.Scripts.UI.Screens.Serviсes;
using Game.Scripts.UI.Widgets;
using Game.Scripts.UI.Widgets.CurrencyCounter;
using Game.Scripts.Utils;
using Game.Scripts.Utils.Debug;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Zenject;

public class GlobalMissionsUI : MonoBehaviour
{
    [SerializeField]private ParticleImage _winFx;
    
    [Header("Mission")]
    [SerializeField] private GlobalMissionButtonUI _missionButtonPrefab;
    [SerializeField] private Transform _missionButtonsRoot;
    
    [Header("Currency View")]
    [SerializeField] private CurrencyView _coinCurrencyView;
    private CurrencyViewPresenter _coinCurrencyViewPresenter;
    
    private List<GlobalMissionButtonUI> _missionButtons = new List<GlobalMissionButtonUI>();

    private UIFXService _uiFxService;
    private GameConfig _gameConfig;
    private CurrencyService _currencyService;
    private DataService _dataService;
    private UIConfig _uiConfig;
    
    [Inject]private DiContainer _container;
    private int _currentPackId;
    private int _currentLevelId;
    private LevelProgress _nextLevel;
    private AnalyticService _analyticService;

     private string _freezeReason = "WinScreenReward";
     private GlobalMissionService _globalMissionService;
     private GlobalMissionsConfig _globalMissionsConfig;
     private int _activeMissionsMaxCount;
     private MissionProgress _newMission;

     [Inject]
    public void Construct
    (
        GameConfig gameConfig,
        UIFXService uifxService,
        CurrencyService currencyService,
        VibrationService vibrationService,
        PageService pageService,
        DataService dataService,
        UIConfig uiConfig,
        AnalyticService analyticService,
        GlobalMissionService globalMissionService)
    {
        _globalMissionService = globalMissionService;
        _analyticService = analyticService;
        _uiConfig = uiConfig;
        _gameConfig = gameConfig;
        _uiFxService = uifxService;
        _currencyService = currencyService;
        _dataService = dataService;
        
        _globalMissionsConfig = _gameConfig.GlobalMissionsConfig;
        _activeMissionsMaxCount = _globalMissionsConfig.GlobalMissionsCountOnUI;
        
        _coinCurrencyViewPresenter = new CurrencyViewPresenter(currencyService, _coinCurrencyView.CurrencyType, vibrationService, _coinCurrencyView.AnimationParameterConfig);
        _coinCurrencyView.Construct(_coinCurrencyViewPresenter);
    }
    public  void OnCreate()
    {
        CreateMissionButtons();
        _globalMissionService.OnActivateNewMission += OnActivateNewMissionHandler;
        _coinCurrencyView.Initialize();
        _coinCurrencyViewPresenter.Initialize();
    }

    private void OnActivateNewMissionHandler(MissionProgress newMission)
    {
        _newMission = newMission;
    }

    public void CreateMissionButtons()
    {
        _missionButtonsRoot.Clear();
        
        var missionsProgress = _globalMissionService.GetActiveMissionsProgress();
        for (int i = 0; i < missionsProgress.Count; i++)
        {
            var missionProgress = missionsProgress[i];
            CreateMissionButton(missionProgress);
        }
    }

    private void CreateMissionButton(MissionProgress missionProgress)
    {
        var missionButtonUI = Instantiate(_missionButtonPrefab, _missionButtonsRoot);
        _container.Inject(missionButtonUI);
        _container.Inject(missionButtonUI.MissionProgressBarUI);
        
        missionButtonUI.Initialize(missionProgress);
        missionButtonUI.OnOpenStart(missionProgress);
        
        _missionButtons.Add(missionButtonUI);
            
        missionButtonUI.OnGetReward += OnGetRewardHandle;
    }

    private void OnGetRewardHandle(GlobalMissionButtonUI missionButtonUI)
    {
        var missionConfig = missionButtonUI.MissionConfig;
        AddCurrency(missionConfig.Reward, missionButtonUI.GetButtonTransform());
       _globalMissionService.CompleteMission(missionConfig);
       _winFx.SetActive(true);
       _winFx.Play();
        DOVirtual.DelayedCall(_uiConfig.LevelCompleteScreenHideDelay, () =>
        {
            RemoveMissionButton(missionButtonUI);
        });
    }

    private void RemoveMissionButton(GlobalMissionButtonUI missionButtonUI)
    {
        _missionButtons.Remove(missionButtonUI);
        missionButtonUI.Remove(OnRemoveCompleted);
    }

    private void OnRemoveCompleted()
    {
        if (_newMission != null)
        {
            Debug.Log($"New mission activated on UI: {_newMission.Mission.EMissionType}");
            CreateMissionButton(_newMission);
            _newMission = null;
        }
    }
    public void OnOpenStart()
    {
        OnOpenStartMissionProgressBar();
    }
    private void OnOpenStartMissionProgressBar()
    {
        var missionsProgress = _globalMissionService.GetActiveMissionsProgress();

        for (var i = 0; i < _missionButtons.Count; i++)
        {
            var missionButton = _missionButtons[i];
            var missionProgress = missionsProgress[i];
            missionButton.OnOpenStart(missionProgress);
        }
    }
    public void OnOpenComplete()
    {
        var missionsProgress = _globalMissionService.GetActiveMissionsProgress();
        for (var i = 0; i < _missionButtons.Count; i++)
        {
            var missionProgressBarUI = _missionButtons[i];
            var missionProgress = missionsProgress[i];
            var missionConfig = missionProgress.Mission;
            missionProgressBarUI.ShowFinalResult(missionProgress ,missionConfig);
        }
    }
    public void OnCloseComplete()
    {
        _winFx.Stop(true);
        _winFx.SetActive(false);
    }
    private void AddCurrency(int reward, Transform spawnPoint)
    {
        _uiFxService.SpawnCurrencyMovementEffect(spawnPoint, _coinCurrencyView.CurrencyIconRoot.transform, _coinCurrencyViewPresenter, ECurrencyType.Coins);
        DOVirtual.DelayedCall(_uiConfig.LevelCompleteScreenHideDelay / 2, () =>
        {
            _currencyService.AddCurrency(ECurrencyType.Coins, reward);
        });
    }

    private void OnDestroy()
    {
        _globalMissionService.OnActivateNewMission -= OnActivateNewMissionHandler;
    }
}
