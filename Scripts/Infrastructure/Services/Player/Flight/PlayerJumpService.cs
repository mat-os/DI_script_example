using System;
using Configs;
using Game.Scripts.Configs.Vfx;
using Game.Scripts.Infrastructure.Services;
using Game.Scripts.Infrastructure.Services.Level;
using Game.Scripts.Infrastructure.Services.Player;
using Game.Scripts.Infrastructure.States;
using Game.Scripts.LevelElements.Player;
using Game.Scripts.Utils.Debug;
using UnityEngine;
using Zenject;

public class PlayerJumpService : IDisposable
{
    private readonly PlayerConfig _playerConfig;
    private readonly EnergyService _energyService;
    private readonly VfxEffectsService _effectsService;
    private readonly VibrationService _vibrationService;
    private readonly SlowMotionService _slowMotionService;
    private readonly PlayerService _playerService;
    private readonly PlayerFlightLaunchService _playerFlightLaunchService;
    private readonly PlayerFlightLandingService _playerFlightLandingService;
    private readonly DifficultyService _difficultyService;

    private PlayerHumanoid _playerHumanoid;
    private float _currentJumpForceFactor = 1;
    private float _difficultyJumpForceMultiplier;

    [Inject]
    public PlayerJumpService(
        GameConfig gameConfig,
        EnergyService energyService,
        VfxEffectsService effectsService,
        VibrationService vibrationService,
        SlowMotionService slowMotionService,
        PlayerService playerService,
        PlayerFlightLaunchService playerFlightLaunchService,
        PlayerFlightLandingService playerFlightLandingService,
        DifficultyService difficultyService)
    {
        _difficultyService = difficultyService;
        _playerFlightLandingService = playerFlightLandingService;
        _playerFlightLaunchService = playerFlightLaunchService;
        _playerService = playerService;
        _playerConfig = gameConfig.PlayerConfig;
        _energyService = energyService;
        _effectsService = effectsService;
        _vibrationService = vibrationService;
        _slowMotionService = slowMotionService;

        _playerService.OnPlayerHumanoidCreated += PlayerCreatedHandler;
        _playerFlightLaunchService.OnPlayerFlyStart += PlayerFlyStartHandler;
        _playerFlightLandingService.OnPlayerFlyComplete += PlayerFlyCompleteHandler;
    }

    private void PlayerFlyCompleteHandler()
    {
    }

    private void PlayerCreatedHandler(PlayerHumanoid playerHumanoid)
    {
        _playerHumanoid = playerHumanoid;
        _difficultyJumpForceMultiplier = _difficultyService.GetPlayerJumpForceMultiplier();
    }

    private void PlayerFlyStartHandler()
    {
        // Placeholder for future logic
    }

    public void PerformJump(Vector2 dragDelta, float normalizedDrag)
    {
        if (dragDelta.magnitude <= 0) 
            return;
        
        // Расчет силы и стоимости энергии
        float resultantForce = CalculateSwipeForce(dragDelta);
        float energyCost = CalculateEnergyCost(ref resultantForce);

        if (!_energyService.HasEnergyFor(energyCost)) 
            return;

        // Применяем рассчитанную силу
        Vector2 clampedForce = CalculateForceComponents(dragDelta, resultantForce);
        ApplyForce(clampedForce, normalizedDrag);

        // Обновляем состояние игрока
        _energyService.ConsumeEnergy(energyCost);
        _energyService.ReduceEnergyRecoverFactor(energyCost);
        ChangeJumpForceFactor(normalizedDrag);
        SpawnJumpEffects();

        CustomDebugLog.Log("[JUMP] PerformJump");
    }

    public void UpdatePreviewEnergyCost(Vector2 dragDelta)
    {
        // Расчет силы и стоимости энергии для превью
        float resultantForce = CalculateSwipeForce(dragDelta);
        float previewEnergyCost = CalculateEnergyCost(ref resultantForce);

        // Обновляем предварительное значение энергии
        _energyService.ChangePreviewEnergy(previewEnergyCost);
    }

