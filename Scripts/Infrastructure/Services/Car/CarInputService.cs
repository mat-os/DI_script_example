using System;
using Configs;
using Game.Scripts.Core.Update;
using Game.Scripts.Core.Update.Interfaces;
using Game.Scripts.Infrastructure.Services.Player;
using Game.Scripts.LevelElements.Car;
using Game.Scripts.Utils.Debug;
using PG;
using UnityEngine;

namespace Game.Scripts.Infrastructure.Services.Car
{
    public class CarInputService : IFixedUpdate, IDisposable
    {
        private readonly InputService _inputService;
        private readonly UpdateService _updateService;
        private readonly CarService _carService;
        private readonly PlayerFlightLaunchService _playerFlightLaunchService;
        private readonly PlayerConfig _playerConfig;

        private CarModel _carModel;
        private bool _isReadInput;

        public CarInputService(PlayerFlightLaunchService playerFlightLaunchService, InputService inputService, GameConfig gameConfig, UpdateService updateService, CarService carService)
        {
            _playerFlightLaunchService = playerFlightLaunchService;
            _carService = carService;
            _updateService = updateService;
            _inputService = inputService;
            
            _carService.OnCarHitWall += PlayerFlyStartHandler;
            _carService.OnCarModelCreated += CarModelCreatedHandler;

            _playerConfig = gameConfig.PlayerConfig;
            _updateService.AddFixedUpdateElement(this);
        }
        private void PlayerFlyStartHandler()
        {
            SetIsReadInput(false);
        }
        private void CarModelCreatedHandler(CarModel carModel)
        {
            _carModel = carModel;
        }
        public void SetIsReadInput(bool isReadInput)
        {
            CustomDebugLog.Log("[CAR INPUT] SetIsReadInput " +isReadInput);
            _isReadInput = isReadInput;
        }

        public void ManualFixedUpdate(float fixedDeltaTime)
        {
            if(_isReadInput == false)
                return;
            _carModel.ManualFixedUpdate(_inputService.InputVector);
        }

        public void Dispose()
        {
            _playerFlightLaunchService.OnPlayerFlyStart -= PlayerFlyStartHandler;
            _carService.OnCarModelCreated -= CarModelCreatedHandler;

            _updateService.RemoveFixedUpdateElement(this);
        }


    }
}