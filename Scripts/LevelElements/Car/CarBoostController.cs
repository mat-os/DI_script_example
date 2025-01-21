using System;
using Configs;
using Game.Scripts.Configs.Player;
using Game.Scripts.Infrastructure;
using Game.Scripts.Infrastructure.Services;
using UniRx;
using UnityEngine;

namespace Game.Scripts.LevelElements.Car
{
    public class CarBoostController : IDisposable
    {
        private readonly CarMovementConfig _carMovementConfig;

        private float _boostElapsedTime;
        private bool _isBoostActive;

        private CarModel _carModel;
        private CarView _carView;

        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        private CameraService _cameraService;


        public CarBoostController(CarModel carModel, CarMovementConfig carMovementConfig, CarView carView, CameraService cameraService)
        {
            _cameraService = cameraService;
            _carView = carView;
            _carMovementConfig = carMovementConfig;
            _carModel = carModel;
            
            GlobalEventSystem.Broker.Receive<PlayerCarEnterBoosterZoneEvent>()
                .Subscribe(ApplyBoost)
                .AddTo(_disposable);
        }

        public void ApplyBoost(PlayerCarEnterBoosterZoneEvent booster)
        {
            _boostElapsedTime = 0f; 
            _isBoostActive = true;
            _cameraService.ChangeCameraFovOnBoost();
            _carModel.ApplyImmediateSpeedBoost(_carMovementConfig.ImmediateBoostSpeedAmount);
            SetIsInBoost(true);
        }

        public void ManualFixedUpdate()
        {
            if (_isBoostActive)
            {
                _boostElapsedTime += Time.fixedDeltaTime;
                if (_boostElapsedTime <= _carMovementConfig.BoostDuration)
                {
                    float normalizedTime = _boostElapsedTime / _carMovementConfig.BoostDuration;
                    float curveValue = _carMovementConfig.BoostIntensityCurve.Evaluate(normalizedTime);
                    float currentMultiplier = Mathf.Lerp(1f, _carMovementConfig.BoostBaseMultiplier, curveValue);

                    _carModel.SetSpeedMultiplier(currentMultiplier);
                }
                else
                {
                    _isBoostActive = false;
                    SetIsInBoost(false);
                    _carModel.SetSpeedMultiplier(1f); // Reset to normal speed
                    _cameraService.ResetCameraFov();
                }
            }
        }
        public void SetIsInBoost(bool isInBoost)
        {
            if(isInBoost)
                _carView.CarVfxView.BoostPs.Play();
            else
                _carView.CarVfxView.BoostPs.Stop();
        }
        public void Dispose()
        {
            _cameraService = null;
            _carModel = null;
            _carView = null;
            _disposable?.Dispose();
        }
    }
}