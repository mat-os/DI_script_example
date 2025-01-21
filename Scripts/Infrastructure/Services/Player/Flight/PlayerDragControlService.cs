using System;
using Configs;
using Game.Scripts.Infrastructure.Services.Player;
using Game.Scripts.Infrastructure.States;
using Game.Scripts.LevelElements.Player;
using Game.Scripts.Utils.Debug;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Infrastructure.Services.Player
{
    public class PlayerDragControlService : IDisposable
    {
        private readonly PlayerConfig _playerConfig;
        private readonly EnergyService _energyService;
        private readonly PlayerService _playerService;

        private PlayerHumanoid _playerHumanoid;
        
        private float _currentDragForceFactor = 1;
        private float _dragTime;

        [Inject]
        public PlayerDragControlService(GameConfig gameConfig, EnergyService energyService, PlayerService playerService)
        {
            _playerService = playerService;
            _playerConfig = gameConfig.PlayerConfig;
            _energyService = energyService;
            
            _playerService.OnPlayerHumanoidCreated += PlayerCreatedHandler;
        }
        private void PlayerCreatedHandler(PlayerHumanoid playerHumanoid)
        {
            _playerHumanoid = playerHumanoid;
        }

        public void HandleDrag(Vector2 dragDelta)
        {
            if (!_energyService.HasEnergy()) 
                return;

            //CustomDebugLog.Log("[DRAG] HandleDrag");
            ApplyHorizontalControl(dragDelta.x);
            ApplyVerticalControl(dragDelta.y);
            //ApplyForwardControl(dragDelta.y);

            ChangeDragSlowDownFactor();
            
            ChangeDragForceFactor(dragDelta);
        }
        private void ApplyHorizontalControl(float horizontalDelta)
        {
            float horizontalInput = horizontalDelta * _playerConfig.DragConfig.HorizontalSensitivity * _currentDragForceFactor  * _playerConfig.DragConfig.TorqueToAddMultiplier;
            _playerHumanoid.AddTorqueForFlip(-Vector3.up, horizontalInput, ForceMode.Force);
        }
        private void ApplyVerticalControl(float verticalDelta)
        {
            float verticalInput = verticalDelta * _playerConfig.DragConfig.VerticalSensitivity * _currentDragForceFactor * _playerConfig.DragConfig.TorqueToAddMultiplier;
            _playerHumanoid.AddTorqueForFlip(-Vector3.right,  verticalInput, ForceMode.Force);
        }
        private void ApplyForwardControl(float verticalDelta)
        {
            float forwardForce = verticalDelta * _playerConfig.DragConfig.ForwardSensitivity * _currentDragForceFactor  * _playerConfig.DragConfig.TorqueToAddMultiplier;
            _playerHumanoid.AddTorqueForFlip(-Vector3.forward,  forwardForce, ForceMode.Force);
        }
        
        private void ChangeDragForceFactor(Vector2 dragDelta)
        {
            _currentDragForceFactor *= _playerConfig.DragConfig.DragForceDecayFactor;
            _currentDragForceFactor = Mathf.Clamp(_currentDragForceFactor, 0f, 1f);
        }
        private void ChangeDragSlowDownFactor()
        {
            _dragTime += Time.deltaTime; 
            float normalizedTime = Mathf.Clamp01(_dragTime / _playerConfig.DragConfig.DragDuration);
            _currentDragForceFactor = _playerConfig.DragConfig.DragForceCurve.Evaluate(normalizedTime);

        }
        public void StopDrag()
        {
            _currentDragForceFactor = 1;
        }

        public void Clear()
        {
            _playerHumanoid = null;
            _currentDragForceFactor = 1;
        }

        public void Dispose()
        {
            _playerService.OnPlayerHumanoidCreated -= PlayerCreatedHandler;
        }
    }
}
