using Game.Scripts.Core.CurrencyService;
using Game.Scripts.UI.Widgets.Base;
using Game.Scripts.Utils;
using TMPro;
using UnityEngine;

namespace Game.Scripts.UI.Widgets.CurrencyCounter
{
    public class CurrencyView : MonoBehaviour
    {
        [SerializeField] private RectTransform _currencyIconRoot;
        [SerializeField] private SimpleBounceAnimator _animator;
        [SerializeField] private TextMeshProUGUI _currencyText;
        [SerializeField] private ECurrencyType _currencyType;
        [SerializeField] private AnimationParameterConfig _animationParameterConfig;

        private CurrencyViewPresenter _presenter;
        
        public RectTransform CurrencyIconRoot => _currencyIconRoot;
        public ECurrencyType CurrencyType => _currencyType;

        public AnimationParameterConfig AnimationParameterConfig => _animationParameterConfig;

        public void Construct(CurrencyViewPresenter presenter)
        {
            _presenter = presenter;
        }

        public void Initialize()
        {
            _presenter.OnCurrencyChanged += OnCurrencyChanged_Handler;
            _presenter.OnParticleReachedTarget += OnParticleReachedTarget_Handler;
        }

        public void Dispose()
        {
            _presenter.OnCurrencyChanged -= OnCurrencyChanged_Handler;
            _presenter.OnParticleReachedTarget -= OnParticleReachedTarget_Handler;
        }

        private void OnCurrencyChanged_Handler(string currencyAmount) => 
            _currencyText.text = currencyAmount;

        private void OnParticleReachedTarget_Handler()
        {
            _animator.Bounce();
        }
    }
}