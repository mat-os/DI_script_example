using Configs;
using DG.Tweening;
using Game.Scripts.Configs;
using TMPro;
using UnityEngine;

namespace Game.Scripts.Infrastructure.Services.Player
{
    public class ScoreTextEffectService 
    {
        private readonly DamageTextConfig _config;

        public ScoreTextEffectService(EffectsConfig effectsConfig)
        {
            _config = effectsConfig.ScoreTextConfig;
        }

        public void CreateScoreTextEffect(Vector3 position, int score)
        {
            // Создаем экземпляр текста урона
            var damageText = Object.Instantiate(_config.DamageTextPrefab, position, Quaternion.identity);
            damageText.text = "+" + score;

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