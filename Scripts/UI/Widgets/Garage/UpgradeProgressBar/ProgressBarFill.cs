using Configs;
using DG.Tweening;
using Game.Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.Widgets
{
    public class ProgressBarFill : ProgressBar
    {
        [SerializeField] private Image _fillImage;    // Image с Fill Amount
        [SerializeField] private TMP_Text _percentageText; // Текст процента
        [SerializeField] private Transform _edgeIndicator;  // Объект на краю

        private Tween _fillTween; // Сохраняем текущую анимацию

        public float GetCurrentFillAmount() => 
            _fillImage.fillAmount;

        // Устанавливаем прогресс с анимацией
        public override void SetProgress(float currentLevel, float maxLevel)
        {
            float targetProgress = currentLevel / maxLevel;

            _fillImage.fillAmount = targetProgress; 
            UpdatePercentageText(targetProgress); 
            //UpdateEdgeIndicatorPosition();
        }

        // Обновляем текст процента
        private void UpdatePercentageText(float progress)
        {
            if (_percentageText != null)
            {
                _percentageText.text = Mathf.RoundToInt(progress * 100) + "%";
            }
        }


        // На случай, если нужно мгновенно обновить прогресс без анимации
        public void SetProgressInstant(float currentLevel, float maxLevel)
        {
            _fillTween?.Kill(); // Убиваем анимацию, если она идет
            float progress = currentLevel / maxLevel;

            // Обновляем всё мгновенно
            _fillImage.fillAmount = progress;
            
            UpdatePercentageText(progress);
            //UpdateEdgeIndicatorPosition();
        }
        
        
        // Обновляем позицию edgeIndicator
        /*private void UpdateEdgeIndicatorPosition()
        {
            if (_edgeIndicator != null)
            {
                RectTransform fillRect = _fillImage.GetComponent<RectTransform>();
                RectTransform edgeRect = _edgeIndicator.GetComponent<RectTransform>();

                // Рассчитываем позицию края на основе fillAmount
                float fillWidth = fillRect.rect.width;
                float newXPos = _fillImage.fillAmount * fillWidth;

                // Устанавливаем новую позицию индикатора
                edgeRect.anchoredPosition = new Vector2(newXPos, edgeRect.anchoredPosition.y);
            }
        }*/
    }
}