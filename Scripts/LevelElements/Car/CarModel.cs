using System;
using Configs;
using Game.Scripts.Configs.Player;
using Game.Scripts.Core.Update.Interfaces;
using Game.Scripts.Infrastructure;
using Game.Scripts.Infrastructure.Services;
using Game.Scripts.Infrastructure.Services.Car;
using Game.Scripts.LevelElements.Car.CarMovement;
using PG;
using UnityEngine;
using Zenject;

namespace Game.Scripts.LevelElements.Car
{
    public class CarModel : IDisposable, IUpdate
    {
        private readonly float _forwardForce;
        private readonly float _steeringSensitivity;
        private readonly float _returnSpeed;
        private readonly CarMovementConfig _carMovementConfig;
        private VfxEffectsService _vfxEffectsService;
        private CameraService _cameraService;

        private CarView _carView;
        private ICarMovement _carMovement;
        private CarBoostController _carBoostController;
        private CarWheelController _carWheelController;
        private CarHealth _carHealth;
        private CarWheelsTrailController _carWheelsTrailController;
        private CarConfig _carConfig;


        [Inject]
        public void Construct(CameraService cameraService, VfxEffectsService vfxEffectsService)
        {
            _cameraService = cameraService;
            _vfxEffectsService = vfxEffectsService;
        }
        public CarModel(CarMovementConfig carMovementConfig, CarView carView, CarUpgradeStep carUpgradeStep,
            float difficultyCarAccelerationMultiplier, CameraService cameraService, CarConfig carConfig,
            float roadWidth)
        {
            _carConfig = carConfig;
            _carMovementConfig = carMovementConfig;
            _carView = carView;

            _carMovement = new BasicCarMovement(_carView, carMovementConfig, carUpgradeStep,
                difficultyCarAccelerationMultiplier, roadWidth);
            _carBoostController = new CarBoostController(this, carMovementConfig, _carView, cameraService);
            _carWheelController = new CarWheelController(_carView, carMovementConfig, _carMovement);
            _carHealth = new CarHealth(carConfig.MaxHealth, _carView);
            _carWheelsTrailController = new CarWheelsTrailController(carView);
        }
        public void ManualUpdate(float deltaTime)
        {
            _carWheelController.RotateWheels(deltaTime);
            
            _cameraService.HandleCarSpeedChange(GetCarSpeedKmh(), GetCarMaxSpeedKmh());
            _vfxEffectsService.HandleCarSpeedChange(GetCarSpeedKmh());

            var hasSlip = Mathf.Abs(_carMovement.GetHorizontalInertia()) >= _carConfig.MinInertiaForWheelTrail;
            _carWheelsTrailController.UpdateTrail(hasSlip);
        }
        public void ManualFixedUpdate(Vector3 inputVector)
        {
            _carMovement.Move(inputVector);
            _carMovement.Rotate(inputVector);
            
            _carBoostController.ManualFixedUpdate();
        }
        public void SetSpeedMultiplier(float multiplier)
        {
            _carMovement.SetSpeedMultiplier(multiplier);
        }
        // Переводим скорость в км/ч
        public float GetCarSpeedKmh()
        {
            return _carMovement.GetSpeed() * 3.6f;;
        }
        public float GetCarMaxSpeedKmh()
        {
            return _carMovement.GetMaxSpeed() * 3.6f;;
        }

        public void UpgradeCarParameters(CarUpgradeStep carUpgradeStep)
        {
            _carMovement.UpgradeCarParameters(carUpgradeStep);
        }

        public void ApplyCollisionSlowdown(float percent)
        {
            _carMovement.ApplyCollisionSlowdown(percent);
        }
        public void Clear()
        {
            _carBoostController.Dispose();
        }

        public void Dispose()
        {
            _carMovement = null;
            _carView = null;
            _carBoostController = null;
            _carWheelsTrailController = null;
        }


        public void ApplyDamage(CarEnterSpeedSlowDownTrigger slowDownTrigger)
        {
            _carHealth.ApplyDamage(slowDownTrigger.CarDamage, slowDownTrigger.Collision);
        }
        public void ApplyDamageTest()
        {
            _carHealth.ApplyDamageTest();
        }

        public void DestroyCar()
        {
            _carHealth.DestroyCar();
        }

        public void ApplyImmediateSpeedBoost(float immediateBoostSpeedAmount)
        {
            _carMovement.ApplyImmediateSpeedBoost(immediateBoostSpeedAmount);
        }
    }
}