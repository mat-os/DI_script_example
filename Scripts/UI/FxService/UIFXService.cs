using AssetKits.ParticleImage;
using Configs;
using Game.Configs.PrefabsCollection.PrefabsSettings;
using Game.Scripts.Constants;
using Game.Scripts.Core.CurrencyService;
using Game.Scripts.Infrastructure.Services;
using Game.Scripts.UI.Widgets.CurrencyCounter;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.UI.FxService
{
    public class UIFXService : IUIFXService
    {
        private readonly VibrationService _vibrationService;
        private readonly UIFXSettings _uiEffectsSettings;

        public UIFXService(EffectsConfig effectsConfig, VibrationService vibrationService)
        {
            _vibrationService = vibrationService;
            _uiEffectsSettings = effectsConfig.UIFXSettings;
        }
    
        public void SpawnCurrencyMovementEffect(Transform startPos, Transform endPos, CurrencyViewPresenter currencyViewPresenter, ECurrencyType currencyType)
        {
            SpawnCurrencyEffect(currencyType, startPos, endPos, 
                HandleFirstParticleFinished, 
                () => HandleParticleFinished(currencyViewPresenter), 
                HandleLastParticleFinished);
        }

        private void SpawnCurrencyEffect(ECurrencyType type, 
            Transform spawnRoot, 
            Transform attractor,
            UnityAction onFirstParticleFinish,
            UnityAction onParticleFinishedHandler, 
            UnityAction onLastParticleFinish = null)
        {
            ParticleImage effect = Object.Instantiate(_uiEffectsSettings.GetCurrencyEffect(type), spawnRoot);
            effect.attractorTarget = attractor;
            effect.onFirstParticleFinished.AddListener(onFirstParticleFinish);
            effect.onAnyParticleFinished.AddListener(onParticleFinishedHandler);
            effect.onAnyParticleFinished.AddListener(PlayCoinSound);
            effect.onLastParticleFinished.AddListener(()=>
            {
                Object.Destroy(effect.gameObject);
                onLastParticleFinish?.Invoke();
            });
            
            effect.Play();
            
            _vibrationService.Vibrate(VibrationPlaceType.CurrencyEffectFinishedSpread);
        }

        private void HandleFirstParticleFinished()
        {
        }        
        private void HandleLastParticleFinished()
        {
        }    
        private void HandleParticleFinished(CurrencyViewPresenter currencyViewPresenter)
        {
            currencyViewPresenter.RaiseParticleReachedTargetEvent();
        }   
        
        private void PlayCoinSound()
        {
           // _audioService.PlaySoundOneShot(SoundFxConstants.UI_COIN_RECEIVE);
        }
    }
}
