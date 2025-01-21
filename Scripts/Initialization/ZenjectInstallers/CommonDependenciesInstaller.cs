using Game.Scripts.Configs.Level;
using Game.Scripts.Core.CurrencyService;
using Game.Scripts.Core.Update;
using Game.Scripts.Customization;
using Game.Scripts.Debugging;
using Game.Scripts.Infrastructure;
using Game.Scripts.Infrastructure.Services;
using Game.Scripts.Infrastructure.Services.Advertisement;
using Game.Scripts.Infrastructure.Services.Analytics;
using Game.Scripts.Infrastructure.Services.Car;
using Game.Scripts.Infrastructure.Services.Dialogue;
using Game.Scripts.Infrastructure.Services.Level;
using Game.Scripts.Infrastructure.Services.NPC;
using Game.Scripts.Infrastructure.Services.Player;
using Game.Scripts.Infrastructure.Services.Tutorial;
using Game.Scripts.Infrastructure.States;
using Game.Scripts.UI.FxService;
using Game.Scripts.UI.Screens.Serviсes;
using Game.Scripts.UI.Widgets.RandomReview;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Initialization.ZenjectInstallers
{
    public class CommonDependenciesInstaller : MonoInstaller
    {
        [SerializeField] private PageService _pageService;
        [SerializeField] private PopupService _popupService;
        [SerializeField] private MessageBoxService _messageBoxService;
        [SerializeField] private UpdateService _updateService;
        [SerializeField] private UIGarageService _garageService;
        [SerializeField] private FungusDialogueService _fungusDialogueService;
        [SerializeField] private CutsceneService _cutsceneService;
        [SerializeField] private VfxEffectsService _vfxEffectsService;

        public override void InstallBindings()
        {
            #region UI
            
            Container.Bind<PageService>().FromInstance(_pageService).AsSingle();
            Container.Bind<PopupService>().FromInstance(_popupService).AsSingle();
            Container.Bind<MessageBoxService>().FromInstance(_messageBoxService).AsSingle();
            Container.Bind<ScreensService>().AsSingle();
            Container.Bind<UIFXService>().AsSingle();
            Container.Bind<UIGarageService>().FromInstance(_garageService).AsSingle();
            Container.Bind<FungusDialogueService>().FromInstance(_fungusDialogueService).AsSingle();
            Container.Bind<RandomReviewService>().AsSingle();

            #endregion

            #region Level
            
            Container.Bind<CameraService>().AsSingle();
            Container.Bind<LevelStarsService>().AsSingle();
            Container.Bind<LevelDataService>().AsSingle();
            Container.BindInterfacesAndSelfTo<LevelsProgressService>().AsSingle();
            Container.Bind<LevelEnvironmentService>().AsSingle();
            Container.BindInterfacesAndSelfTo<LevelPackService>().AsSingle();
            Container.BindInterfacesAndSelfTo<CameraStateService>().AsSingle();
            
            Container.Bind<MissionService>().AsSingle();
            Container.Bind<TargetHitMissionService>().AsSingle();
            Container.Bind<BowlingMissionService>().AsSingle();
            Container.Bind<DestroyCarMissionService>().AsSingle();
            Container.Bind<ObjectsDestroyMissionService>().AsSingle();
            Container.Bind<PeopleHitMissionService>().AsSingle();
            
            Container.Bind<DamageTextEffectService>().AsSingle();
            Container.Bind<ScoreTextEffectService>().AsSingle();
            Container.Bind<LevelTrophyService>().AsSingle();
            Container.Bind<GroundParticleSpawnService>().AsSingle();

            Container.BindInterfacesAndSelfTo<CarNpcService>().AsSingle();
            
            Container.BindInterfacesAndSelfTo<TutorialService>().AsSingle();
            
            Container.Bind<CutsceneService>().FromInstance(_cutsceneService).AsSingle();
            #endregion

            #region Core
            Container.Bind<GlobalEventSystem>().AsSingle();

            Container.Bind<UpdateService>().FromInstance(_updateService).AsSingle();
            Container.BindInterfacesAndSelfTo<VfxEffectsService>().FromInstance(_vfxEffectsService).AsSingle();
            
            Container.Bind<CurrencyService>().AsSingle();
            Container.Bind<InputService>().AsSingle();
            
            Container.Bind<LevelBuilderService>().AsSingle();
            
            Container.Bind<DataService>().AsSingle();
            
            Container.Bind<SlowMotionService>().AsSingle();
            
            Container.Bind<AnalyticService>().AsSingle();
            Container.BindInterfacesAndSelfTo<VibrationService>().AsSingle();
            
            Container.BindInterfacesAndSelfTo<ColorGradingService>().AsSingle();
            Container.BindInterfacesAndSelfTo<PostProcessingService>().AsSingle();
            
            #endregion

            #region Ads
            
            Container.BindInterfacesAndSelfTo<FakeAdvertisementService>().AsSingle();
           // Container.Bind<AdvertisementService>().AsSingle();
           // Container.Bind<AdvertisementTimer>().AsSingle();

            #endregion

            #region Player
            
            Container.Bind<DifficultyService>().AsSingle();
            Container.Bind<PlayerService>().AsSingle();
            Container.Bind<UpgradeService>().AsSingle();
            Container.Bind<PlayerDamageService>().AsSingle();
            Container.Bind<CarService>().AsSingle();
            Container.Bind<CarInputService>().AsSingle();
            Container.Bind<EnergyService>().AsSingle();
            Container.Bind<PlayerSwipeService>().AsSingle();
            Container.Bind<PlayerDragControlService>().AsSingle();
            Container.Bind<PlayerJumpService>().AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerBoneDisplayService>().AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerCollisionPhysicsService>().AsSingle();
            
            Container.BindInterfacesAndSelfTo<PlayerFlightControlService>().AsSingle();
            Container.Bind<PlayerFlightLandingService>().AsSingle();
            Container.Bind<PlayerFlightLaunchService>().AsSingle();
            Container.Bind<PlayerFlightTrackerService>().AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerFlightLineDirectionService>().AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerLaunchTapGameService>().AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerAirTimeCounterService>().AsSingle();
            
            Container.Bind<PlayerScoreService>().AsSingle();
            Container.Bind<PlayerComboService>().AsSingle();
            Container.Bind<PraiseService>().AsSingle();
            
            Container.Bind<CustomizationShopService>().AsSingle();
            
            #endregion

            #region Debug
            
            Container.BindInterfacesAndSelfTo<DebugCheatsLevel>().AsSingle();

            #endregion        
            
            #region Missions
            
            Container.BindInterfacesAndSelfTo<GlobalMissionProgressService>().AsSingle();
            Container.BindInterfacesAndSelfTo<GlobalMissionService>().AsSingle();

            #endregion
        }
    }
}