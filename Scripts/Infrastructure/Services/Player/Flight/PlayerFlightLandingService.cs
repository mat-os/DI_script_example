using System;
using Configs;
using Game.Scripts.Core.Update;
using Game.Scripts.Core.Update.Interfaces;
using Game.Scripts.Infrastructure.States;
using Game.Scripts.LevelElements.Player;
using Game.Scripts.Utils.Debug;
using UniRx;
using UnityEngine;

namespace Game.Scripts.Infrastructure.Services.Player
{
    public class PlayerFlightLandingService: IFixedUpdate, IDisposable
    {
        public Action OnPlayerFlyComplete;
        
        private readonly PlayerFlightLaunchService _playerFlightLaunchService;
        private readonly UpdateService _updateService;
        private readonly PlayerService _playerService;
        private readonly GameConfig _gameConfig;
        private readonly CameraService _cameraService;

        private PlayerHumanoid _player;
        private bool _isPlayerLaunched;
        private bool _isPlayerStopped;
        private float _timeSinceLaunch;
        private float _stopCheckTimer;

        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        public PlayerFlightLandingService(PlayerService playerService, GameConfig gameConfig, 
            CameraService cameraService, UpdateService updateService, PlayerFlightLaunchService playerFlightLaunchService)
        {
            _playerFlightLaunchService = playerFlightLaunchService;
            _cameraService = cameraService;
            _gameConfig = gameConfig;
            _playerService = playerService;
            _updateService = updateService;
            
            _updateService.AddFixedUpdateElement(this);
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            _playerService.OnPlayerHumanoidCreated += PlayerCreatedHandler;
            _playerFlightLaunchService.OnPlayerFlyStart += PlayerFlyStartHandler;
            
            GlobalEventSystem.Broker.Receive<PlayerEnterLevelCompleteTrigger>()
                .Subscribe(PlayerEnterLevelCompleteTriggerHandler)
                .AddTo(_disposable);
        }

        private void PlayerEnterLevelCompleteTriggerHandler(PlayerEnterLevelCompleteTrigger trigger)
        {
            CompleteFly();
        }

        private void PlayerFlyStartHandler()
        {
            _isPlayerLaunched = true;
        }

        private void PlayerCreatedHandler(PlayerHumanoid player)
        {
            _player = player;
        }

        public void ManualFixedUpdate(float fixedDeltaTime)
        {
            if (!_isPlayerLaunched)
                return;

            _timeSinceLaunch += fixedDeltaTime;
            CheckIsPlayerCompleteFlying(fixedDeltaTime);
        }

        private void CheckIsPlayerCompleteFlying(float fixedDeltaTime)
        {
            if (_timeSinceLaunch < _gameConfig.PlayerConfig.FlyEndConfig.InitialFlyCompleteCountDelay)
                return;

            Vector3 velocity = _player.PlayerView.RigidbodyRoot.velocity;
            Vector3 angularVelocity = _player.PlayerView.RigidbodyRoot.angularVelocity;

            bool isMoving = velocity.magnitude > _gameConfig.PlayerConfig.FlyEndConfig.SpeedThresholdToEndFly;
            bool isFalling = Mathf.Abs(velocity.y) > _gameConfig.PlayerConfig.FlyEndConfig.FallThresholdToEndFly;
            bool isRotating = angularVelocity.magnitude > _gameConfig.PlayerConfig.FlyEndConfig.AngularSpeedThresholdToEndFly;

            if (isMoving || isFalling || isRotating)
            {
                _stopCheckTimer = 0f;
                _isPlayerStopped = false;
                return;
            }

            _stopCheckTimer += fixedDeltaTime;

            if (_stopCheckTimer >= _gameConfig.PlayerConfig.FlyEndConfig.StopCheckDelay)
            {
                if (!_isPlayerStopped && Input.GetMouseButton(0) == false)
                {
                    CompleteFly();
                }
            }
        }

        private void CompleteFly()
        {
            CustomDebugLog.Log("[Player Fly] CompleteFly");
            _isPlayerStopped = true;
            _isPlayerLaunched = false;
            _cameraService.SetActiveCamera(EGameCameraType.FlyEnd);
            OnPlayerFlyComplete?.Invoke();
        }

        public void Clear()
        {
            OnPlayerFlyComplete?.Invoke();

            _timeSinceLaunch = 0;
            _stopCheckTimer = 0;
            _isPlayerLaunched = false;
            _isPlayerStopped = false;
            _player = null;
        }

        public void Dispose()
        {
            _playerService.OnPlayerHumanoidCreated -= PlayerCreatedHandler;
            _playerFlightLaunchService.OnPlayerFlyStart -= PlayerFlyStartHandler;
            _updateService.RemoveFixedUpdateElement(this);
            OnPlayerFlyComplete = null;
            _disposable?.Dispose();
        }
    }
}