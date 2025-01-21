using System;
using Configs;
using DG.Tweening;
using Game.Scripts.Configs.Vfx;
using Game.Scripts.Core.Update;
using Game.Scripts.Core.Update.Interfaces;
using Game.Scripts.Infrastructure.Services.Level;
using Game.Scripts.Infrastructure.States;
using Game.Scripts.LevelElements.Player;
using Game.Scripts.Utils.Debug;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Infrastructure.Services.Player
{
    public class PlayerFlightControlService : IDisposable, IFixedUpdate
    {        
        private readonly EnergyService _energyService;

        private readonly PlayerFlightLaunchService _playerFlightLaunchService;
        private readonly PlayerFlightLandingService _playerFlightLandingService;
        private readonly SlowMotionService _slowMotionService;
        private readonly ColorGradingService _colorGradingService;
        private readonly PlayerSwipeService _swipeService;
        private readonly PlayerJumpService _jumpService;
        private readonly PlayerDragControlService _dragService;
        private readonly UpdateService _updateService;
        private readonly JumpConfig _jumpConfig;
        private PlayerFlightLineDirectionService _flightLineDirectionService;

        private bool _isPlayerInFlightState;
        private bool _isCanJump = true;
        private Vector2 _currentDragDelta;
        private bool _isPlayerUnderControl;

        private bool _isJumpOnCooldown;
        private PostProcessingService _postProcessingService;

        [Inject]
        public PlayerFlightControlService(
            EnergyService energyService, 
            PlayerFlightLaunchService playerFlightLaunchService,
            PlayerFlightLandingService playerFlightLandingService,
            SlowMotionService slowMotionService,
            ColorGradingService colorGradingService,
            PlayerSwipeService swipeService,
            PlayerJumpService jumpService,
            PlayerDragControlService dragService,
            UpdateService updateService,
            GameConfig gameConfig,
            PlayerFlightLineDirectionService flightLineDirectionService,
            PostProcessingService postProcessingService)
        {
            _postProcessingService = postProcessingService;
            _flightLineDirectionService = flightLineDirectionService;
            _updateService = updateService;
            _dragService = dragService;
            _jumpService = jumpService;
            _swipeService = swipeService;
            _colorGradingService = colorGradingService;
            _slowMotionService = slowMotionService;
            _playerFlightLandingService = playerFlightLandingService;
            _playerFlightLaunchService = playerFlightLaunchService;
            _energyService = energyService;
            _jumpConfig = gameConfig.PlayerConfig.JumpConfig;
            
            _swipeService.OnSwipeStart += OnSwipeStart;
            _swipeService.OnSwipeUpdate += OnSwipeUpdate;
            _swipeService.OnSwipeEnd += OnSwipeEnd;
            
            _playerFlightLaunchService.OnPlayerFlyStart += PlayerFlyStartHandler;
            _playerFlightLandingService.OnPlayerFlyComplete += PlayerFlyCompleteHandler;
            
            _updateService.AddFixedUpdateElement(this);
        }

        private void PlayerFlyCompleteHandler()
        {
            _isPlayerInFlightState = false;
            _slowMotionService.StopSlowMo(ESlowMotionType.FlightChangeDirection);
        }
        private void PlayerFlyStartHandler()
        {
            _isPlayerInFlightState = true;
        }
        public void ManualFixedUpdate(float fixedDeltaTime)
        {
            if (!_isCanJump || !_isPlayerInFlightState || !_isPlayerUnderControl) 
                return;
            if(_isJumpOnCooldown)
                return;
            
            _jumpService.UpdatePreviewEnergyCost(_currentDragDelta);
            _dragService.HandleDrag(_currentDragDelta);
        }
        
        private void OnSwipeStart()
        {
            if(_isCanJump == false || _isPlayerInFlightState == false)
                return;
            if(_isJumpOnCooldown)
                return;
            
            _energyService.PauseEnergyRecovery();
            _slowMotionService.StartSlowMo(ESlowMotionType.FlightChangeDirection);
            _colorGradingService.StartColorGrading();
            _postProcessingService.StartJumpPostProcessing();
            _flightLineDirectionService.OnSwipeStart();
            
            _isPlayerUnderControl = true;
        }

        private void OnSwipeUpdate(Vector3 dragDelta)
        {
            if(_isCanJump == false || _isPlayerInFlightState == false || _isPlayerUnderControl == false)
                return;
            if(_isJumpOnCooldown)
                return;
            
            _flightLineDirectionService.OnSwipeUpdate(dragDelta);
            _currentDragDelta = dragDelta;
        }

        private void OnSwipeEnd()
        {
            if(_isCanJump == false || _isPlayerInFlightState == false)
                return;

            if (_isPlayerUnderControl)
            {
                _dragService.StopDrag();
                _colorGradingService.EndColorGrading();
                _postProcessingService.EndJumpPostProcessing();
                _energyService.ResumeEnergyRecovery();
                _energyService.ChangePreviewEnergy(0);
                _flightLineDirectionService.OnSwipeEnd();
                _slowMotionService.StopSlowMo(ESlowMotionType.FlightChangeDirection);
            }
            if (_swipeService.IsSwipeValid() && _isJumpOnCooldown == false)
            {
                _jumpService.PerformJump(_swipeService.DragDelta, _swipeService.NormalizedDrag);
                StartJumpCooldown();
            }
            
            _isPlayerUnderControl = false;
        }

        private void StartJumpCooldown()
        {
            _isJumpOnCooldown = true;
            DOVirtual.DelayedCall(_jumpConfig.JumpCooldown, () => _isJumpOnCooldown = false);
        }

        public void SetIsPlayerCanJump(bool isCanJump)
        {
            CustomDebugLog.Log("[JUMP] SetIsPlayerCanJump " + isCanJump);
            _isCanJump = isCanJump;
        }
        
        public void Clear()
        {
            _isPlayerInFlightState = false;
            _isCanJump = true;
        }

        public void Dispose()
        {
            Clear();
            
            _playerFlightLaunchService.OnPlayerFlyStart -= PlayerFlyStartHandler;
            _playerFlightLandingService.OnPlayerFlyComplete -= PlayerFlyCompleteHandler;
            
            _playerFlightLaunchService.OnPlayerFlyStart -= PlayerFlyStartHandler;
            _playerFlightLandingService.OnPlayerFlyComplete -= PlayerFlyCompleteHandler;
            
            _swipeService.OnSwipeStart -= OnSwipeStart;
            _swipeService.OnSwipeUpdate -= OnSwipeUpdate;
            _swipeService.OnSwipeEnd -= OnSwipeEnd;
            
            _updateService.RemoveFixedUpdateElement(this);
        }
    }
}