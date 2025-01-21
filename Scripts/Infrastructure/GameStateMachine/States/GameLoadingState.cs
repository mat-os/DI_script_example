using Configs;
using Cysharp.Threading.Tasks;
using Game.Scripts.Core.CurrencyService;
using Game.Scripts.Core.StateMachine;
using Game.Scripts.Customization;
using Game.Scripts.Infrastructure.Services;
using Game.Scripts.Infrastructure.Services.Analytics;

namespace Game.Scripts.Infrastructure.GameStateMachine.States
{
    public class GameLoadingState : State<EGameState>
    {
        private readonly DataService _dataService;
        
        private readonly LevelBuilderService _levelBuilderService;
        private readonly CurrencyService _currencyService;
        private AnalyticService _analyticService;
        private CustomizationShopService _customizationShopService;
        private GlobalMissionService _globalMissionService;

        public GameLoadingState(DataService dataService, 
            LevelBuilderService levelBuilderService, 
            CurrencyService currencyService,
            AnalyticService analyticService,
            CustomizationShopService customizationShopService,
            GlobalMissionService globalMissionService)
        {
            _globalMissionService = globalMissionService;
            _customizationShopService = customizationShopService;
            _analyticService = analyticService;
            _levelBuilderService = levelBuilderService;
            _currencyService = currencyService;
            _dataService = dataService;
        }

        public override void OnEnter(ITriggerResponder<EGameState> stateMachine)
        {
            base.OnEnter(stateMachine);
            
            _dataService.Load();
            _globalMissionService.InitializeMissions();
            _analyticService.Initialize();
            _currencyService.TryAddStartMoney();
            _customizationShopService.Initialize();
            
            _dataService.GameSettings.IsItFirstLaunch.Value = false;
            
            UniTask.WaitUntil(() => _levelBuilderService.IsReady);
        }

        public override void OnExit()
        {
            
            base.OnExit();
        }
    }
}