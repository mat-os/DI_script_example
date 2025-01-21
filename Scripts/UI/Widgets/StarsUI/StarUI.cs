using System;
using AssetKits.ParticleImage;
using Configs;
using DG.Tweening;
using Game.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.Widgets
{
    public class StarUI : MonoBehaviour, IStarUI
    {
        [SerializeField] private ParticleImage _particles;
        [SerializeField] private Image _starImage;
        
        private UIConfig _uiConfig;
        private Tween _currentTween; 
        public void Initialize(UIConfig uiConfig)
        {
            _uiConfig = uiConfig;
        }
        public void PlayActivateAnimation()
        {
            // Убиваем текущую анимацию, чтобы избежать конфликтов при повторных вызовах
            _currentTween?.Kill();

            // Активируем частицы
            _particles.SetActive(true);
            _particles.Play();
        
            // Устанавливаем начальную спрайт-звезду и начальный масштаб
            _starImage.sprite = _uiConfig.ActiveStar;
            _starImage.transform.localScale = Vector3.one * _uiConfig.ActiveStarScale; // Например, 0.6x или 0.5x от оригинала

            // Последовательность анимаций для "сочной" звезды
            _currentTween = DOTween.Sequence()
                // 1️⃣ Масштабируем звезду с эффектом "пружины"
                .Append(_starImage.transform
                    .DOScale(1.3f, _uiConfig.ShowStarScaleAnimation.Duration * 0.6f) // Небольшой "выплеск" до 130% от размера
                    .SetEase(Ease.OutBack)
                )
                // 2️⃣ Быстрая обратная анимация к размеру 1
                .Append(_starImage.transform
                    .DOScale(1f, _uiConfig.ShowStarScaleAnimation.Duration * 0.4f)
                    .SetEase(Ease.OutBack)
                )
                // 3️⃣ Дополнительный эффект вращения для динамики
                .Join(_starImage.transform
                    .DORotate(new Vector3(0, 0, 360), _uiConfig.ShowStarScaleAnimation.Duration, RotateMode.FastBeyond360)
                    .SetEase(Ease.OutCubic)
                )
                // 4️⃣ Эффект "вспышки" по яркости или альфе
                .Join(_starImage.DOFade(0.6f, _uiConfig.ShowStarScaleAnimation.Duration * 0.2f) // Уменьшаем прозрачность
                        .SetLoops(2, LoopType.Yoyo) // Делаем два "мигания"
                )
                .OnComplete(() =>
                {
                    // Здесь можно отключить частицы, если нужно
                    //_particles.SetActive(false);
                });
        }

        public void Deactivate()
        {
            _particles.Stop(true);
            _particles.SetActive(false);
            _starImage.sprite = _uiConfig.InactiveStar;
        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

        private void OnDisable()
        {
            _particles.Stop(true);
            _particles.SetActive(false);
            
            // Убиваем текущую анимацию
            _currentTween?.Kill();
            _currentTween = null;
        }
    }
}