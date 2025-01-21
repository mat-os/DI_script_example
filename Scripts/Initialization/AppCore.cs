using Configs;
using DG.Tweening;
using Game.Scripts.Debugging;
using Game.Scripts.Infrastructure.GameStateMachine;
using Game.Scripts.Infrastructure.Services;
using Game.Scripts.UI.Screens.Serviсes;
using Game.Scripts.UI.Widgets;
using Game.Scripts.Utils;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Initialization
{
    public class AppCore : MonoBehaviour
    {
        [SerializeField] private InputHandler _inputHandler;
        [SerializeField] private AutoMobileColorGrading _colorGrading;
        [SerializeField] private PostProcessor _postProcessor;

        private GameStateMachine _gameStateMachine;
        private ScreensService _screensService;
        private DebugCheatsLevel _debugCheatsLevel;
        private InputService _inputService;
        private LevelsRepository _levelsRepository;
        private LevelDataService _levelDataService;
        private ColorGradingService _colorGradingService;
        private DataService _dataService;
        private PostProcessingService _postProcessingService;

        [Inject]
        public void Construct
        (
            GameStateMachine gameStateMachine,
            ScreensService screensService,
            DebugCheatsLevel debugCheatsLevel,
            InputService inputService,
            LevelsRepository levelsRepository,
            LevelDataService levelDataService,
            ColorGradingService colorGradingService,
            DataService dataService,
            PostProcessingService postProcessingService)
        {
            _postProcessingService = postProcessingService;
            _dataService = dataService;
            _colorGradingService = colorGradingService;
            _levelDataService = levelDataService;
            _levelsRepository = levelsRepository;
            _inputService = inputService;
            _debugCheatsLevel = debugCheatsLevel;
            _gameStateMachine = gameStateMachine;
            _screensService = screensService;
        }

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        public void InitializeGame()
        {
            DOTween.Init();
            DOTween.SetTweensCapacity(500,125);

            if (Debug.isDebugBuild)
            {
                SRDebug.Init();
                SRDebug.Instance.AddOptionContainer(_debugCheatsLevel);
                _debugCheatsLevel.ShowFPS = true;
            }
            
            _screensService.OnGameStateEnter(_gameStateMachine.CurrentState);
            _inputService.SetInputHandler(_inputHandler);
            _colorGradingService.SetColorGrading(_colorGrading);
            _postProcessingService.SetPostProcessing(_postProcessor);
        }

        public void EnterGame()
        {
            if (_levelDataService.IsStartDebugLevel || IsStartFromLevelZero())
            {
                _gameStateMachine.FireTrigger(EGameState.LevelLoading);
            }
            else
            {
                _gameStateMachine.FireTrigger(EGameState.Lobby);
            }
        }

        private bool IsStartFromLevelZero()
        {
            return _levelDataService.IsStartFromLevelZero && _dataService.Level.IsCompleteLevelZero.Value == false;
        }

        public static bool IsMobilePlatform
        {
            get
            {
#if UNITY_EDITOR
                return UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.Android ||
                       UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.iOS;
#else
                return Application.isMobilePlatform;
#endif
            }
        }
    }
}