using System;
using System.Collections.Generic;
using Configs;
using DG.Tweening;
using Game.Scripts.Configs.Level;
using Game.Scripts.Infrastructure.Services;
using Game.Scripts.Utils;
using Game.Scripts.Utils.Debug;
using ModestTree;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.Widgets
{
    public class MissionProgressBarUI : MonoBehaviour
    {
        //public Action<int> OnStarActivated;
        //[SerializeField] private StarUI _starsIcon;   
        [SerializeField] private TMP_Text _reward;   

        [SerializeField] private TextMeshProUGUI _missionDescription;
        [SerializeField] private TextMeshProUGUI _missionResult;
        
        [SerializeField] private ProgressBarFill _progressBarFill;
        [SerializeField] private RectTransform _progressBarTransform; // RectTransform прогресс-бара, чтобы вычислить длину
        
        private UIConfig _uiConfig;
        private MissionRepository _missionRepository;
        private Tweener _tweenSmoothUpgrade;

        [Inject]
        public void Construct(UIConfig uiConfig, MissionRepository missionRepository)
        {
            _missionRepository = missionRepository;
            _uiConfig = uiConfig;
            //_starsIcon.Initialize(_uiConfig);
        }
        public void ShowFinalResult(MissionProgress missionProgress, LevelMission missionConfig)
        {
            UpdateProgressSmooth((int)missionProgress.CurrentValue, missionConfig.TargetValue);
            
            if (missionProgress.CurrentValue >= missionConfig.TargetValue)
            {
               // _starsIcon.Activate();
            }
        }
        public void OnOpenStart(LevelMission levelMission)
        {
            _tweenSmoothUpgrade?.Kill();

            var missionSettings = _missionRepository.MissionSettings[levelMission.EMissionType];
            _missionDescription.text = missionSettings.TextForLevelCompleteProgressBarUI;
            
            if(_missionResult != null) 
                _missionResult.text = 0 + "/" + levelMission.TargetValue;
            
            _progressBarFill.SetProgressInstant(0, 1);
            //_starsIcon.Deactivate();
            _reward.text = $"+{levelMission.Reward}";
        }

        private void UpdateProgressSmooth(int currentValue, int targetValue)
        {
            if (_missionResult != null)
            {
                // Плавная анимация с использованием float
                _tweenSmoothUpgrade = DOVirtual.Float(0, currentValue, _uiConfig.FinalResultProgressBarFillAnimation.Duration, value =>
                    {
                        // Округление только для отображения текста
                        int displayedValue = Mathf.Clamp(Mathf.RoundToInt(value), 0, targetValue);
                        _missionResult.text = displayedValue + "/" + targetValue;

                        // Обновляем прогресс бар с плавным значением
                        _progressBarFill.SetProgress(value, targetValue);
                    })
                    .SetEase(_uiConfig.FinalResultProgressBarFillAnimation.Ease);
            }
        }

        public void OnCloseComplete()
        {
            _tweenSmoothUpgrade?.Kill();
        }
    }
}