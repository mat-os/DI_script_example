using System.Linq;
using AssetKits.ParticleImage;
using Coffee.UIEffects;
using Configs;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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
using Game.Scripts.UI.Widgets.LevelComplete;
using Game.Scripts.UI.Widgets.RandomReview;
using Game.Scripts.Utils;
using Game.Scripts.Utils.Debug;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Zenject;

public class CompleteLevelPage : Page
{
    [SerializeField]private Button _nextLevelButton;
    [SerializeField]private Button _menuButton;
    [SerializeField]private Button _restartButton;
    [SerializeField]private ParticleImage _winFx;
    [SerializeField]private TMP_Text _levelInfo;
    
    [Header("Score")]
    [SerializeField]private TMP_Text _score;
    [SerializeField]private AnimationParameterConfig _scoreAnimation;
    
    [Header("Earning")]
    [SerializeField]private TMP_Text _earnings;

    [Header("Mission")]
    [SerializeField] private MissionProgressBarUIFinalScreen _missionProgressBar;
        
    [Header("Stars")]
    [SerializeField] private StarUI[] _bigStars;
    [SerializeField] private TMP_Text[] _rewardsForStars;
   // [SerializeField] private StarSimpleUI[] _progressBarStars;

   [Header("Trophy")] 
   [SerializeField] private TrophyOnLevelCompleteUI _trophyUI;
   
   [Header("Review")]
   [SerializeField] private RandomReviewUI _randomReviewUI;

    [Header("Currency View")]
    [SerializeField] private CurrencyView _coinCurrencyView;
    private CurrencyViewPresenter _coinCurrencyViewPresenter;

    private float _skipButtonDelay;

    private UIFXService _uiFxService;
    private GameConfig _gameConfig;
    private PlayerService _playerService;
    private LevelStateMachine _levelStateMachine;
    private GameStateMachine _gameStateMachine;
    private LevelDataService _levelDataService;
    private PlayerScoreService _scoreService;
    private LevelStarsService _levelStarsService;
    private LevelsProgressService _levelsProgressService;
    private LevelPackService _levelPackService;
    
    private int _totalReward;
    
    private CurrencyService _currencyService;
    private DataService _dataService;
    private UIConfig _uiConfig;
    private bool _isContinueButtonPressed;
    
    [Inject]private DiContainer _container;
    private int _currentPackId;
    private int _currentLevelId;
    private LevelProgress _nextLevel;
    private LevelTrophyService _levelTrophyService;
    private AnalyticService _analyticService;

     private string _freezeReason = "WinScreenReward";
     private MissionService _missionService;

     [Inject]
    public void Construct
    (
        GameConfig gameConfig,
        UIFXService uifxService,
        PlayerService playerService,
        CurrencyService currencyService,
        VibrationService vibrationService,
        LevelStateMachine levelStateMachine,
        GameStateMachine gameStateMachine,
        PageService pageService,
        DataService dataService,
        UIConfig uiConfig,
        LevelDataService levelDataService,
        PlayerScoreService scoreService,
        LevelStarsService levelStarsService,
        LevelsProgressService levelsProgressService,
        LevelPackService levelPackService,
        LevelTrophyService levelTrophyService,
        AnalyticService analyticService,
        MissionService missionService)
    {
        _missionService = missionService;
        _analyticService = analyticService;
        _levelTrophyService = levelTrophyService;
        _levelPackService = levelPackService;
        _levelsProgressService = levelsProgressService;
        _levelStarsService = levelStarsService;
        _scoreService = scoreService;
        _levelDataService = levelDataService;
        _uiConfig = uiConfig;
        _gameStateMachine = gameStateMachine;
        _levelStateMachine = levelStateMachine;
        _gameConfig = gameConfig;
        _uiFxService = uifxService;
        _playerService = playerService;
        _currencyService = currencyService;
        _dataService = dataService;

        foreach (var starUI in _bigStars)
        {
            starUI.Initialize(_uiConfig);
        }
        _container.Inject(_missionProgressBar);
        _container.Inject(_trophyUI);
        _container.Inject(_randomReviewUI);

        _coinCurrencyViewPresenter = new CurrencyViewPresenter(currencyService, _coinCurrencyView.CurrencyType, vibrationService, _coinCurrencyView.AnimationParameterConfig);
        _coinCurrencyView.Construct(_coinCurrencyViewPresenter);
    }

