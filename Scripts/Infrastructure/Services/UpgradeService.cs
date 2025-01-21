using System;
using System.Collections.Generic;
using System.Linq;
using Configs;
using Game.Scripts.Constants;
using Game.Scripts.Core.CurrencyService;
using Game.Scripts.Infrastructure.Services.Analytics;
using Game.Scripts.UI.Screens.Messages;
using Game.Scripts.UI.Screens.Serviсes;
using Game.Scripts.Utils.Debug;
using Game.Scripts.Utils.Prefs;
using UnityEngine;

namespace Game.Scripts.Infrastructure.Services
{
    public class UpgradeService : IDisposable
    {
        public Action OnBuyUpgrade;

        public Action OnUpgradeCar;
        public Action OnUpgradeStunt;
        public Action OnUpgradeIncome;

        private readonly UpgradesConfig _upgradesConfig;
        private readonly DataService _dataService;
        private readonly VibrationService _vibrationService;
        private readonly CurrencyService _currencyService;
        private readonly LevelDataService _levelDataService;
        private readonly UIGarageService _uiGarageService;
        private readonly MessageBoxService _messageBoxService;

        private readonly Dictionary<EUpgradeType, UpgradeData> _upgrades;
        private readonly Dictionary<EUpgradeType, List<UpgradeStep>> _upgradeSteps;
        private AnalyticService _analyticService;

        public UpgradeService(GameConfig gameConfig,
            DataService dataService,
            VibrationService vibrationService,
            CurrencyService currencyService,
            UIGarageService uiGarageService,
            LevelDataService levelDataService,
            MessageBoxService messageBoxService,
            AnalyticService analyticService)
        {
            _analyticService = analyticService;
            _messageBoxService = messageBoxService;
            _levelDataService = levelDataService;
            _uiGarageService = uiGarageService;
            _currencyService = currencyService;
            _vibrationService = vibrationService;
            _dataService = dataService;
            _upgradesConfig = gameConfig.UpgradesConfig;

            // Initialize the upgrades in a dictionary for easier access
            _upgrades = new Dictionary<EUpgradeType, UpgradeData>
            {
                {
                    EUpgradeType.Car,
                    new UpgradeData(_dataService.Upgrades.CarLevel, GetUpgradePrices(_upgradesConfig.CarUpgrades))
                },
                {
                    EUpgradeType.Income,
                    new UpgradeData(_dataService.Upgrades.IncomeLevel, GetUpgradePrices(_upgradesConfig.IncomeUpgrades))
                },
                {
                    EUpgradeType.Stunt,
                    new UpgradeData(_dataService.Upgrades.StuntLevel, GetUpgradePrices(_upgradesConfig.StuntUpgrades))
                }
            };

            _upgradeSteps = new Dictionary<EUpgradeType, List<UpgradeStep>>();
            _upgradeSteps[EUpgradeType.Car] = GetUpgradeSteps(_upgradesConfig.CarUpgrades);
            _upgradeSteps[EUpgradeType.Stunt] = GetUpgradeSteps(_upgradesConfig.StuntUpgrades);
            _upgradeSteps[EUpgradeType.Income] = GetUpgradeSteps(_upgradesConfig.IncomeUpgrades);
            
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            _upgrades[EUpgradeType.Car].OnUpgrade += () => OnUpgradeCar?.Invoke();
            _upgrades[EUpgradeType.Income].OnUpgrade += () => OnUpgradeIncome?.Invoke();
            _upgrades[EUpgradeType.Stunt].OnUpgrade += () => OnUpgradeStunt?.Invoke();
        }


        public T GetCurrentUpgradeStep<T>(EUpgradeType upgradeType) where T : UpgradeStep
        {
            if (upgradeType == EUpgradeType.Car || upgradeType == EUpgradeType.Stunt)
            {
                if (_levelDataService.GetCurrentLevelData().IsUseSpecialLevelData && _levelDataService.GetCurrentLevelData().SpecialLevelDataSettings.IsForceSetUpgrades)
                {
                    return upgradeType switch
                    {
                        EUpgradeType.Car => (T)(object)_levelDataService.GetCurrentLevelData().SpecialLevelDataSettings.CarUpgrade,
                        //EUpgradeType.Income => (T)(object)_levelDataService.GetCurrentLevelData().SpecialLevelDataSettings.IncomeUpgrade,
                        EUpgradeType.Stunt => (T)(object)_levelDataService.GetCurrentLevelData().SpecialLevelDataSettings.StuntUpgrade,
                        _ => throw new ArgumentOutOfRangeException(nameof(upgradeType), upgradeType, null)
                    };
                }
            }

            int currentLevel = _upgrades[upgradeType].Level.Value;
            switch (upgradeType)
            {
                case EUpgradeType.Car:
                    return GetCurrentUpgradeStep(_upgradesConfig.CarUpgrades, currentLevel) as T;
                case EUpgradeType.Stunt:
                    return GetCurrentUpgradeStep(_upgradesConfig.StuntUpgrades, currentLevel) as T;
                case EUpgradeType.Income:
                    return GetCurrentUpgradeStep(_upgradesConfig.IncomeUpgrades, currentLevel) as T;
                default:
                    throw new ArgumentOutOfRangeException(nameof(upgradeType), upgradeType, null);
            }
        }

