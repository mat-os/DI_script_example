using System;

public interface IUpgradeView
{
    void SetCoinButtonActive(bool isActive);
    void SetAdButtonActive(bool isActive);
    void UpdateLevelText(int level);
    void UpdateCostText(int cost);
    void DisableButton();
    void PlayEffectOnBuy();
    void ActivateLockedView(bool isLockViewActive);
    void SetNotificationActive(bool isActive);
    void SetupProgressBar(int currentLevel, int maxLevel);


    event Action OnCoinUpgradeClick;
    event Action OnAdUpgradeClick;
}