    public override void OnCreate()
    {
        base.OnCreate();
        _nextLevelButton.onClick.AddListener(NextLevelButtonClickHandle);
        _menuButton.onClick.AddListener(MenuButtonHandler);
        _restartButton.onClick.AddListener(RestartLevelButtonClickHandle);

        /*foreach (var missionProgressBarUI in _missionProgressBarUI)
        {
            missionProgressBarUI.OnStarActivated += OnStarActivatedHandler;
        }*/
        
        _coinCurrencyView.Initialize();
        _coinCurrencyViewPresenter.Initialize();
        
    }

    public override UniTask OnOpenStart()
    {
        //TODO: ЧЗХ тут с индексами левел паков?
        var currentLevelPack = _levelPackService.GetCurrentLevelPack();
        _currentPackId = currentLevelPack.GetPackId();
        var levelPackIndex = _levelPackService.LevelPackIndex;

        _currentLevelId = _levelDataService.LevelId;
        _levelInfo.text = $"LEVEL  {_levelDataService.GetGlobalLevelId(_currentLevelId, levelPackIndex)}";
        
        _totalReward = GetTotalReward();
        
        _coinCurrencyViewPresenter.FreezeRefresh(_freezeReason);
        _currencyService.AddCurrency(ECurrencyType.Coins, _totalReward);
        
        SetupStarRewards();
        _missionProgressBar.OnOpenStart(_missionService.GetCurrentMissionConfigs(), _missionService.GetActiveMissionsOnCurrentLevel());

        //ShowSkipButton();
        SetupButtonsByLevelState();

        var levelData = _levelDataService.GetCurrentLevelData();
        var isNeedReview = levelData.IsNeedReviewOnThisLevel;
        if (isNeedReview)
        {
            _randomReviewUI.Activate();
            _randomReviewUI.OnOpenStart(levelData.LevelName);
            _randomReviewUI.OnPlayerCloseIt += HandlePlayerCloseReviewUI;
        }
        else
        {
            _randomReviewUI.Deactivate();
            SetupTrophyScreen();
        }
        
        return base.OnOpenStart();
    }

    private void SetupTrophyScreen()
    {
        var isTrophyCollected = _levelTrophyService.IsTrophyCollected;
        _trophyUI.SetupTrophy(isTrophyCollected);
        if (isTrophyCollected)
        {
            _trophyUI.OnTrophyAnimationComplete += HandleTrophyAnimationComplete;
            _nextLevelButton.interactable = false;
        }
        else
        {
            PlayRewardAnimations();
            _nextLevelButton.interactable = true;
        }
    }

    private void HandlePlayerCloseReviewUI()
    {
        _randomReviewUI.OnPlayerCloseIt -= HandlePlayerCloseReviewUI;
        SetupTrophyScreen();
    }

    private void SetupButtonsByLevelState()
    {
        if (_dataService.Level.TotalLevelComplete.Value == 1)
        {
            _menuButton.gameObject.SetActive(false);
            _restartButton.gameObject.SetActive(false);
        }
        else
        {
            _menuButton.gameObject.SetActive(true);
            _restartButton.gameObject.SetActive(true);
        }
    }

    private void PlayRewardAnimations()
    {
        SetupBigStars();
        ShowScoreText();
        ShowEarningText(0, _totalReward);
        _nextLevelButton.interactable = true;
        var lastMission = _missionService.GetCurrentMissionsProgress().LastOrDefault();
        if (lastMission != null)
        {
            _missionProgressBar.UpdateProgressSmooth((int)lastMission.CurrentValue, lastMission.Mission.TargetValue);
            //_missionProgressBar.UpdateProgressSmooth(_missionService.GetTotalMissionsProgress(), _missionService.GetMaxMissionsProgress());
        }
    }

