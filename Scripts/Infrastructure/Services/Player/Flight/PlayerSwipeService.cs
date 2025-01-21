using System;
using Configs;
using Game.Scripts.Core.Update;
using Game.Scripts.Core.Update.Interfaces;
using Game.Scripts.Infrastructure.LevelStateMachin;
using Game.Scripts.Infrastructure.LevelStateMachin.States;
using Game.Scripts.Infrastructure.Services.Player;
using UnityEngine;

public class PlayerSwipeService : IDisposable, IUpdate
{
    public event Action OnSwipeStart;
    public event Action<Vector3> OnSwipeUpdate;
    public event Action OnSwipeEnd;

    private Vector3 _dragStart;
    private Vector3 _dragCurrent;
    
    private readonly float _minSwipeDistance;
    private readonly float _maxSwipeDistance;
    
    private readonly UpdateService _updateService;
    private readonly LevelStateMachine _levelStateMachine;
    private PlayerFlightLaunchService _playerFlightLaunchService;
    private PlayerFlightLandingService _playerFlightLandingService;
    
    private bool _isPlayerInFlightState;
    private bool _isDetectFirstTap;

    public bool IsSwipeValid() => 
        DragMagnitude >= _minSwipeDistance;
    public Vector3 DragDelta => 
        GetClampedDragDelta();
    public float DragMagnitude => 
        GetClampedDragMagnitude();
    public float NormalizedDrag => 
        Mathf.Clamp01(DragMagnitude / _maxSwipeDistance);

    public PlayerSwipeService(GameConfig gameConfig, UpdateService updateService, LevelStateMachine levelStateMachine,
        PlayerFlightLaunchService playerFlightLaunchService,
        PlayerFlightLandingService playerFlightLandingService)
    {
        _playerFlightLandingService = playerFlightLandingService;
        _playerFlightLaunchService = playerFlightLaunchService;
        _levelStateMachine = levelStateMachine;
        _updateService = updateService;

        _minSwipeDistance = gameConfig.PlayerConfig.SwipeConfig.MinSwipeDistance;
        _maxSwipeDistance = gameConfig.PlayerConfig.SwipeConfig.MaxSwipeDistance;

        _playerFlightLaunchService.OnPlayerFlyStart += PlayerFlyStartHandler;
        _playerFlightLandingService.OnPlayerFlyComplete += PlayerFlyCompleteHandler;
        
        _updateService.AddUpdateElement(this);
    }
    private void PlayerFlyCompleteHandler()
    {
        _isPlayerInFlightState = false;
    }
    private void PlayerFlyStartHandler()
    {
        _isPlayerInFlightState = true;
    }
    public void ManualUpdate(float deltaTime)
    {
        if (_levelStateMachine.CurrentState.GetType() != typeof(PlayLevelState)) 
            return;
        
        if (_isPlayerInFlightState)
        {
            Update();
        }
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _dragStart = Input.mousePosition;
            OnSwipeStart?.Invoke();
            _isDetectFirstTap = true;
        }

        if (Input.GetMouseButton(0) && _isDetectFirstTap)
        {
            _dragCurrent = Input.mousePosition;
            OnSwipeUpdate?.Invoke(DragDelta);
        }

        if (Input.GetMouseButtonUp(0) && _isDetectFirstTap)
        {
            OnSwipeEnd?.Invoke();
            ResetSwipe();
            _isDetectFirstTap = false;
        }
    }

    private Vector3 GetClampedDragDelta()
    {
        Vector3 delta = _dragCurrent - _dragStart;

        if (delta.magnitude > _maxSwipeDistance)
        {
            delta = delta.normalized * _maxSwipeDistance;
        }

        return delta;
    }

    private float GetClampedDragMagnitude()
    {
        float magnitude = (_dragCurrent - _dragStart).magnitude;

        return Mathf.Min(magnitude, _maxSwipeDistance);
    }

    private void ResetSwipe()
    {
        _dragStart = Vector3.zero;
        _dragCurrent = Vector3.zero;
    }

    public void Dispose()
    {
        _updateService.RemoveUpdateElement(this);

        OnSwipeStart = null;
        OnSwipeUpdate = null;
        OnSwipeEnd = null;
    }

    public void Clear()
    {
        _dragStart = Vector3.zero;
        _dragCurrent = Vector3.zero;
        _isPlayerInFlightState = false;
        _isDetectFirstTap = false;
    }
}
