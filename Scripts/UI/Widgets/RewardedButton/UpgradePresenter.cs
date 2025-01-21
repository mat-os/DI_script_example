using System;
using Configs;
using Game.Scripts.Utils.Debug;
using UnityEngine;
using Zenject;

public class UpgradePresenter
{
    private readonly IUpgradeView _view;
    private readonly UpgradeModel _model;
    private readonly EUpgradeType _upgradeType;

    [Inject]
    public UpgradePresenter(IUpgradeView view, UpgradeModel model, EUpgradeType upgradeType)
    {
        _view = view;
        _model = model;
        _upgradeType = upgradeType;

        _view.OnCoinUpgradeClick += HandleCoinUpgrade;
        _view.OnAdUpgradeClick += HandleAdUpgrade;
        _model.OnPlayerBuyUpgrade += HandlePlayerBuyUpgrade;
        
        RefreshUI();
    }

    public void OnOpenStart()
    {
        RefreshUI();
    }
    private void HandlePlayerBuyUpgrade()
    {
        RefreshUI();
    }

    private void HandleCoinUpgrade()
    {
        CustomDebugLog.Log("[Upgrade Button] Handle Coin Upgrade");
        HandleUpgrade(_model.UpgradeForCoins, () => _model.IsEnoughCurrency(_upgradeType));
        
        if (_model.IsFullyUpgraded(_upgradeType)) 
            _view.DisableButton();
    }

    private void HandleAdUpgrade()
    {
        HandleUpgrade(_model.Upgrade, _model.CanUpgradeWithAds);
        
        if (_model.IsFullyUpgraded(_upgradeType)) 
            _view.DisableButton();
    }

    private void HandleUpgrade(Action<EUpgradeType> upgradeAction, Func<bool> canUpgrade)
    {
        if (canUpgrade())
        {
            upgradeAction(_upgradeType);
            _view.PlayEffectOnBuy();
        }
    }

    private void RefreshUI()
    {
        if (_model.IsFullyUpgraded(_upgradeType))
        {
            _view.DisableButton();
            return;
        }

        var isEnoughCurrency = _model.IsEnoughCurrency(_upgradeType);
        int level = _model.GetUpgradeLevel(_upgradeType);
        int cost = _model.GetUpgradeCost(_upgradeType);

        _view.SetupProgressBar(_model.GetCurrentUpgradeStepIndex(_upgradeType) + 1 ,_model.GetCountOfStepsOnThisUpgrade(_upgradeType));
        
        _view.UpdateLevelText(level);
        _view.UpdateCostText(cost);

        _view.ActivateLockedView(isEnoughCurrency == false);
        _view.SetNotificationActive(isEnoughCurrency);
        _view.SetCoinButtonActive(isEnoughCurrency);
        _view.SetAdButtonActive(_model.CanUpgradeWithAds());
    }
}