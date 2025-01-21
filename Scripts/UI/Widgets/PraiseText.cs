using System;
using DG.Tweening;
using Game.Scripts.Utils;
using TMPro;
using UnityEngine;

namespace Game.Scripts.UI.Widgets
{
    public class PraiseText : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private AnimationParameterConfig _showParameter;
        [SerializeField] private float _stayDuration;
        [SerializeField] private AnimationParameterConfig _hideParameter;
        [SerializeField] private float _fadeOutDelay = 0.1f; // Добавим небольшую задержку

        private Action<PraiseText> _onDestroyCallback;
        private Sequence _animationSequence;
        private Sequence _fadeAnimationSequence;
        private bool _isCleared = false;

        public void Initialize(string text, int score, Action<PraiseText> onDestroyCallback)
        {
            _text.text = text + " +<color=#FFE868>" + score + "</color>";
            _onDestroyCallback = onDestroyCallback;
            AnimatePraiseText();
        }

        public void SetInactiveStyle()
        {
            if (gameObject.activeInHierarchy == false) 
                return;
            
            // Изменяем стиль для старых сообщений
            _fadeAnimationSequence = DOTween.Sequence()
                .Append(transform.DOScale(0.8f, 0.3f))
                .Insert(0, _text.DOFade(0.5f, 0.3f));
            
        }

        private void AnimatePraiseText()
        {
            if (gameObject.activeInHierarchy == false) 
                return;

            // Останавливаем любую ранее запущенную анимацию
            _animationSequence?.Kill();

            // Начальная настройка
            _rectTransform.localScale = Vector3.zero;
            _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, 0);

            // Секвенция анимации
            _animationSequence = DOTween.Sequence()
                // Плавное появление
                .Append(_rectTransform.DOScale(Vector3.one, _showParameter.Duration)
                    .SetEase(_showParameter.Ease))
                .Join(_text.DOFade(1f, _showParameter.Duration))
                
                // Задержка
                .AppendInterval(_stayDuration)
                
                // Плавное исчезновение
                .Append(_rectTransform.DOScale(Vector3.zero, _hideParameter.Duration)
                    .SetEase(_hideParameter.Ease))
                .Join(_text.DOFade(0f, _hideParameter.Duration))
                
                // Завершающее удаление
                .AppendCallback(() => 
                {
                    _onDestroyCallback?.Invoke(this);
                    Destroy(gameObject);
                });
        }
        private void OnDestroy()
        {
            if (_isCleared) 
                return;
            _isCleared = true;

            // Останавливаем все анимации, если объект отключается
            _animationSequence?.Kill();
            _fadeAnimationSequence?.Kill();
        }


        public void Clear()
        {
            if (_isCleared) 
                return;
            
            _isCleared = true;

            _animationSequence?.Kill();
            _fadeAnimationSequence?.Kill();

            Destroy(gameObject);
        }
    }
}
