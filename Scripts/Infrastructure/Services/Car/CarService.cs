using System;
using System.Collections.Generic;
using System.Linq;
using Configs;
using Game.Scripts.Configs;
using Game.Scripts.Configs.Vfx;
using Game.Scripts.Core.Update;
using Game.Scripts.Core.Update.Interfaces;
using Game.Scripts.Customization;
using Game.Scripts.Customization.ItemConfigs;
using Game.Scripts.Infrastructure.Services.Level;
using Game.Scripts.Initialization;
using Game.Scripts.LevelElements.Car;
using Game.Scripts.LevelElements.Triggers;
using PG;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using Object = UnityEngine.Object;

namespace Game.Scripts.Infrastructure.Services.Car
{
    public class CarService : IDisposable
    {
        public Action<CarModel> OnCarModelCreated;
        public Action<CarView> OnCarViewCreated;
        public Action OnCarHitWall;

        private readonly PrefabRepository _prefabRepository;
        private readonly CarInputService _carInputService;
        private readonly GameConfig _gameConfig;
        
        private CarModel _carModel;
        private CarView _carView;

        private CameraService _cameraService;
        private readonly UpdateService _updateService;
        private readonly UpgradeService _upgradeService;
        private readonly VfxEffectsService _effectsService;


        [Inject]private DiContainer _container;

        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        private DifficultyService _difficultyService;
        private CustomizationShopService _customizationShopService;

        [Inject]
        public CarService(PrefabRepository prefabRepository, GameConfig gameConfig,
            UpdateService updateService, UpgradeService upgradeService, CameraService cameraService, 
            VfxEffectsService effectsService, DifficultyService difficultyService,
            CustomizationShopService customizationShopService)
        {
            _customizationShopService = customizationShopService;
            _difficultyService = difficultyService;
            _effectsService = effectsService;
            _cameraService = cameraService;
            _upgradeService = upgradeService;
            _updateService = updateService;
            _gameConfig = gameConfig;
            _prefabRepository = prefabRepository;

            _upgradeService.OnUpgradeCar += CarUpgradeHandle;
            
            GlobalEventSystem.Broker.Receive<CarEnterSpeedSlowDownTrigger>()
                .Subscribe(CarSpeedSlowDownTriggerHandler)
                .AddTo(_disposable);   
        }

        private void CarSpeedSlowDownTriggerHandler(CarEnterSpeedSlowDownTrigger slowDownTrigger)
        {
            _carModel.ApplyCollisionSlowdown(slowDownTrigger.SpeedBrakePercent);
            _carModel.ApplyDamage(slowDownTrigger);
        }

        private void CarUpgradeHandle()
        {
            if (_carModel != null)
            {
                var currentUpgradeStep = _upgradeService.GetCurrentUpgradeStep<CarUpgradeStep>(EUpgradeType.Car);
                _carModel.UpgradeCarParameters(currentUpgradeStep);
            }
        }

        public CarView CreateCar(ECarType carType, Transform playerRoot, float roadWidth, ShopCarCarColorItemConfig carColorItemConfig)
        {
            var carPrefab = _prefabRepository.Cars[carType];
            _carView = Object.Instantiate(carPrefab, playerRoot);
            _carView.transform.position = playerRoot.position;
            _carView.transform.rotation = playerRoot.rotation;
            OnCarViewCreated?.Invoke(_carView);

            var carUpgradeStep = _upgradeService.GetCurrentUpgradeStep<CarUpgradeStep>(EUpgradeType.Car);
            var difficultyCarAccelerationMultiplier = _difficultyService.GetCarAccelerationMultiplier();

            _carModel = new CarModel(
                _gameConfig.PlayerConfig.CarMovementConfig, 
                _carView, 
                carUpgradeStep, 
                difficultyCarAccelerationMultiplier,
                _cameraService, 
                _gameConfig.PlayerConfig.CarConfig,
                roadWidth);
            
            _container.Inject(_carModel);

           OnCarModelCreated?.Invoke(_carModel);
           Debug.Log("OnCarModelCreated");

           Debug.Log(_carView.CarCustomizationController);
           Debug.Log(carColorItemConfig);
           Debug.Log(carColorItemConfig.CarMaterial);
           _carView.CarCustomizationController.SetCarMaterial(carColorItemConfig.CarMaterial);
           Debug.Log("_carView.CarCustomizationController");

           _carView.WallHitTrigger.TriggerEnter += CarEnterWallTriggerHandle;
           Debug.Log("_carView.WallHitTrigger");

           _updateService.AddUpdateElement(_carModel);

           
            return _carView;
        }

        private void CarEnterWallTriggerHandle(TriggerComponent triggerComponent, Collider collider)
        {
            if (collider.CompareTag("Wall"))
            {
                OnCarHitWall?.Invoke();
                
                GlobalEventSystem.Broker.Publish(new PlayerCarHitWallEvent() { CarSpeed = GetCarSpeedKmh()});
                
                _carModel.DestroyCar();
                _effectsService.SpawnEffect(VfxEffectType.PlayerHitWallExplosion, _carView.ExplosionFxPosition.position);
            }
        }

        public float GetCarMaxSpeedKmh()
        {
            if (_carModel == null)
                return 0;
            
            return _carModel.GetCarMaxSpeedKmh();
        }

        public float GetCarSpeedKmh()
        {
            if (_carModel == null)
                return 0;
            
            return _carModel.GetCarSpeedKmh();
        }

        public void Clear()
        {
            if (_carModel != null)
            {
                _carView.WallHitTrigger.TriggerEnter -= CarEnterWallTriggerHandle;
                _updateService.RemoveUpdateElement(_carModel);
                _carModel.Clear();
                _carView = null;
                _carModel = null;
            }
        }

        public void Dispose()
        {
            Clear();
            
            OnCarHitWall = null;
            OnCarViewCreated = null;
            OnCarModelCreated = null;
            
            _disposable?.Dispose();
        }

        public void DebugDealCarDamage()
        {
            _carModel.ApplyDamageTest();
        }
    }
}