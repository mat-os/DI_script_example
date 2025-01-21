using System;
using System.Collections.Generic;
using System.Linq;
using Configs;
using DG.Tweening;
using Game.Scripts.Infrastructure.Services;
using Game.Scripts.Infrastructure.Services.Level;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.Widgets
{
    public class MissionProgressBarUIFinalScreen : MonoBehaviour, IDisposable
    {
        [Header("UI Elements")]
        [SerializeField] private ProgressBarFill _progressBarFill; 
        [SerializeField] private RectTransform _progressBarTransform; 
        [SerializeField] private TextMeshProUGUI _missionResult;
        [SerializeField] private TextMeshProUGUI _missionDescription;
        
        [Header("Stars")]
        [SerializeField] private StarSimpleUI[] _stars;         
        
        private UIConfig _uiConfig;
        
        private StarController _starController;
        private Tweener _tweenSmoothUpgrade;
        
        private int _oneStarThreshold;
        private int _twoStarThreshold;
        private int _threeStarThreshold;
        private MissionRepository _missionRepository;

        
        [Inject]
        public void Construct( UIConfig uiConfig, MissionRepository missionRepository)
        {
            _missionRepository = missionRepository;
            _uiConfig = uiConfig;
            
            _starController = new StarController(_stars.Cast<IStarUI>().ToArray());
        }
        public void OnOpenStart(List<LevelMission> missionConfig, List<MissionProgress> activeMissionsOnLevel)
        {
            _oneStarThreshold = missionConfig[0].TargetValue;
            _twoStarThreshold = missionConfig[1].TargetValue;
            _threeStarThreshold = missionConfig[2].TargetValue;
            
            _starController.SetStarCount(missionConfig.Count);
            PositionStarsOnProgressBar();
            
            _progressBarFill.SetProgressInstant(0, missionConfig.Count); 
            
            var missionSettings = _missionRepository.MissionSettings[missionConfig[0].EMissionType];
            _missionDescription.text = missionSettings.TextForLevelCompleteProgressBarUI;
        }
        public void UpdateProgressSmooth(int currentValue, int targetValue)
        {
            // Плавная анимация 
            _tweenSmoothUpgrade = DOVirtual.Float(0, currentValue, _uiConfig.FinalResultProgressBarFillAnimation.Duration, value =>
                {
                    // Округление только для отображения текста
                    int displayedValue = Mathf.Clamp(Mathf.RoundToInt(value), 0, targetValue);
                    _missionResult.text = displayedValue + "/" + targetValue;

                    // Обновляем прогресс бар с плавным значением
                    _progressBarFill.SetProgress(value, targetValue);
                    
                    TryActivateStars(value);
                })
                .SetEase(_uiConfig.FinalResultProgressBarFillAnimation.Ease);
        }

        private void TryActivateStars(float value)
        {
            if (value >= _oneStarThreshold)
            {
                _starController.TryActivateStar(0);
            }
            if (value >= _twoStarThreshold)
            {
                _starController.TryActivateStar(1);
            }
            if (value >= _threeStarThreshold)
            {
                _starController.TryActivateStar(2);
            }
        }

        private void PositionStarsOnProgressBar()
        {
            float progressBarWidth = _progressBarTransform.rect.width; // Длина прогресс-бара
            // Вычисляем позиции звезд на прогресс-баре
            SetStarPosition(_stars[0].StarRectTransform, _oneStarThreshold, progressBarWidth);
            SetStarPosition(_stars[1].StarRectTransform, _twoStarThreshold, progressBarWidth);
            SetStarPosition(_stars[2].StarRectTransform, _threeStarThreshold, progressBarWidth);
        }

        private void SetStarPosition(RectTransform starIcon, int starThreshold, float progressBarWidth)
        {
            // Рассчитываем нормализованную позицию для звезды (0 - в начале, 1 - в конце)
            float normalizedPosition = (float)starThreshold / _threeStarThreshold;

            // Перемещаем звезду вдоль оси X относительно начала прогресс-бара
            Vector2 newPosition = new Vector2(normalizedPosition * progressBarWidth, starIcon.anchoredPosition.y);
            starIcon.anchoredPosition = newPosition;
        }
        public void Dispose()
        {
        }
    }
}
