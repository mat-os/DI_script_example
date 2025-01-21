using System;
using Configs;
using DG.Tweening;
using Game.Scripts.Configs.Vfx;
using Game.Scripts.Core.Update;
using Game.Scripts.Core.Update.Interfaces;
using Game.Scripts.Infrastructure.Services.Car;
using Game.Scripts.LevelElements.Car;
using Game.Scripts.LevelElements.Triggers;
using UnityEngine;

namespace Game.Scripts.Infrastructure.Services.Player
{
    public class PlayerLaunchTapGameService  : IDisposable, IUpdate, IFixedUpdate
    {
        public event Action OnTapGameStart;
        public event Action<float> OnTapGameEnd; // float - процент заполнения шкалы
        public event Action<float> OnProgressChanged; // float - прогресс шкалы (0.0 - 1.0)
        
        private readonly TapGameConfig _tapGameConfig;
        private readonly UpdateService _updateService;
        private readonly CarService _carService;

        private float _currentProgress = 0f;
        private float _remainingTime;
        private bool _isGameRunning = false;
        private CarView _carView;

        public PlayerLaunchTapGameService(GameConfig gameConfig,
            UpdateService updateService,
            CarService carService)
        {
            _carService = carService;
            _updateService = updateService;
            _tapGameConfig = gameConfig.PlayerConfig.TapGameConfig;
            
            _updateService.AddUpdateElement(this);
            _updateService.AddFixedUpdateElement(this);
            
           // _carService.OnCarHitWall += CarEnterWallTriggerHandle;
        }
        private void CarEnterWallTriggerHandle()
        {
            if(_isGameRunning)
                return;
            
            StartTapGame();
        }

        public void StartTapGame()
        {
            ResetGame();
            OnTapGameStart?.Invoke();
            GlobalEventSystem.Broker.Publish(new TapGameStartEvent());
            _isGameRunning = true;
        }

        private void ResetGame()
        {
            _currentProgress = 0f;
            _remainingTime = _tapGameConfig.TotalGameTime;
            NotifyProgressChanged();
        }
        public void ManualUpdate(float deltaTime)
        {
            if (!_isGameRunning) 
                return;

            HandleInput();
            NotifyProgressChanged();
        }

        public void ManualFixedUpdate(float fixedDeltaTime)
        {
            if (!_isGameRunning) 
                return;
            
            DecreaseProgress();
            CheckGameEnd();
        }
        private void HandleInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                AddProgress(_tapGameConfig.TapIncreaseAmount);
            }
        }

        private void DecreaseProgress()
        {
            _currentProgress -= _tapGameConfig.DecreaseRate * Time.deltaTime;
            _currentProgress = Mathf.Clamp01(_currentProgress);
        }

        private void AddProgress(float amount)
        {
            _currentProgress += amount;
            _currentProgress = Mathf.Clamp01(_currentProgress);
            NotifyProgressChanged();
        }

        private void NotifyProgressChanged()
        {
            OnProgressChanged?.Invoke(_currentProgress);
        }

        private void CheckGameEnd()
        {
            _remainingTime -= Time.deltaTime;

            if (_remainingTime <= 0 || _currentProgress >= _tapGameConfig.SuccessThreshold)
            {
                EndTapGame();
            }
        }

        private void EndTapGame()
        {
            _isGameRunning = false;
            GlobalEventSystem.Broker.Publish(new TapGameEndEvent(){  Result = _currentProgress});
            OnTapGameEnd?.Invoke(_currentProgress);
        }

        public void Dispose()
        {
            OnTapGameStart = null;
            OnTapGameEnd = null;
            OnProgressChanged = null;
            
            _updateService.RemoveUpdateElement(this);
            _updateService.RemoveFixedUpdateElement(this);
            
            _carService.OnCarHitWall -= CarEnterWallTriggerHandle;
        }
    }
}