    private void HandleTrophyAnimationComplete()
    {
        PlayRewardAnimations();
        _nextLevelButton.interactable = true;
        _trophyUI.OnTrophyAnimationComplete -= HandleTrophyAnimationComplete;
    }

    public override UniTask OnOpenComplete()
    {
        _winFx.SetActive(true);
        _winFx.Play();
        
        _isContinueButtonPressed = false;
        
        //ShowFinalResultOnMissionProgressBar();

        return base.OnOpenComplete();
    }

    private void SetupStarRewards()
    {
        //TODO:
        var levelMissions = _levelDataService.GetCurrentLevelData().LevelMissions;
        for (int i = 0; i < _rewardsForStars.Length; i++)
        {
            _rewardsForStars[i].text = "+" + levelMissions[i].Reward.ToString();
        }
    }

    private void SetupBigStars()
    {
        var missionsProgress = _missionService.GetCurrentMissionsProgress();
        var missionConfigs = _missionService.GetCurrentMissionConfigs();

        for (var i = 0; i < _bigStars.Length; i++)
        {
            var starUI = _bigStars[i];
            var missionProgress = missionsProgress[i];
            var missionConfig = missionConfigs[i];
            
            if (missionProgress.CurrentValue >= missionConfig.TargetValue)
            {
                starUI.PlayActivateAnimation();
            }
            else
            {
                starUI.Deactivate();
            }
        }
    }

    private int GetTotalReward()
    {
        var rewardForScore = _scoreService.GetRewardForScore(true);
        var rewardForStars = _levelStarsService.GetRewardForStars();
        CustomDebugLog.Log("rewardForScore " + rewardForScore + " and  rewardForStars " + rewardForStars);
         return rewardForScore + rewardForStars;
    }

    private void ShowEarningText(int startAmount, int targetAmount)
    {
        DOVirtual.Int(startAmount, targetAmount, _scoreAnimation.Duration, value =>
        {
            _earnings.text = "+" + value.ToString();
        }).SetEase(_scoreAnimation.Ease);
    }

    private void ShowScoreText()
    {
        var score = _scoreService.GetTotalScore();
        DOVirtual.Int(0, score, _scoreAnimation.Duration, value =>
        {
            _score.text = value.ToString("N0");
        }).SetEase(_scoreAnimation.Ease);
    }

    public override UniTask PlayOpenAnimation()
    {
        base.PlayOpenAnimation();
        
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, _uiConfig.OpenLevelCompleteUIAnimationParameters.Duration)
            .SetEase(_uiConfig.OpenLevelCompleteUIAnimationParameters.Ease);

