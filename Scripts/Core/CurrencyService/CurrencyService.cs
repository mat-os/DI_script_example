using System;
using Configs;
using Game.Scripts.Infrastructure.Services;
using Game.Scripts.Utils.Prefs;

namespace Game.Scripts.Core.CurrencyService
{
    public class CurrencyService : ICurrencyService
    {
        public event Action<ECurrencyType, int> OnCurrencyChanged;

        private readonly DataService _dataService;
        private readonly GameConfig _gameConfig;

        public CurrencyService(DataService dataService, GameConfig gameConfig)
        {
            _gameConfig = gameConfig;
            _dataService = dataService;
        }

        public void TryAddStartMoney()
        {
            if (_dataService.GameSettings.IsItFirstLaunch.Value == true &&  GetCurrencyValue(ECurrencyType.Coins) == 0)
            {
                AddCurrency(ECurrencyType.Coins, _gameConfig.RewardConfig.StartMoneyAmount);
            }
        }

        public void AddCurrency(ECurrencyType type, int amount)
        {
            var save = GetSaveByType(type);
            save.Value += amount;
            OnCurrencyChanged?.Invoke(type, save.Value);
        }

        public bool TrySpendCurrency(ECurrencyType type, int amount)
        {
            if (!IsEnoughCurrency(type, amount))
                return false;

            var save = GetSaveByType(type);
            save.Value -= amount;
            OnCurrencyChanged?.Invoke(type, save.Value);
            
            return true;
        }

        public int GetCurrencyValue(ECurrencyType type) => 
            GetSaveByType(type).Value;

        public bool IsEnoughCurrency(ECurrencyType type, int amount)
        {
            var save = GetSaveByType(type);
            return save.Value - amount >= 0;
        }

        private PrefsInt GetSaveByType(ECurrencyType eCurrencyType)
        {
            switch (eCurrencyType)
            {
                case ECurrencyType.Coins:
                    return _dataService.Currency.Coins;
                case ECurrencyType.Diamonds:
                    return _dataService.Currency.Diamonds;
                default:
                    throw new ArgumentOutOfRangeException(nameof(eCurrencyType), eCurrencyType, 
                        $"There is no currency with provided {eCurrencyType}");
            }
        }
    }
}