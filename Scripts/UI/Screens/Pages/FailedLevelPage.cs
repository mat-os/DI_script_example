using System.Linq;
using Configs;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Scripts.Core.CurrencyService;
using Game.Scripts.Infrastructure.GameStateMachine;
using Game.Scripts.Infrastructure.LevelStateMachin;
using Game.Scripts.Infrastructure.Services;
using Game.Scripts.Infrastructure.Services.Analytics;
using Game.Scripts.Infrastructure.Services.Level;
using Game.Scripts.Infrastructure.Services.Player;
using Game.Scripts.UI.FxService;
using Game.Scripts.UI.Screens.Base.Screens;
using Game.Scripts.UI.Widgets;
using Game.Scripts.UI.Widgets.CurrencyCounter;
using Game.Scripts.Utils;
using Game.Scripts.Utils.Debug;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;
using GameStateMachine = Game.Scripts.Infrastructure.GameStateMachine.GameStateMachine;

public class FailedLevelPage : Page
{
    [SerializeField] private TMP_Text _level;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _menuButton;

    [Header("Score")]
    [SerializeField]private TMP_Text _score;
    [SerializeField]private AnimationParameterConfig _scoreAnimation;
    
    [Header("Earning")]
    [SerializeField]private TMP_Text _earnings;
    
    [Header("Currency View")]
    [SerializeField] private CurrencyView _coinCurrencyView;
    private CurrencyViewPresenter _coinCurrencyViewPresenter;
    
    [Header("Missions Progress")]
    //[SerializeField] private MissionProgressBarUI[] _missionProgressBarUI;
    [SerializeField] private MissionProgressBarUIFinalScreen _missionProgressBar;

    private LevelStateMachine _levelStateMachine;
    private GameStateMachine _gameStateMachine;
    private DataService _dataService;

    [Inject]private DiContainer _container;
    private PlayerScoreService _scoreService;
    private int _totalReward;
    private UIFXService _uiFxService;
    private UIConfig _uiConfig;
    private CurrencyService _currencyService;
    private LevelPackService _levelPackService;
    private LevelDataService _levelDataService;
    private AnalyticService _analyticService;
    
    private bool _isContinueButtonPressed;
    private MissionService _missionService;

    [Inject]
   private void Construct(LevelStateMachine levelStateMachine,
       GameStateMachine gameStateMachine,
       DataService dataService,
       PlayerScoreService scoreService,
       CurrencyService currencyService,
       VibrationService vibrationService,
       UIFXService uifxService,
       UIConfig uiConfig,
       LevelDataService levelDataService,
       LevelPackService levelPackService,
       AnalyticService analyticService,
       MissionService missionService)
   {
       _missionService = missionService;
       _analyticService = analyticService;
       _levelDataService = levelDataService;
       _levelPackService = levelPackService;
       _uiConfig = uiConfig;
       _uiFxService = uifxService;
       _scoreService = scoreService;
       _dataService = dataService;
       _gameStateMachine = gameStateMachine;
       _levelStateMachine = levelStateMachine;
       _currencyService = currencyService;

       /*foreach (var missionProgressBar in _missionProgressBarUI)
       {
           _container.Inject(missionProgressBar);
       }*/
       
       _coinCurrencyViewPresenter = new CurrencyViewPresenter(_currencyService, _coinCurrencyView.CurrencyType, vibrationService, _coinCurrencyView.AnimationParameterConfig);
       _coinCurrencyView.Construct(_coinCurrencyViewPresenter); 
   }

   public override void OnCreate()
   {
       _restartButton.onClick.AddListener(G_RestartLevel);
       _menuButton.onClick.AddListener(G_MenuButtonHandler);
       
       _coinCurrencyView.Initialize();
       _coinCurrencyViewPresenter.Initialize();
       
       base.OnCreate();
   }

