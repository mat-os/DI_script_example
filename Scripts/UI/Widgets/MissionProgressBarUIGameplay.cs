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
    public class MissionProgressBarUIGameplay : MonoBehaviour, IDisposable
    {
        [Header("UI Elements")]
        [SerializeField] private ProgressBarFill _progressBarFill; 
        [SerializeField] private RectTransform _progressBarTransform; 
        [SerializeField] private TextMeshProUGUI _missionResult;
        [SerializeField] private TextMeshProUGUI _missionDescription;
        
        [Header("Stars")]
        [SerializeField] private StarSimpleUI[] _stars;         
        
        private List<MissionProgress> _activeMissionsOnLevel;
        private UIConfig _uiConfig;
        
        private StarController _starController;
        private Tweener _tweenSmoothUpgrade;
        
        private int _oneStarThreshold;
        private int _twoStarThreshold;
        private int _threeStarThreshold;
        private MissionRepository _missionRepository;

        private int ActiveMissionsCount => _activeMissionsOnLevel.Count;
        
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
            
            _activeMissionsOnLevel = activeMissionsOnLevel;
            
            _starController.SetStarCount(_activeMissionsOnLevel.Count);
            
            PositionStarsOnProgressBar();
            
            _progressBarFill.SetProgressInstant(0, ActiveMissionsCount); 
            
            _missionDescription.text = _missionRepository.MissionSettings[missionConfig[0].EMissionType].TextForGameplayDescriptionLineUI;
        }
        public void UpdateProgressSmooth(int currentValue, int targetValue)
        {
            int displayedValue = Mathf.Clamp(Mathf.RoundToInt(currentValue), 0, targetValue);
            _missionResult.text = displayedValue + "/" + targetValue;

            // Обновляем прогресс бар с плавным значением
            _progressBarFill.SetProgress(currentValue, targetValue);
        }
        public void UpdateProgress(List<MissionProgress> missionProgressList)
        {
            if (missionProgressList == null) 
                return;

            float totalProgress = 0;
            for (int i = 0; i < _activeMissionsOnLevel.Count; i++)
            {
                var missionProgress = _activeMissionsOnLevel[i];
                if (missionProgress.Mission == null) 
                    continue;
                
                if (missionProgress.IsCompleted)
                {
                    _starController.TryActivateStar(i);
                }
            }
            
            totalProgress = _activeMissionsOnLevel.Last().CurrentValue / _threeStarThreshold;
            _progressBarFill.SetProgress(totalProgress, 1); // Прогресс всего барра
        }

        private void PositionStarsOnProgressBar()
        {
            float progressBarWidth = _progressBarTransform.rect.width; // Длина прогресс-бара
            // Вычисляем позиции звезд на прогресс-баре
            SetStarPosition(_stars[0].GetComponent<RectTransform>(), _oneStarThreshold, progressBarWidth);
            SetStarPosition(_stars[1].GetComponent<RectTransform>(), _twoStarThreshold, progressBarWidth);
            SetStarPosition(_stars[2].GetComponent<RectTransform>(), _threeStarThreshold, progressBarWidth);
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