        private UpgradeStep GetCurrentUpgradeStep<T>(List<UpgradePack<T>> upgradePacks, int currentLevel) where T : UpgradeStep
        {
            int currentIndex = 0;
            foreach (var upgradePack in upgradePacks)
            {
                foreach (var step in upgradePack.Steps)
                {
                    if (currentIndex == currentLevel)
                    {
                        return step;
                    }
                    currentIndex++;
                }
            }
            throw new ArgumentOutOfRangeException(nameof(currentLevel), "Current level is out of range.");
        }
    

        public UpgradePack<T> GetUpgradeInfoByIndex<T>(EUpgradeType upgradeType) where T : UpgradeStep
        {
            var level = _upgrades[upgradeType].Level.Value;
            switch (upgradeType)
            {
                case EUpgradeType.Car:
                    return GetUpgradeInfoByIndex(_upgradesConfig.CarUpgrades, level) as UpgradePack<T>;
                case EUpgradeType.Stunt:
                    return GetUpgradeInfoByIndex(_upgradesConfig.StuntUpgrades, level) as UpgradePack<T>;
                case EUpgradeType.Income:
                    return GetUpgradeInfoByIndex(_upgradesConfig.IncomeUpgrades, level) as UpgradePack<T>;
                default:
                    throw new ArgumentOutOfRangeException(nameof(upgradeType), upgradeType, null);
            }
        }

        public int GetStepsCountOnThisUpgrade(EUpgradeType upgradeType)
        {
            switch(upgradeType)
            {
                case EUpgradeType.Car:
                    return GetUpgradeInfoByIndex<CarUpgradeStep>(upgradeType).Steps.Count;
                    break;
                case EUpgradeType.Stunt:
                    return GetUpgradeInfoByIndex<StuntUpgradeStep>(upgradeType).Steps.Count;
                    break;
                case EUpgradeType.Income:
                    return GetUpgradeInfoByIndex<IncomeUpgradeStep>(upgradeType).Steps.Count;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(upgradeType), upgradeType, null);
            }
        }

        private UpgradePack<T> GetUpgradeInfoByIndex<T>(List<UpgradePack<T>> upgradeInfos, int upgradeIndexLevel) where T : UpgradeStep
        {
            int currentIndex = 0;
            foreach (var upgradeInfo in upgradeInfos)
            {
                foreach (var step in upgradeInfo.Steps)
                {
                    if (currentIndex == upgradeIndexLevel)
                    {
                        return upgradeInfo;
                    }
                    currentIndex++;
                }
            }
            throw new ArgumentOutOfRangeException(nameof(upgradeIndexLevel), "Upgrade index level is out of range.");
        }
        public int GetUpgradeLevel(EUpgradeType upgradeType)
        {
            return _upgrades[upgradeType].Level.Value;
        }

        public int GetUpgradeCost(EUpgradeType upgradeType)
        {
            return _upgrades[upgradeType].GetNextUpgradeCost();
        }

        public void Upgrade(EUpgradeType upgradeType)
        {
            var upgradeData = _upgrades[upgradeType];

            var oldValue = GetCurrentUpgradeValueForMessage(upgradeType);
                
            upgradeData.Level.Value++;
            upgradeData.InvokeUpgrade();
            
            var newValue = GetCurrentUpgradeValueForMessage(upgradeType);
            
            _vibrationService.Vibrate(VibrationPlaceType.BuyUpgrade);
            _uiGarageService.PlayUpgradeFx(upgradeType);
            
            CustomDebugLog.Log($"[UPGRADE] Upgrade {upgradeType} to {upgradeData.Level.Value}");
            
            ShowUpgradeMessage(upgradeType, oldValue, newValue);
            _analyticService.LogBuyUpgrade(upgradeType.ToString(), upgradeData.Level.Value);

            OnBuyUpgrade?.Invoke();
        }

        private float GetCurrentUpgradeValueForMessage(EUpgradeType upgradeType)
        {
            float oldValue = 0f;
            switch (upgradeType)
            {
                case EUpgradeType.Car:
                    oldValue = GetCurrentUpgradeStep<CarUpgradeStep>(upgradeType).MaxCarSpeedMultiplier;
                    break;
                case EUpgradeType.Stunt:
                    oldValue = GetCurrentUpgradeStep<StuntUpgradeStep>(upgradeType).FlyForceMultiplier;
                    break;
                case EUpgradeType.Income:
                    oldValue = GetCurrentUpgradeStep<IncomeUpgradeStep>(upgradeType).EarningMultiplier;
                    break;
            }

            return oldValue;
        }

