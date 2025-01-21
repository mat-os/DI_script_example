using AssetKits.ParticleImage;
using Configs;
using Game.Scripts.Infrastructure.Services;
using Game.Scripts.Utils.Debug;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.Widgets
{
    public class UpgradeProgressBarUI : MonoBehaviour
    {
        [SerializeField] private ProgressBar _carProgressBar;
        [SerializeField] private ProgressBar _stuntProgressBar;
        [SerializeField] private ProgressBar _earningsProgressBar;

        private UpgradeService _upgradeService;

        [Inject]
        public void Construct(UpgradeService upgradeService)
        {
            _upgradeService = upgradeService;
        }

        public void Initialize()
        {
            UpdateProgressBar(EUpgradeType.Car, _carProgressBar);
            UpdateProgressBar(EUpgradeType.Stunt, _stuntProgressBar);
            UpdateProgressBar(EUpgradeType.Income, _earningsProgressBar);
            
            SubscribeToUpgrades();
        }

        private void SubscribeToUpgrades()
        {
            _upgradeService.OnUpgradeCar += () => UpdateProgressBar(EUpgradeType.Car, _carProgressBar);
            _upgradeService.OnUpgradeStunt += () => UpdateProgressBar(EUpgradeType.Stunt, _stuntProgressBar);
            _upgradeService.OnUpgradeIncome += () => UpdateProgressBar(EUpgradeType.Income, _earningsProgressBar);
        }

        private void UpdateProgressBar(EUpgradeType upgradeType, IProgressBar progressBar)
        {
            int currentLevel = _upgradeService.GetUpgradeLevel(upgradeType);
            //TODO:
            int maxLevel = _upgradeService.GetTotalCountOfUpgrades(upgradeType) - 1;
            
            // Обновляем сам прогресс бар
            progressBar.SetProgress(currentLevel, maxLevel);
        }

        private void OnDestroy()
        {
            _upgradeService.OnUpgradeCar -= () => UpdateProgressBar(EUpgradeType.Car, _carProgressBar);
            _upgradeService.OnUpgradeStunt -= () => UpdateProgressBar(EUpgradeType.Stunt, _stuntProgressBar);
            _upgradeService.OnUpgradeIncome -= () => UpdateProgressBar(EUpgradeType.Income, _earningsProgressBar);
        }
    }
}
