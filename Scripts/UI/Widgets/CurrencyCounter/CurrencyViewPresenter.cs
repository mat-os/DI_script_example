using System;
using System.Collections.Generic;
using Configs;
using DG.Tweening;
using Game.Scripts.Core.CurrencyService;
using Game.Scripts.Infrastructure.Services;
using Game.Scripts.Utils;
using Game.Scripts.Utils.Debug;

namespace Game.Scripts.UI.Widgets.CurrencyCounter
{
    public class CurrencyViewPresenter : IDisposable
    {
        public event Action<string> OnCurrencyChanged;
        public event Action OnParticleReachedTarget; 
        
        private readonly ICurrencyService _currencyService;
        private readonly VibrationService _vibrationService;
        private readonly AnimationParameterConfig _animationParameterConfig;
        private readonly ECurrencyType _currencyType;
        private readonly Dictionary<int, string> _lockReasons = new ();

        private int _currencyAmount;
        
        public CurrencyViewPresenter(ICurrencyService currencyService, ECurrencyType currencyType, VibrationService vibrationService, AnimationParameterConfig animationParameterConfig)
        {
            _currencyService = currencyService;
            _animationParameterConfig = animationParameterConfig;
            _currencyType = currencyType;
            _vibrationService = vibrationService;
        }

        public void Initialize()
        {
            _currencyAmount = _currencyService.GetCurrencyValue(_currencyType);
            OnCurrencyChanged?.Invoke(_currencyAmount.Kilo());
            SubscribeToEvents();
        }

        public void Dispose()
        {
            DOTween.Kill(this, true);
            UnsubscribeFromEvents();
        }

        public void RaiseParticleReachedTargetEvent() => OnParticleReachedTarget?.Invoke();

        private void SubscribeToEvents() =>
            _currencyService.OnCurrencyChanged += OnCurrencyChanged_Handler;

        private void UnsubscribeFromEvents() => 
            _currencyService.OnCurrencyChanged -= OnCurrencyChanged_Handler;

        public void FreezeRefresh(string reason)
        {
            if (_lockReasons.ContainsKey(reason.GetHashCode()))
                return;
            
            CustomDebugLog.Log("[CURRENCY] FreezeRefresh Reason " + reason);
            _lockReasons.Add(reason.GetHashCode(), reason);
        }

        public void UnfreezeRefresh(string reason, bool shouldTriggerRefresh = false)
        {
            if (_lockReasons.ContainsKey(reason.GetHashCode()))
                _lockReasons.Remove(reason.GetHashCode());
            
            CustomDebugLog.Log("[CURRENCY] UnfreezeRefresh Reason " + reason);

            if (shouldTriggerRefresh)
                OnCurrencyChanged_Handler(_currencyType, _currencyService.GetCurrencyValue(_currencyType));
        }

        private void OnCurrencyChanged_Handler(ECurrencyType type, int value)
        {
            if (_currencyType != type || _lockReasons.Count != 0)
                return;

            DOVirtual.Int(_currencyAmount, value, _animationParameterConfig.Duration, currency =>
                {
                    _currencyAmount = currency;
                    //_vibrationService.Vibrate(VibrationPlaceType.CurrencyEffectReachedTarget);

                    OnCurrencyChanged?.Invoke(currency.Kilo());
                })
                .SetEase(_animationParameterConfig.Ease)
                .OnComplete(() => _currencyAmount = value)
                .SetId(this);
        }
    }
}