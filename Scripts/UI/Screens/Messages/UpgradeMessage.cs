using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Scripts.UI.Screens.Base.Screens;
using Game.Scripts.UI.Screens.Serviсes;
using Game.Scripts.Utils;
using TMPro;
using UnityEngine;
using Zenject;

namespace Game.Scripts.UI.Screens.Messages
{
    public class UpgradeMessage : MessageBox
    {
        public TMP_Text UpgradeText;
        public AnimationParameterConfig ShowTextAnimation;
        public AnimationParameterConfig HideTextAnimation;
        public float ShowTextDuration = 1.5f;
        public float MoveDistance = 50f;
        
        private MessageBoxService _messageBoxService;
        
        // Ссылка на последовательность твинов
        private Sequence _sequence;

        [Inject]
        public void Construct(MessageBoxService messageBoxService)
        {
            _messageBoxService = messageBoxService;
        }

        public void SetUpgradeText(string description, float oldValue, float newValue)
        {
            // Завершаем и очищаем предыдущую последовательность твинов
            KillSequence();

            var amount = CalculatePercentageIncrease(oldValue, newValue);
            UpgradeText.text = description + ": +" + amount + "%";

            // Сбрасываем начальные параметры текста
            UpgradeText.transform.localScale = Vector3.zero;
            UpgradeText.transform.localPosition = Vector3.zero;
            UpgradeText.alpha = 0; // Устанавливаем прозрачность текста в 0

            // Создаем новую последовательность твинов
            _sequence = DOTween.Sequence()
                // Появление текста (прозрачность)
                .Append(UpgradeText.DOFade(1, ShowTextAnimation.Duration)
                    .SetEase(ShowTextAnimation.Ease))
                
                // Увеличение масштаба текста
                .Join(UpgradeText.transform.DOScale(1f, ShowTextAnimation.Duration)
                    .SetEase(ShowTextAnimation.Ease))
                
                // Движение вверх текста
                .Join(UpgradeText.transform.DOLocalMoveY(MoveDistance, ShowTextAnimation.Duration)
                    .SetEase(ShowTextAnimation.Ease))
                
                // Ожидание времени показа текста
                .AppendInterval(ShowTextDuration)
                
                // Исчезновение текста (прозрачность)
                .Append(UpgradeText.DOFade(0, HideTextAnimation.Duration)
                    .SetEase(HideTextAnimation.Ease))
                
                // После завершения всего последовательного действия вызываем закрытие экрана
                .OnComplete(() =>
                {
                    _messageBoxService.CloseScreen<UpgradeMessage>();
                });
        }
        
        private void KillSequence()
        {
            if (_sequence != null)
            {
                _sequence.Kill();
                _sequence = null;
            }
        }
        public override UniTask OnCloseComplete()
        {
            KillSequence(); 
            return base.OnCloseComplete();
        }

        public float CalculatePercentageIncrease(float oldValue, float newValue)
        {
            if (oldValue == 0) 
            {
                Debug.LogError("Деление на ноль: старое значение не может быть равно 0.");
                return 0; 
            }

            Debug.Log($"Old Value {oldValue} / {newValue}");
            float percentIncrease = ((newValue - oldValue) / oldValue) * 100f;
            float roundedResult = Mathf.Round(percentIncrease * 10) / 10f; // Округляем до 1 знака после запятой
            return roundedResult;
        }
    }
}