        private void ShowUpgradeMessage(EUpgradeType upgradeType, float oldValue, float newValue)
        {
            var upgradeMessage = _messageBoxService.ShowScreen<UpgradeMessage>();
            var description = GetDescriptionTextForUpgradeMessage(upgradeType);
            upgradeMessage.SetUpgradeText(description, oldValue, newValue);
        }

        private string GetDescriptionTextForUpgradeMessage(EUpgradeType upgradeType)
        {
            switch (upgradeType)
            {
                case EUpgradeType.Car:
                    return _upgradesConfig.CarUpgradeDescriptionMessageText;
                    break;
                case EUpgradeType.Stunt:
                    return _upgradesConfig.StuntUpgradeDescriptionMessageText;
                    break;
                case EUpgradeType.Income:
                    return _upgradesConfig.IncomeUpgradeDescriptionMessageText;
                    break;
            }

            return null;
        }

        public bool IsFullyUpgraded(EUpgradeType type)
        {
            return _upgrades[type].IsFullyUpgraded();
        }
        
        // Метод для получения списка цен для любого типа улучшений
        private int[] GetUpgradePrices<T>(List<UpgradePack<T>> upgradeInfos) where T : UpgradeStep
        {
            return upgradeInfos
                .SelectMany(upgradeInfo => upgradeInfo.Steps)
                .Select(step => step.Price)
                .ToArray();
        }
        private List<UpgradeStep> GetUpgradeSteps<T>(List<UpgradePack<T>> upgradeInfos) where T : UpgradeStep
        {
            return upgradeInfos
                .SelectMany(upgradeInfo => upgradeInfo.Steps)
                .Cast<UpgradeStep>()
                .ToList();
        }
        public int GetCurrentStepIndex(EUpgradeType upgradeType)
        {
            var upgradeIndexLevel = _upgrades[upgradeType].Level.Value;
            switch (upgradeType)
            {
                case EUpgradeType.Car:
                    return GetCurrentStepIndex(_upgradesConfig.CarUpgrades, upgradeIndexLevel);
                case EUpgradeType.Stunt:
                    return GetCurrentStepIndex(_upgradesConfig.StuntUpgrades, upgradeIndexLevel);
                case EUpgradeType.Income:
                    return GetCurrentStepIndex(_upgradesConfig.IncomeUpgrades, upgradeIndexLevel);
                default:
                    throw new ArgumentOutOfRangeException(nameof(upgradeType), upgradeType, null);
            }
        }

