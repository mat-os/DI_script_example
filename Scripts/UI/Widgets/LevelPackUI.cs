using System;
using Configs;
using DG.Tweening;
using Game.Scripts.Configs.Level;
using Game.Scripts.Infrastructure.Services;
using Game.Scripts.Infrastructure.Services.Level;
using Game.Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.Widgets
{
    public class LevelPackUI : MonoBehaviour
    {
        [SerializeField] private Button _button;
        //[SerializeField] private TMP_Text  _levelNameText;
        [SerializeField] private Image _levelImage;
        [SerializeField] private GameObject _activeView;

        [Header("Lock")]
        [SerializeField] private GameObject _lockView;
        [SerializeField] private Image _levelImageFill;
        [SerializeField] private Image _levelImageLock;
        [SerializeField] private AnimationParameterConfig _fillAnimation;
        
        [Header("Stats")]
        [SerializeField] private ProgressBarFill _progressBar;
        [SerializeField] private CanvasGroup _stats;
        [SerializeField] private TMP_Text  _starsCountText;
        [SerializeField] private TMP_Text  _trophyCountText;
        [SerializeField] private TMP_Text  _actNameText;
        
        private LevelPackConfig _levelPackConfig;
        private LevelStarsService _levelStarsService;
        private LevelTrophyService _levelTrophyService;
        private int _index;

        private LevelPackService _levelPackService;

        public LevelPackConfig GetPackConfig() => 
            _levelPackConfig;

        [Inject]
        public void Construct(LevelStarsService levelStarsService, LevelTrophyService levelTrophyService, UIConfig uiConfig, LevelPackService levelPackService)
        {
            _levelPackService = levelPackService;
            _levelTrophyService = levelTrophyService;
            _levelStarsService = levelStarsService;
        }

        public void Initialize(int index, LevelPackConfig levelPackConfig, Action onSelectLevelPack)
        {
            _index = index + 1;
            _levelPackConfig = levelPackConfig;
            //_levelNameText.text = levelPackConfig.Name;
            
            _levelImage.sprite = levelPackConfig.Image;
            _levelImageFill.sprite = levelPackConfig.Image;
            _levelImageLock.sprite = levelPackConfig.Image;
            
            _button.onClick.AddListener(() => onSelectLevelPack?.Invoke());
        }

        public void UpdateData(bool isLocked)
        {
            _actNameText.text = _index.ToString() + ". " + _levelPackConfig.Name;
            
            _lockView.SetActive(isLocked);
            _activeView.SetActive(isLocked == false);

            if (isLocked)
            {

            }
            else
            {
                var totalLevelsCount = _levelPackConfig.LevelDataConfigs.Count;
                UpdateStatisctic(totalLevelsCount);
                
                var completedLevelsCount = _levelPackService.GetCountOfCompletedLevelsInPack(_levelPackConfig);
                var completedPercent = completedLevelsCount / totalLevelsCount;
                _levelImageFill.gameObject.SetActive(completedPercent < 1);
                if (completedPercent == 0)
                {
                    _levelImageFill.fillAmount = 0;
                }
                else if (completedPercent < 1)
                {
                    _levelImageFill.fillAmount = 0;
                    _levelImageFill.DOFillAmount(1 - completedPercent, _fillAnimation.Duration).SetEase(_fillAnimation.Ease).SetSpeedBased();
                }
                else
                {
                    _levelImageFill.fillAmount = 0;
                }
            }
        }

        private void UpdateStatisctic(int totalLevelsCount)
        {
            var earnedStars = _levelStarsService.GetCountOfEarnedStarsInPack(_levelPackConfig);
            var startOnThisPackTotalCount = totalLevelsCount * 3;
            _starsCountText.text = $"{earnedStars}/{startOnThisPackTotalCount}";
            
            var earnedTrophy = _levelTrophyService.GetCountOfCollectedTrophyInPack(_levelPackConfig);
            _trophyCountText.text = $"{earnedTrophy}/{totalLevelsCount}";
            _progressBar.SetProgressInstant(earnedStars, startOnThisPackTotalCount);
        }

        public void SetIsInteractable(bool isInteractable)
        {
            _button.interactable = isInteractable;
            var fade = isInteractable ? 1 : 0;
            _stats.DOFade(fade, 0.2f);
        }
    }
}