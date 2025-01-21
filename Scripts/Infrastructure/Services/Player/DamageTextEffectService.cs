using Configs;
using DG.Tweening;
using Game.Scripts.Configs;
using TMPro;
using UnityEngine;

namespace Game.Scripts.Infrastructure.Services.Player
{
    public class DamageTextEffectService 
    {
        private readonly DamageTextConfig _config;
        private int _damageTextSpawnThreshold;

        public DamageTextEffectService(EffectsConfig effectsConfig)
        {
            _config = effectsConfig.DamageTextConfig;
            _damageTextSpawnThreshold = _config.DamageTextSpawnThreshold;
        }

        public void CreateDamageTextEffect(Vector3 position, int damageAmount)
        {
            if(damageAmount < _damageTextSpawnThreshold)
                return;

            Debug.Log(position);
            // Создаем экземпляр текста урона
            var damageText = Object.Instantiate(_config.DamageTextPrefab, position, Quaternion.identity);
            //damageText.text = "+" + damageAmount.ToString();
            damageText.text = damageAmount.ToString();

            damageText.transform.localScale = Vector3.zero;
            
            Sequence damageSequence = DOTween.Sequence();
            damageSequence.Append(damageText.transform.DOScale(Vector3.one, _config.ScaleConfig.Duration).SetEase(_config.ScaleConfig.Ease));
            damageSequence.Join(damageText.DOColor(_config.EndColor, _config.FadeConfig.Duration).SetEase(_config.FadeConfig.Ease));
            damageSequence.Join(damageText.transform.DOMoveY(position.y + _config.MoveUpOffset, _config.MoveUpConfig.Duration).SetEase(_config.MoveUpConfig.Ease));
            damageSequence.Join(damageText.DOFade(0, _config.FadeConfig.Duration).SetEase(_config.FadeConfig.Ease).SetDelay(_config.FadeDelay));
            damageSequence.OnComplete(() => Object.Destroy(damageText.gameObject));
        }
    }
}