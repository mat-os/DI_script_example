using System;
using Configs;
using Game.Scripts.Core.Update;
using Game.Scripts.Core.Update.Interfaces;
using Game.Scripts.Infrastructure.Services;
using UnityEngine;

public class EnergyService : IDisposable, IFixedUpdate
{
    public Action<float> OnEnergyChange;
    public Action<float> OnPreviewEnergyChange;

    private readonly PlayerConfig _playerConfig;
    private readonly UpdateService _updateService;
    private readonly UpgradeService _upgradeService;

    private bool _isEnergyRecoveryPaused;
    private float _currentEnergyValue;

    private float _currentEnergyRecoverFactor = 1;
    private float _currentEnergyRecoverRate;

    private float _currentMaxEnergy;

    public float CurrentEnergy
    {
        get => _currentEnergyValue;
        set
        {
            _currentEnergyValue = value;
            OnEnergyChange?.Invoke(_currentEnergyValue);
        }
    }

    public EnergyService(GameConfig gameConfig, UpdateService updateService, UpgradeService upgradeService)
    {
        _upgradeService = upgradeService;
        _updateService = updateService;
        _playerConfig = gameConfig.PlayerConfig;

        _updateService.AddFixedUpdateElement(this);
    }

    public void Initialize()
    {
        StuntUpgradeStep currentUpgradeStep = _upgradeService.GetCurrentUpgradeStep<StuntUpgradeStep>(EUpgradeType.Stunt);

        _currentMaxEnergy = _playerConfig.EnergyConfig.MaxEnergy * currentUpgradeStep.MaxEnergyAmountMultiplier;
        CurrentEnergy = _currentMaxEnergy;
        
        _currentEnergyRecoverRate = _playerConfig.EnergyConfig.EnergyRecoveryRate * currentUpgradeStep.EnergyRecoveryRateMultiplier;
        _currentEnergyRecoverFactor = 1;
        
        _isEnergyRecoveryPaused = true;
    }

    public void ManualFixedUpdate(float fixedDeltaTime)
    {
        if (!_isEnergyRecoveryPaused)
        {
            RecoverEnergy(fixedDeltaTime);
        }
    }

    public void ChangePreviewEnergy(float value)
    {
        OnPreviewEnergyChange?.Invoke(value);
    }
    public bool HasEnergy()
    {
        return CurrentEnergy > 0f;
    }

    public void ConsumeEnergy(float amount)
    {
        CurrentEnergy = Mathf.Max(0f, CurrentEnergy - amount);
    }

    private void RecoverEnergy(float deltaTime)
    {
        var energyToRecover = _currentEnergyRecoverRate * deltaTime * _currentEnergyRecoverFactor;
        var energyRecoveryRate = CurrentEnergy + energyToRecover;
        CurrentEnergy = Mathf.Clamp(energyRecoveryRate, 0f, _playerConfig.EnergyConfig.MaxEnergy);
    }

    public void PauseEnergyRecovery()
    {
        _isEnergyRecoveryPaused = true;
    }

    public void ResumeEnergyRecovery()
    {
        _isEnergyRecoveryPaused = false;
    }
    public void ReduceEnergyRecoverFactor(float energyCost)
    {
        //var percentOfEnergy = energyCost / _playerConfig.MaxEnergy;
        _currentEnergyRecoverFactor *= _playerConfig.EnergyConfig.EnergyDecayFactor;
        _currentEnergyRecoverFactor = Mathf.Clamp(_currentEnergyRecoverFactor, 0f, 1f);
    }
    public void Dispose()
    {
        _updateService.RemoveFixedUpdateElement(this);
    }
    public bool HasEnergyFor(float energyCost)
    {
        return CurrentEnergy >= energyCost;
    }
}