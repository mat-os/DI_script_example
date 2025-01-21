using System;
using Configs;
using Cysharp.Threading.Tasks;
using Game.Scripts.Configs.Level;
using Game.Scripts.Customization;
using Game.Scripts.Infrastructure.Services.Car;
using Game.Scripts.Infrastructure.Services.Level;
using Game.Scripts.Infrastructure.Services.Player;
using Game.Scripts.Infrastructure.States;
using Game.Scripts.LevelElements;
using Game.Scripts.LevelElements.Triggers;
using Game.Scripts.Utils.Debug;
using PG;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Scripts.Infrastructure.Services
{
    public class LevelBuilderService : IDisposable
    {
        public event Action OnLevelCreated;
        public event Action OnLevelRemoved;
        
        private readonly CarService _carService;
        private readonly CameraService _cameraService;
        private readonly PlayerService _playerService;
        private readonly SlowMotionService _slowMotionService;
        private readonly LevelEnvironmentService _levelEnvironmentService;
        private readonly LevelDataService _levelDataService;
        
        private LevelDataConfig _currentLevelDataConfig;
        private LevelView _levelView;
        private VfxEffectsService _vfxEffectsService;
        private CutsceneService _cutsceneService;
        private LevelTrophyService _levelTrophyService;
        private CustomizationShopService _customizationShopService;

        public bool IsReady { get; private set; }

        public LevelView LevelView
        {
            get
            {
                if (_levelView != null)
                    return _levelView;

                _levelView = Object.FindObjectOfType<LevelView>();

                if (_levelView == null)
                {
                    CustomDebugLog.LogError($"No level view");
                    return null;
                }

                return _levelView;
            }
        }

        public LevelBuilderService(
            LevelDataService levelDataService,
            CarService carService,
            CameraService cameraService,
            PlayerService playerService,
            SlowMotionService slowMotionService,
            LevelEnvironmentService levelEnvironmentService,
            VfxEffectsService vfxEffectsService,
            CutsceneService cutsceneService,
            LevelTrophyService levelTrophyService,
            CustomizationShopService customizationShopService)
        {
            _customizationShopService = customizationShopService;
            _levelTrophyService = levelTrophyService;
            _cutsceneService = cutsceneService;
            _vfxEffectsService = vfxEffectsService;
            _levelEnvironmentService = levelEnvironmentService;
            _slowMotionService = slowMotionService;
            _playerService = playerService;
            _cameraService = cameraService;
            _carService = carService;
            _levelDataService = levelDataService;
        }
        
        public async UniTask CreateCurrentLevel()
        {
            IsReady = false;
            _currentLevelDataConfig = _levelDataService.GetCurrentLevelData();
            
            _levelView = Object.Instantiate(_currentLevelDataConfig.Level).GetComponent<LevelView>();
            var roadWidth = _currentLevelDataConfig.RoadWidth;
            var carModelType = _customizationShopService.GetCurrentCarModelConfig().CarType;
            var carColorConfig = _currentLevelDataConfig.IsSetCustomCarCustomization ? 
                _currentLevelDataConfig.CustomCarColorItemConfig 
                : _customizationShopService.GetCurrentCarColorConfig();

            Debug.Log(carColorConfig);

            var carView = _carService.CreateCar(carModelType, _levelView.PlayerRoot, roadWidth, carColorConfig);
            Debug.Log("_carService.CreateCar!");

            var playerView = _playerService.CreatePlayer(_levelView.PlayerRoot, carView.PlayerRoot);
            Debug.Log("_playerService.CreatePlayer!");

            _cameraService.SetLevelCameras(_levelView.LevelCameraView);
            _vfxEffectsService.SetWindFx(_levelView.WindFx);
            _cameraService.SetCar(carView.transform);
            _cameraService.SetPlayerHumanoid(playerView);
            _cutsceneService.SetCutsceneCameras(LevelView);
            Debug.Log("_cutsceneService.SetCutsceneCameras!");

            _levelTrophyService.TryActivateTrophyOnLevel();

            _levelEnvironmentService.SetupEnvironment(_currentLevelDataConfig.EnvironmentConfig);
            Debug.Log("_levelEnvironmentService.SetupEnvironment!");

            await UniTask.DelayFrame(1);

            GC.Collect();
            DynamicGI.UpdateEnvironment();

            Debug.Log("Level Created!");

            OnLevelCreated?.Invoke();
            IsReady = true;
        }
        
        public void Dispose()
        {
        }

        public void ClearLevel()
        {
            if(IsReady == false)
                return;
            
            _carService.Clear();
            _cutsceneService.Clear();
            Object.Destroy(_levelView.gameObject); 
            OnLevelRemoved?.Invoke();
        }
    }
}