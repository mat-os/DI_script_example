using System;
using Configs;
using DG.Tweening;
using Game.Scripts.Core.Update;
using Game.Scripts.Core.Update.Interfaces;
using Game.Scripts.Infrastructure.Services.Car;
using Game.Scripts.Infrastructure.Services.Level;
using Game.Scripts.Infrastructure.States;
using Game.Scripts.LevelElements;
using Game.Scripts.LevelElements.Car;
using Game.Scripts.LevelElements.Player;
using Game.Scripts.LevelElements.Triggers;
using Game.Scripts.Utils.Debug;
using PG;
using UnityEngine;

namespace Game.Scripts.Infrastructure.Services.Player
{
    public class PlayerFlightLaunchService : IDisposable
    {
       public Action OnPlayerFlyStart;
       public Action OnPlayerHitWall;

        private readonly PlayerService _playerService;
        private readonly GameConfig _gameConfig;
        private readonly CarService _carService;
        private readonly CameraService _cameraService;
        private readonly UpgradeService _upgradeService;
        private readonly VibrationService _vibrationService;
        private readonly LevelDataService _levelDataService;
        private PlayerLaunchTapGameService _tapGameService;
        private readonly DifficultyService _difficultyService;

        private PlayerHumanoid _player;
        private CarView _carView;

        private bool _isLaunched = false;
        private Vector3 _specialForceFromLevelData;

        public PlayerFlightLaunchService(PlayerService playerService,
            GameConfig gameConfig, 
            CarService carService,
            CameraService cameraService,
            UpgradeService upgradeService,
            VibrationService vibrationService,
            LevelDataService levelDataService,
            PlayerLaunchTapGameService tapGameService,
            DifficultyService difficultyService)
        {
            _difficultyService = difficultyService;
            _tapGameService = tapGameService;
            _levelDataService = levelDataService;
            _vibrationService = vibrationService;
            _upgradeService = upgradeService;
            _cameraService = cameraService;
            _carService = carService;
            _gameConfig = gameConfig;
            _playerService = playerService;
            
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            _playerService.OnPlayerHumanoidCreated += PlayerCreatedHandler;
            _carService.OnCarHitWall += CarEnterWallTriggerHandle;
            //_tapGameService.OnTapGameEnd += TapTapGameEndHandle;
        }

        private void TapTapGameEndHandle(float value)
        {
            /*if(_isLaunched)
                return;

            Debug.Log("TapTapGameEndHandle");
            OnPlayerHitWall?.Invoke();
            LaunchPlayerFromCar();
            _vibrationService.Vibrate(VibrationPlaceType.HitWall);
            _isLaunched = true;*/
        }
        
        private void PlayerCreatedHandler(PlayerHumanoid player)
        {
            _player = player;

            var specialLevelDataSettings = _levelDataService.GetCurrentLevelData().SpecialLevelDataSettings;
            if (specialLevelDataSettings.IsForceSetImpulseOnFlightLaunch)
            {
                _specialForceFromLevelData = specialLevelDataSettings.ImpulseOnFlightLaunch;
            }
        }

        private void CarEnterWallTriggerHandle()
        {
            if(_isLaunched)
                return;

            OnPlayerHitWall?.Invoke();
            
            //TODO:
            //_cameraService.SetActiveCamera(EGameCameraType.WallHit);
            //_cameraService.SetActiveCamera(EGameCameraType.Fly);
            LaunchPlayerFromCar();
            _vibrationService.Vibrate(VibrationPlaceType.HitWall);
            _isLaunched = true;
        }

        public void LaunchPlayerFromCar()
        {
            DOVirtual.DelayedCall(_gameConfig.PlayerConfig.FlyStartConfig.DelayBeforeLaunchPlayer, () =>
            {
                var launchForce = CountFlightLaunchForce();
                _player.AddLaunchForceImpulse(launchForce);
                
                _cameraService.SetActiveCamera(EGameCameraType.Fly);
                OnPlayerFlyStart?.Invoke();
            }).SetUpdate(true);
            
            //В начале прыжка чтоб ногами землю не зацепил
            DisablePlayerCollisions();
            CustomDebugLog.Log("[Player] Player launched!");
        }

        private Vector3 CountFlightLaunchForce()
        {
            if (_specialForceFromLevelData != Vector3.zero)
                return _specialForceFromLevelData;
            
            float carSpeed = _carService.GetCarSpeedKmh();
            StuntUpgradeStep currentUpgradeStep = _upgradeService.GetCurrentUpgradeStep<StuntUpgradeStep>(EUpgradeType.Stunt);
            
            float clampedSpeed = Mathf.Clamp(carSpeed, _gameConfig.PlayerConfig.FlyStartConfig.MinCollisionSpeed, _gameConfig.PlayerConfig.FlyStartConfig.MaxCollisionSpeed);

            // Calculate the multiplier based on the speed range
            float speedRange = _gameConfig.PlayerConfig.FlyStartConfig.MaxCollisionSpeed - _gameConfig.PlayerConfig.FlyStartConfig.MinCollisionSpeed;
            float forceMultiplier = (clampedSpeed - _gameConfig.PlayerConfig.FlyStartConfig.MinCollisionSpeed) / speedRange;

            // Interpolate the flight force between min and max forces based on the multiplier
            float flightForce = Mathf.Lerp(_gameConfig.PlayerConfig.FlyStartConfig.MinFlightForce, _gameConfig.PlayerConfig.FlyStartConfig.MaxFlightForce, forceMultiplier);

            // Calculate forward and upward forces using the interpolated flight force
            Vector3 forwardForce = _player.PlayerView.transform.forward * (_gameConfig.PlayerConfig.FlyStartConfig.FlyForwardForce * flightForce * currentUpgradeStep.FlyForceMultiplier);
            Vector3 upForce = _player.PlayerView.transform.up * (_gameConfig.PlayerConfig.FlyStartConfig.FlyUpForce * flightForce * currentUpgradeStep.FlyForceMultiplier);

            var difficultyMultiplier = _difficultyService.GetPlayerFlyForceMultiplier();
            return (forwardForce + upForce) * difficultyMultiplier;
        }

        private void DisablePlayerCollisions()
        {
            LayerCollisionHandler.TemporarilyIgnoreLayerCollision(
                LayerMask.NameToLayer("Player Humanoid"),
                LayerMask.NameToLayer("Ground"),
                0.5f + _gameConfig.PlayerConfig.FlyStartConfig.DelayBeforeLaunchPlayer
            );
            LayerCollisionHandler.TemporarilyIgnoreLayerCollision(
                LayerMask.NameToLayer("Player Humanoid"),
                LayerMask.NameToLayer("Default"),
                0.5f + _gameConfig.PlayerConfig.FlyStartConfig.DelayBeforeLaunchPlayer
            );
        }

        public void Clear()
        {
            _carView = null;
            _player = null;
            _isLaunched = false;
            _specialForceFromLevelData = Vector3.zero;
        }
        public void Dispose()
        {
            UnsubscribeEvents();
        }
        private void UnsubscribeEvents()
        {
            _playerService.OnPlayerHumanoidCreated -= PlayerCreatedHandler;
            _carService.OnCarHitWall -= CarEnterWallTriggerHandle;
        }
    }
}