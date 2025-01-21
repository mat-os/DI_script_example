using System;
using Configs;
using Game.Configs;
using Game.Scripts.Constants;
using Game.Scripts.Core.CurrencyService;
using Game.Scripts.Infrastructure.Services;
using Game.Scripts.Infrastructure.Services.Advertisement;
using Zenject;

public class UpgradeModel
{
    public Action OnPlayerBuyUpgrade;
    
    private UpgradeService _upgradeService;
    private CurrencyService _currencyService;
    private IAdvertisementService _advertisementService;

    [Inject]
    public void Construct(UpgradeService upgradeService, CurrencyService currencyService, IAdvertisementService advertisementService)
    {
        _upgradeService = upgradeService;
        _currencyService = currencyService;
        _advertisementService = advertisementService;

        _upgradeService.OnBuyUpgrade += PlayerBuyUpgradeHandler;
    }

    public int GetCountOfStepsOnThisUpgrade(EUpgradeType upgradeType)
    {
        return _upgradeService.GetStepsCountOnThisUpgrade(upgradeType);
    }
    public int GetCurrentUpgradeStepIndex(EUpgradeType upgradeType)
    {
        return _upgradeService.GetCurrentStepIndex(upgradeType);
    }

    private void PlayerBuyUpgradeHandler()
    {
        OnPlayerBuyUpgrade?.Invoke();
    }
    public bool CanUpgradeWithAds()
    {
        return _advertisementService.CanShowRewarded(AdsPlacementsConstants.Rewarded.LOBBY_SCREEN);
    }

    public int GetUpgradeCost(EUpgradeType upgradeType)
    {
        return _upgradeService.GetUpgradeCost(upgradeType);
    }

    public int GetUpgradeLevel(EUpgradeType upgradeType)
    {
        return _upgradeService.GetUpgradeLevel(upgradeType);
    }

    public void Upgrade(EUpgradeType upgradeType)
    {
        _upgradeService.Upgrade(upgradeType);
    }

    public bool IsEnoughCurrency(EUpgradeType upgradeType)
    {
        int cost = _upgradeService.GetUpgradeCost(upgradeType);
        return _currencyService.IsEnoughCurrency(ECurrencyType.Coins, cost);
    }
    public void UpgradeForCoins(EUpgradeType upgradeType)
    {
        int cost = _upgradeService.GetUpgradeCost(upgradeType);
        if(_currencyService.TrySpendCurrency(ECurrencyType.Coins, cost))
            _upgradeService.Upgrade(upgradeType);
    }

    public bool IsFullyUpgraded(EUpgradeType upgradeType)
    {
        return _upgradeService.IsFullyUpgraded(upgradeType);
    }
}