        return new UniTask();
    }
    
    public override UniTask OnCloseComplete()
    {
        _winFx.Stop(true);
        _winFx.SetActive(false);

        foreach (var starUI in _bigStars)
        {
            starUI.Deactivate();
        }
        return base.OnCloseComplete();
    }
    
    public void NextLevelButtonClickHandle()
    {
        if(_isContinueButtonPressed)
            return;
        
        _nextLevelButton.interactable = false;
        _nextLevel = _levelsProgressService.GetNextLevel(_currentPackId, _currentLevelId);
        if (_dataService.Level.TotalLevelComplete.Value == 1)
        {
            ExitToMenu();
        }
        else if (_nextLevel.LevelIndex == 0)
        {
            _dataService.Level.LevelPackIndex.Value = _nextLevel.PackId;
            ExitToMenu();
        }
        else if (_nextLevel != null)
        {
            ContinueGame();
        }
        else
        {
            CustomDebugLog.Log("No more levels to play!");
            ExitToMenu();
        }
        
        //_audioService.PlaySoundOneShot(SoundFxConstants.UI_TAP_GREEN_BUTTON);
        _isContinueButtonPressed = true;
    }

    private void ContinueGame()
    {
        AddCurrency();
        _analyticService.LogCurrencyEarned(_levelDataService.LevelId, _levelPackService.LevelPackIndex, ECurrencySource.LevelCompleted, _totalReward);

        DOVirtual.DelayedCall(_uiConfig.LevelCompleteScreenHideDelay, LoadNextLevel);
        
        _levelPackService.SetLevelPackByIndex(_nextLevel.PackId);
        _levelDataService.SetCurrentLevelData(_nextLevel.LevelIndex);
        
        CustomDebugLog.Log($"Next level is Pack {_nextLevel.PackId}, Level {_nextLevel.LevelIndex}");
    }

    public void LoadNextLevel()
    {
        CustomDebugLog.Log("[UI] Exit Level");
        _levelStateMachine.FireTrigger(ELevelState.Exit);
        _gameStateMachine.FireTrigger(EGameState.LevelLoading);
    }
    private void MenuButtonHandler()
    {
        if(_isContinueButtonPressed)
            return;
        
        _nextLevelButton.interactable = false;

        AddCurrency();
        _analyticService.LogCurrencyEarned(_levelDataService.LevelId, _levelPackService.LevelPackIndex, ECurrencySource.LevelCompleted, _totalReward);
        _analyticService.LogPlayerExitLevel(_levelDataService.LevelId, _levelPackService.LevelPackIndex, EExitLevelReason.LevelComplete);

        DOVirtual.DelayedCall(_uiConfig.LevelCompleteScreenHideDelay, ExitToMenu);
        _isContinueButtonPressed = true;
    }

    private void ExitToMenu()
    {
        _gameStateMachine.FireTrigger(EGameState.Lobby);
        _levelStateMachine.FireTrigger(ELevelState.Exit);
    }
    private void RestartLevelButtonClickHandle()
    {
        _levelDataService.RestartLevel();
        
        if(_isContinueButtonPressed)
            return;
        
        _nextLevelButton.interactable = false;
        
        AddCurrency();
        _analyticService.LogCurrencyEarned(_levelDataService.LevelId, _levelPackService.LevelPackIndex, ECurrencySource.LevelCompleted, _totalReward);
        _analyticService.LogLevelRetry(_levelDataService.LevelId, _levelPackService.LevelPackIndex, ERetryReason.AfterWin);

        _levelPackService.SetLevelPackByIndex(_currentPackId);
        _levelDataService.SetCurrentLevelData(_currentLevelId);
        
        DOVirtual.DelayedCall(_uiConfig.LevelCompleteScreenHideDelay, LoadNextLevel);
        
        //_audioService.PlaySoundOneShot(SoundFxConstants.UI_TAP_GREEN_BUTTON);
        _isContinueButtonPressed = true;
    }

    private void AddCurrency()
    {
        ShowEarningText(_totalReward, 0);
        _uiFxService.SpawnCurrencyMovementEffect(_nextLevelButton.transform, _coinCurrencyView.CurrencyIconRoot.transform, _coinCurrencyViewPresenter, ECurrencyType.Coins);
        DOVirtual.DelayedCall(_uiConfig.LevelCompleteScreenHideDelay / 2, () =>
        {
            _coinCurrencyViewPresenter.UnfreezeRefresh(_freezeReason, true);
            //_currencyService.AddCurrency(ECurrencyType.Coins, _totalReward);
        });
    }
    
    protected override void OnDestroy()
    {
        base.OnDestroy();
        _nextLevelButton.onClick.RemoveListener(NextLevelButtonClickHandle);
        _menuButton.onClick.RemoveListener(MenuButtonHandler);
        _restartButton.onClick.RemoveListener(RestartLevelButtonClickHandle);
        
        /*foreach (var missionProgressBarUI in _missionProgressBarUI)
        {
            missionProgressBarUI.OnStarActivated -= OnStarActivatedHandler;
        }*/
    }
    
    
    /*private void ShowSkipButton()
{
    _nextLevelButton.interactable = true;
    _nextLevelButton.gameObject.SetActive(false);
    CancelInvoke(nameof(EnableSkip));
    Invoke(nameof(EnableSkip), _skipButtonDelay);
}

private void EnableSkip()
{
    _nextLevelButton.gameObject.SetActive(true);
}*/
}
