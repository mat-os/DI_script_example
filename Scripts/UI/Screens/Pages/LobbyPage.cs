using UnityEngine;
using TMPro;
using Configs;
using Cysharp.Threading.Tasks;
using Game.Scripts.Core.CurrencyService;
using Game.Scripts.Infrastructure.GameStateMachine;
using Game.Scripts.Infrastructure.LevelStateMachin;
using Game.Scripts.Infrastructure.Services;
using Game.Scripts.Infrastructure.Services.Level;
using Game.Scripts.UI.Screens.Base.Screens;
using Game.Scripts.UI.Screens.Popups;
using Game.Scripts.UI.Screens.Serviсes;
using Game.Scripts.UI.Widgets;
using Game.Scripts.UI.Widgets.CurrencyCounter;
using UnityEngine.UI;
using Zenject;

public class LobbyPage : Page
{
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _dailyChallengeButton;
    [SerializeField] private LevelPackMenuUI _levelPackMenuUI ;
    [SerializeField] private GarageUI _garageUI ;
    [SerializeField] private LobbySelectPageUI _lobbySelectPageUI ;
    [SerializeField] private GlobalMissionsUI _globalMissionsUI ;

    [Header("Currency View")]
    [SerializeField] private CurrencyView _coinCurrencyView;
    private CurrencyViewPresenter _coinCurrencyViewPresenter;

    private GameStateMachine _gameStateMachine;
    private LevelStateMachine _levelStateMachine;
    private CurrencyService _currencyService;
    private DataService _dataService;
    private PopupService _popupService;
    private UIConfig _uiConfig;
    private LevelsRepository _levelsRepository;

    [Inject] private DiContainer _container;
    private LevelDataService _levelDataService;
    private LevelPackService _levelPackService;

    [Inject]
    public void Construct
    (
        DataService dataService,
        CurrencyService currencyService,
        LevelStateMachine levelStateMachine,
        GameStateMachine gameStateMachine,
        PopupService popupService,
        UIConfig uiConfig,
        LevelsRepository levelsRepository,
        VibrationService vibrationService,
        LevelDataService levelDataService,
        LevelPackService levelPackService)
    {
        _levelPackService = levelPackService;
        _levelDataService = levelDataService;
        _levelsRepository = levelsRepository;
        _uiConfig = uiConfig;
        _popupService = popupService;
        _dataService = dataService;
        _currencyService = currencyService;
        _levelStateMachine = levelStateMachine;
        _gameStateMachine = gameStateMachine;

        _coinCurrencyViewPresenter = new CurrencyViewPresenter(currencyService, _coinCurrencyView.CurrencyType, vibrationService, _coinCurrencyView.AnimationParameterConfig);
        _coinCurrencyView.Construct(_coinCurrencyViewPresenter);     
        
        _container.Inject(_garageUI);
    }

    public override void OnCreate()
    {
        base.OnCreate();
        
        _coinCurrencyView.Initialize();
        _coinCurrencyViewPresenter.Initialize();
        
        _levelPackMenuUI.GenerateLevelPacks();
        _lobbySelectPageUI.OnCreate();
        
        _garageUI.Initialize();
        
        _playButton.onClick.AddListener(G_StartLevel);
        _dailyChallengeButton.onClick.AddListener(G_StartDailyLevel);
        
        _globalMissionsUI.OnCreate();

        _currencyService.OnCurrencyChanged += PlayerCoinsCountChanged;
    }


    public override UniTask OnOpenStart()
    {
        //_popupService.ShowScreen<MissionStartInfoPopup>();
        _levelPackMenuUI.OnOpenStart();
        _garageUI.ShowGarageView();
        _lobbySelectPageUI.OnOpenStart();
        _globalMissionsUI.OnOpenStart();
        return base.OnOpenStart();
    }

    public override UniTask OnOpenComplete()
    {
        _levelPackMenuUI.GoToCurrentPanel();
        _globalMissionsUI.OnOpenComplete();
        return base.OnOpenComplete();
    }

    public override UniTask OnCloseComplete()
    {
        _garageUI.HideGarageView();
        _globalMissionsUI.OnCloseComplete();
        return base.OnCloseComplete();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        
        _playButton.onClick.RemoveListener(G_StartLevel);
        _currencyService.OnCurrencyChanged -= PlayerCoinsCountChanged;

        _coinCurrencyView.Dispose(); 
        //_diamondCurrencyView.Dispose();
    }

    private void PlayerCoinsCountChanged(ECurrencyType currencyType, int value)
    {
    }
    private void G_StartLevel()
    {
        _popupService.ShowScreen<LevelPackSelectPopup>();

        //_levelPackService.SetLevelPackByLastPlayedPack();
        //_levelDataService.SetLevelDataByLastPlayedPack();
        //_gameStateMachine.FireTrigger(EGameState.LevelLoading);
    }
    private void G_StartDailyLevel()
    {
        _levelDataService.SetDailyChallenge();
        _gameStateMachine.FireTrigger(EGameState.LevelLoading);
        
        //_popupService.ShowScreen<LevelPackSelectPopup>();

        /*_levelDataService.SetLevelByLastPlayedPack();
        _gameStateMachine.FireTrigger(EGameState.LevelLoading);*/
    }
}
