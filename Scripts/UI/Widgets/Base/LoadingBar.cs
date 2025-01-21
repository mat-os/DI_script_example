using Game.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.Widgets.Base
{
    public class LoadingBar : MonoBehaviour
    {
        [SerializeField]private Image _fillImage;

        private float _minValue = 0f;
        private float _maxValue = 1f;

        public void Init(float min, float max)
        {
            _minValue = min;
            _maxValue = max;
            
            // Инициализируем Fill Amount в соответствии с минимальным значением
            _fillImage.fillAmount = MathUtil.Remap(_minValue, _maxValue, 0f, 1f, _minValue);
        }
        public void UpdateProgress(float progress)
        {
            float clampedProgress = Mathf.Clamp(progress, _minValue, _maxValue);
            float mappedProgress = MathUtil.Remap(_minValue, _maxValue, 0f, 1f, clampedProgress);
            _fillImage.fillAmount = mappedProgress;
        }
    }
}