using Game.Scripts.Core.CurrencyService;
using Game.Scripts.UI.Widgets.CurrencyCounter;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.UI.FxService
{
    public interface IUIFXService
    {
        void SpawnCurrencyMovementEffect(Transform startPos, Transform endPos, CurrencyViewPresenter currencyViewPresenter, ECurrencyType currencyType);
    }
}