   public override UniTask OnOpenStart()
   {
       //var levelPackId = _levelPackService.LevelPackIndex + 1;
       //var levelId = _levelDataService.LevelId + 1;
       //_level.text = $"LEVEL  {_levelDataService.GetGlobalLevelId(levelPackId, levelId)} FAILED";
       
       _totalReward = GetTotalReward();
       ShowScoreText();
       ShowEarningText();
       _missionProgressBar.OnOpenStart(_missionService.GetCurrentMissionConfigs(), _missionService.GetActiveMissionsOnCurrentLevel());

       //OnOpenStartMissionProgressBar();
       
       //_missionProgressBarUI.OnOpenStart();
       return base.OnOpenStart();
   }
   /*private void OnOpenStartMissionProgressBar()
   {
       var missionsProgress = _missionService.GetCurrentMissionsProgress();
       var missionConfigs = _missionService.GetCurrentMissionConfigs();

       for (var i = 0; i < _missionProgressBarUI.Length; i++)
       {
           var missionProgressBarUI = _missionProgressBarUI[i];
           var missionProgress = missionsProgress[i];
           var missionConfig = missionConfigs[i];
            
           missionProgressBarUI.OnOpenStart(missionConfig);
       }
   }*/
   public override UniTask OnOpenComplete()
   {
       //_missionProgressBarUI.ShowFinalResult();
       _isContinueButtonPressed = false;
       //ShowFinalResultOnMissionProgressBar();
       
       var lastMission = _missionService.GetCurrentMissionsProgress().LastOrDefault();
       if (lastMission != null)
       {
           _missionProgressBar.UpdateProgressSmooth((int)lastMission.CurrentValue, lastMission.Mission.TargetValue);
           //_missionProgressBar.UpdateProgressSmooth(_missionService.GetTotalMissionsProgress(), _missionService.GetMaxMissionsProgress());
       }
       
       return base.OnOpenComplete();
   }

   /*private void ShowFinalResultOnMissionProgressBar()
   {
       var missionsProgress = _missionService.GetCurrentMissionsProgress();
       var missionConfigs = _missionService.GetCurrentMissionConfigs();
       for (var i = 0; i < _missionProgressBarUI.Length; i++)
       {
           var missionProgressBarUI = _missionProgressBarUI[i];
           var missionProgress = missionsProgress[i];
           var missionConfig = missionConfigs[i];
            
           missionProgressBarUI.ShowFinalResult(missionProgress, missionConfig);
       }
   }*/

   private void ShowScoreText()
   {
       var score = _scoreService.GetTotalScore();
       DOVirtual.Int(0, score, _scoreAnimation.Duration, value =>
       {
           _score.text = value.ToString("N0");
       }).SetEase(_scoreAnimation.Ease);
   }
   private void ShowEarningText()
   {
       DOVirtual.Int(0, _totalReward, _scoreAnimation.Duration, value =>
       {
           _earnings.text = value.ToString();
       }).SetEase(_scoreAnimation.Ease);
   }
   private int GetTotalReward()
   {
       var rewardForScore = _scoreService.GetRewardForScore(false);
       CustomDebugLog.Log("rewardForScore " + rewardForScore);
       return rewardForScore;
   }
   private void G_MenuButtonHandler()
   {
       if(_isContinueButtonPressed)
           return;
       _isContinueButtonPressed = true;
       
       AddCurrency();
       _analyticService.LogPlayerExitLevel(_levelDataService.LevelId, _levelPackService.LevelPackIndex, EExitLevelReason.LevelFail);
       DOVirtual.DelayedCall(_uiConfig.LevelCompleteScreenHideDelay, OpenMenu);
   }
   private void G_RestartLevel()
   {
       if(_isContinueButtonPressed)
           return;
       _isContinueButtonPressed = true;
       
       AddCurrency();
       _analyticService.LogLevelRetry(_levelDataService.LevelId, _levelPackService.LevelPackIndex, ERetryReason.AfterFail);
       DOVirtual.DelayedCall(_uiConfig.LevelCompleteScreenHideDelay, RestartLevel);
   }
   private void OpenMenu()
   {
       _levelStateMachine.FireTrigger(ELevelState.Exit);
       _gameStateMachine.FireTrigger(EGameState.Lobby);
   }
   private void RestartLevel()
    {
        _levelStateMachine.FireTrigger(ELevelState.Exit);
        _gameStateMachine.FireTrigger(EGameState.LevelLoading);
    }
    private void AddCurrency()
    {
        _uiFxService.SpawnCurrencyMovementEffect(_restartButton.transform, _coinCurrencyView.CurrencyIconRoot.transform, _coinCurrencyViewPresenter, ECurrencyType.Coins);
        DOVirtual.DelayedCall(_uiConfig.LevelCompleteScreenHideDelay / 2, () =>
        {
            _currencyService.AddCurrency(ECurrencyType.Coins, _totalReward);
            _analyticService.LogCurrencyEarned(_levelDataService.LevelId, _levelPackService.LevelPackIndex, ECurrencySource.LevelFailed, _totalReward);
        });
    }
}