        private int GetCurrentStepIndex<T>(List<UpgradePack<T>> upgradeInfos, int upgradeIndexLevel) where T : UpgradeStep
        {
            int currentIndex = 0;
            foreach (var upgradeInfo in upgradeInfos)
            {
                foreach (var step in upgradeInfo.Steps)
                {
                    if (currentIndex == upgradeIndexLevel)
                    {
                        return upgradeInfo.Steps.IndexOf(step);
                    }
                    currentIndex++;
                }
            }
            throw new ArgumentOutOfRangeException(nameof(upgradeIndexLevel), "Upgrade index level is out of range.");
        }
        public bool IsCanBuyUpgrade(EUpgradeType upgradeType)
        {
            if (IsFullyUpgraded(upgradeType))
                return false;

            var price = GetUpgradeCost(upgradeType);
            return _currencyService.IsEnoughCurrency(ECurrencyType.Coins, price);
        }
        public EUpgradeType GetLowestPriceUpgrade()
        {
            return _upgrades.Aggregate((x, y) => x.Value.GetNextUpgradeCost() < y.Value.GetNextUpgradeCost() ? x : y)
                .Key;
        }
        public bool IsHasAffordableUpgrade(float availableMoney)
        {
            return _upgrades.Any(upgrade => upgrade.Value.GetNextUpgradeCost() <= availableMoney);
        }
        public List<UpgradePack<T>> GetUpgradeInfos<T>(EUpgradeType upgradeType) where T : UpgradeStep
        {
            switch (upgradeType)
            {
                case EUpgradeType.Car:
                    return _upgradesConfig.CarUpgrades.Cast<UpgradePack<T>>().ToList();
                case EUpgradeType.Stunt:
                    return _upgradesConfig.StuntUpgrades.Cast<UpgradePack<T>>().ToList();
                case EUpgradeType.Income:
                    return _upgradesConfig.IncomeUpgrades.Cast<UpgradePack<T>>().ToList();
                default:
                    throw new ArgumentOutOfRangeException(nameof(upgradeType), upgradeType, null);
            }
        }
        private List<UpgradePack<T>> GetUpgradeListByType<T>(EUpgradeType upgradeType) where T : UpgradeStep
        {
            return upgradeType switch
            {
                EUpgradeType.Car => _upgradesConfig.CarUpgrades.Cast<UpgradePack<T>>().ToList(),
                EUpgradeType.Stunt => _upgradesConfig.StuntUpgrades.Cast<UpgradePack<T>>().ToList(),
                EUpgradeType.Income => _upgradesConfig.IncomeUpgrades.Cast<UpgradePack<T>>().ToList(),
                _ => throw new ArgumentOutOfRangeException(nameof(upgradeType), upgradeType, null)
            };
        }
        public List<TStep> GetAllUpgradeSteps<TUpgradeInfo, TStep>(List<TUpgradeInfo> upgradeInfos, Func<TUpgradeInfo, TStep[]> selector)
        {
            return upgradeInfos
                .SelectMany(selector) // Разворачиваем шаги из каждого объекта
                .ToList();
        }
        /*public T GetUpgradeStep<T>(EUpgradeType upgradeType)
        {
            int currentLevel = _upgrades[upgradeType].Level.Value;

            if (_levelDataService.GetCurrentLevelData().IsUseSpecialLevelData)
            {
                return upgradeType switch
                {
                    EUpgradeType.Car => (T)(object)_levelDataService.GetCurrentLevelData().SpecialLevelDataSettings.CarUpgrade,
                    EUpgradeType.Income => (T)(object)_upgradesConfig.IncomeUpgrades[currentLevel],
                    EUpgradeType.Stunt => (T)(object)_levelDataService.GetCurrentLevelData().SpecialLevelDataSettings.StuntUpgrade,
                    _ => throw new ArgumentOutOfRangeException(nameof(upgradeType), upgradeType, null)
                };
            }
            
            return upgradeType switch
            {
                EUpgradeType.Car => (T)(object)_upgradesConfig.CarUpgrades[currentLevel],
                EUpgradeType.Income => (T)(object)_upgradesConfig.IncomeUpgrades[currentLevel],
                EUpgradeType.Stunt => (T)(object)_upgradesConfig.StuntUpgrades[currentLevel],
                _ => throw new ArgumentOutOfRangeException(nameof(upgradeType), upgradeType, null)
            };
        }*/
        public void Dispose()
        {
            OnUpgradeCar = null;
            OnUpgradeStunt = null;
            OnUpgradeIncome = null;
            OnBuyUpgrade = null;
        }
        
        /*public T GetUpgradeInfo<T>(EUpgradeType upgradeType)
        {
            int currentLevel = _upgrades[upgradeType].Level.Value;

            if (_levelDataService.GetCurrentLevelData().IsUseSpecialLevelData)
            {
                return upgradeType switch
                {
                    EUpgradeType.Car => (T)(object)_levelDataService.GetCurrentLevelData().SpecialLevelDataSettings.CarUpgradeInfo,
                    EUpgradeType.Income => (T)(object)_upgradesConfig.IncomeUpgrades[currentLevel],
                    EUpgradeType.Stunt => (T)(object)_levelDataService.GetCurrentLevelData().SpecialLevelDataSettings.StuntUpgradeInfo,
                    _ => throw new ArgumentOutOfRangeException(nameof(upgradeType), upgradeType, null)
                };
            }
            
            return upgradeType switch
            {
                EUpgradeType.Car => (T)(object)_upgradesConfig.CarUpgrades[currentLevel],
                EUpgradeType.Income => (T)(object)_upgradesConfig.IncomeUpgrades[currentLevel],
                EUpgradeType.Stunt => (T)(object)_upgradesConfig.StuntUpgrades[currentLevel],
                _ => throw new ArgumentOutOfRangeException(nameof(upgradeType), upgradeType, null)
            };
        }*/
        public int GetTotalCountOfUpgrades(EUpgradeType upgradeType)
        {
            Debug.Log("TOTAL COUNT OF UPGRADES " + _upgradeSteps[upgradeType].Count);
            return _upgradeSteps[upgradeType].Count;
        }
    }

    public class UpgradeData
    {
        public PrefsInt Level { get; }
        private readonly int[] _upgradePrices;
        public event Action OnUpgrade;
        public UpgradeData(PrefsInt level, int[] upgradePrices)
        {
            Level = level;
            _upgradePrices = upgradePrices;
        }

        public int GetNextUpgradeCost()
        {
            if (IsFullyUpgraded())
                return 0;

            return _upgradePrices[Level.Value + 1];
        }

        public bool IsFullyUpgraded()
        {
            return Level.Value + 1 >= _upgradePrices.Length;
        }
        public void InvokeUpgrade()
        {
            OnUpgrade?.Invoke();
        }
    }
}