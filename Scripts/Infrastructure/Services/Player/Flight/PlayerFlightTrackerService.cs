using System;
using Game.Scripts.Core.Update;
using Game.Scripts.Core.Update.Interfaces;
using Game.Scripts.Infrastructure.States;
using Game.Scripts.LevelElements.Player;
using UnityEngine;

namespace Game.Scripts.Infrastructure.Services.Player
{
    public class PlayerFlightTrackerService : IDisposable, IFixedUpdate
    {
        public Action<float> OnFlyDistanceChange;

        private readonly PlayerService _playerService;
        private readonly UpdateService _updateService;
        private readonly PlayerFlightLaunchService _playerFlightLaunchService;
        
        private PlayerHumanoid _player;
        private Vector3 _startFlyPosition;
        private float _flyDistance;
        private bool _isPlayerFlyStart;

        public PlayerFlightTrackerService(UpdateService updateService, 
            PlayerService playerService, 
            PlayerFlightLaunchService playerFlightLaunchService)
        {
            _playerFlightLaunchService = playerFlightLaunchService;
            _playerService = playerService;
            _updateService = updateService;

            _playerService.OnPlayerHumanoidCreated += PlayerHumanoidCreatedHandler;
            _updateService.AddFixedUpdateElement(this);
            _playerFlightLaunchService.OnPlayerFlyStart += PlayerFlyStartHandler;
        }

        private void PlayerFlyStartHandler()
        {
            _isPlayerFlyStart = true;
            _startFlyPosition = _player.PlayerView.transform.position;
        }

        private void PlayerHumanoidCreatedHandler(PlayerHumanoid playerHumanoid)
        {
            _player = playerHumanoid;
        }

        public void ManualFixedUpdate(float fixedDeltaTime)
        {
            if(_isPlayerFlyStart)
                CountFlyDistance();
        }

        private void CountFlyDistance()
        {
            var flyDistance = Mathf.Abs(Vector3.Distance(_startFlyPosition, _player.PlayerView.RigidbodyRoot.transform.position));
            if (_flyDistance < flyDistance)
            {
                _flyDistance = flyDistance;
                OnFlyDistanceChange?.Invoke(_flyDistance);
            }
        }

        public void Clear()
        {
            _startFlyPosition = Vector3.zero;
            _flyDistance = 0;
            _isPlayerFlyStart = false;
        }

        public void Dispose()
        {
            _playerService.OnPlayerHumanoidCreated -= PlayerHumanoidCreatedHandler;
            _updateService.RemoveFixedUpdateElement(this);
            _playerFlightLaunchService.OnPlayerFlyStart -= PlayerFlyStartHandler;
        }
    }
}