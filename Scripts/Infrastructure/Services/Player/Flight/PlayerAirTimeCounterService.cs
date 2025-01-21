using System;
using Configs;
using Game.Scripts.Core.Update;
using Game.Scripts.Core.Update.Interfaces;
using Game.Scripts.Infrastructure.States;
using Game.Scripts.LevelElements.Player;
using UnityEngine;

namespace Game.Scripts.Infrastructure.Services.Player
{
    public class PlayerAirTimeCounterService: IDisposable, IFixedUpdate
    {
        public Action<float> OnPlayerTouchGround;
        public Action<float> OnPlayerAirTimeChange;
        public Action OnPlayerScoredDuringAirTime;

        private readonly PlayerService _playerService;
        private readonly UpdateService _updateService;
        
        private PlayerHumanoid _player;
        private bool _isInAir;
        private float _airTime;
        
        private readonly float _scoreInterval;
        private float _timeSinceLastScore;

        private readonly PlayerFlightLaunchService _playerFlightLaunchService;

        public PlayerAirTimeCounterService(UpdateService updateService, PlayerService playerService, PlayerFlightLaunchService playerFlightLaunchService, GameConfig gameConfig)
        {
            _playerFlightLaunchService = playerFlightLaunchService;
            _playerService = playerService;
            _updateService = updateService;

            _updateService.AddFixedUpdateElement(this);
            _playerService.OnPlayerHumanoidCreated += PlayerHumanoidCreatedHandler;
            _playerFlightLaunchService.OnPlayerFlyStart += PlayerFlyStartHandler;

            _scoreInterval = gameConfig.ScoreConfig.AirTimeScoreInterval;
        }

        private void PlayerFlyStartHandler()
        {
            _airTime = 0;
            _timeSinceLastScore = 0;
            _isInAir = true;
        }
        private void PlayerHumanoidCreatedHandler(PlayerHumanoid playerHumanoid)
        {
            _player = playerHumanoid;
        }
        public void PlayerTouchGround()
        {
            if (_isInAir)
            {
                OnPlayerTouchGround?.Invoke(_airTime);
                ResetAirTime();
            }
        }
        
        public void ManualFixedUpdate(float fixedDeltaTime)
        {
            if (_player == null) 
                return;
            
            if (_isInAir)
            {
                _airTime += fixedDeltaTime;
                _timeSinceLastScore += fixedDeltaTime; 
                
                OnPlayerAirTimeChange?.Invoke(_airTime);
                
                // 💡 Начисление очков, если интервал превышен
                if (_timeSinceLastScore >= _scoreInterval)
                {
                    OnPlayerScoredDuringAirTime?.Invoke();
                    _timeSinceLastScore -= _scoreInterval; 
                }
            }
        }
        
        private void ResetAirTime()
        {
            _airTime = 0;
            _timeSinceLastScore = 0; 
            _isInAir = false;
        }
        
        public void Clear()
        {
            ResetAirTime();
        }
        
        public void Dispose()
        {
            _playerService.OnPlayerHumanoidCreated -= PlayerHumanoidCreatedHandler;
            _playerFlightLaunchService.OnPlayerFlyStart -= PlayerFlyStartHandler;
            _updateService.RemoveFixedUpdateElement(this);
        }


    }
}