    private float CalculateSwipeForce(Vector2 dragDelta)
    {
        float horizontalInput = dragDelta.x * _playerConfig.JumpConfig.HorizontalSensitivity;
        float verticalInput = dragDelta.y * _playerConfig.JumpConfig.VerticalSensitivity;

        return Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput);
    }

    private float CalculateEnergyCost(ref float resultantForce)
    {
        float energyCost = Mathf.Lerp(
            _playerConfig.JumpConfig.MinJumpEnergyCost,
            _playerConfig.JumpConfig.MaxJumpEnergyCost,
            resultantForce / _playerConfig.MaxSwipeForce);

        float availableEnergy = _energyService.CurrentEnergy;
        if (energyCost > availableEnergy)
        {
            float energyFactor = availableEnergy / energyCost;
            resultantForce *= energyFactor; 
            energyCost = availableEnergy;  
        }

        return energyCost;
    }

    private Vector2 CalculateForceComponents(Vector2 dragDelta, float resultantForce)
    {
        float horizontalInput = dragDelta.x * _playerConfig.JumpConfig.HorizontalSensitivity;
        float verticalInput = dragDelta.y * _playerConfig.JumpConfig.VerticalSensitivity;

        horizontalInput = Mathf.Clamp(horizontalInput, -resultantForce, resultantForce);
        verticalInput = Mathf.Clamp(verticalInput, -resultantForce, resultantForce);

        return new Vector2(horizontalInput, verticalInput);
    }

    private void ApplyForce(Vector2 clampedForce, float normalizedDrag)
    {
        Vector3 forceToAdd = new Vector3(
            -clampedForce.x,
            clampedForce.y,
            -clampedForce.y * _playerConfig.JumpConfig.ForwardSensitivity);

        Rigidbody playerRigidbody = _playerHumanoid.PlayerView.RigidbodyRoot;
        CustomDebugLog.Log("[JUMP] playerRigidbody.velocity " + playerRigidbody.velocity, DebugColor.Magenta);

        if (Mathf.Abs(playerRigidbody.velocity.x) > _playerConfig.JumpConfig.MaxSpeedX)
        {
            forceToAdd.x = 0;
        }
        if (playerRigidbody.velocity.y > _playerConfig.JumpConfig.MaxSpeedY)
        {
            forceToAdd.y = 0;
        }
        else if (Mathf.Abs(playerRigidbody.velocity.y) < _playerConfig.JumpConfig.MinSpeedY || (playerRigidbody.velocity.y < 0 && forceToAdd.x > 0))
        {
            CustomDebugLog.Log("[JUMP] RigidbodyClearVelocity Y");
            _playerHumanoid.RigidbodyClearVelocityY(0);
        }

        var forwardSpeed = Mathf.Abs(playerRigidbody.velocity.z);
        float normalizedSpeed = Mathf.Clamp(forwardSpeed / _playerConfig.JumpConfig.MaxSpeedZ, 0, 1);
        var forwardForce = _playerConfig.JumpConfig.MaxForceZ * _playerConfig.JumpConfig.ForwardSpeedToForceCurve.Evaluate(normalizedSpeed);
        forceToAdd.z = -forwardForce;

        var finalForceToAdd = forceToAdd * (_difficultyJumpForceMultiplier * _currentJumpForceFactor);
        CustomDebugLog.Log("[JUMP] ForceToAdd = " + finalForceToAdd, DebugColor.Green);

        _playerHumanoid.AddForceToAllPlayerRigidbodies(finalForceToAdd, ForceMode.Impulse);
    }

    private void ChangeJumpForceFactor(float normalizedDrag)
    {
        // Рассчитываем уменьшение на основе нормализованной силы
        float decay = Mathf.Lerp(1, _playerConfig.JumpConfig.JumpForceDecayFactor, normalizedDrag);
        _currentJumpForceFactor *= decay;
        _currentJumpForceFactor = Mathf.Clamp(_currentJumpForceFactor, 0f, 1f);

        CustomDebugLog.Log($"[JUMP] _currentJumpForceFactor = {_currentJumpForceFactor} after normalized Drag = {normalizedDrag}", DebugColor.Blue);
    }

    private void SpawnJumpEffects()
    {
        var playerChest = _playerHumanoid.PlayerView.RigidbodyRoot.transform;
        _effectsService.SpawnEffect(VfxEffectType.PlayerJump, playerChest.position, playerChest.rotation);
        _vibrationService.Vibrate(VibrationPlaceType.PlayerJump);
    }

    public void Clear()
    {
        _playerHumanoid = null;
        _currentJumpForceFactor = 1;
    }

    public void Dispose()
    {
        _playerService.OnPlayerHumanoidCreated -= PlayerCreatedHandler;
        _playerFlightLaunchService.OnPlayerFlyStart -= PlayerFlyStartHandler;
        _playerFlightLandingService.OnPlayerFlyComplete -= PlayerFlyCompleteHandler;
    }
}
