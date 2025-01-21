using System;

namespace Game.Scripts.Core.CurrencyService
{
    public interface ICurrencyService
    {
        public void AddCurrency(ECurrencyType type, int amount);
        public bool TrySpendCurrency(ECurrencyType type, int amount);
        bool IsEnoughCurrency(ECurrencyType type, int amount);
        event Action<ECurrencyType, int> OnCurrencyChanged;
        int GetCurrencyValue(ECurrencyType type);
    }
}