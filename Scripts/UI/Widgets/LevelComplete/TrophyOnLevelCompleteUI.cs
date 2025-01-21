using System;
using System.Collections;
using Configs;
using DG.Tweening;
using Game.Scripts.Infrastructure.Services;
using Game.Scripts.Infrastructure.Services.Level;
using Game.Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Random = UnityEngine.Random;

namespace Game.Scripts.UI.Widgets.LevelComplete
{
    public class TrophyOnLevelCompleteUI : MonoBehaviour
    {
        public event Action OnTrophyAnimationComplete; // Событие для завершения анимации

        [SerializeField] private Image _trophy; 
        [SerializeField] private Transform _trophyRoot; 
        [SerializeField] private GameObject _trophyUiEffect;
        [SerializeField] private CanvasGroup _canvasGroup; 
        [SerializeField] private AnimationParameterConfig _trophyShowAnimation; 
        [SerializeField] private AnimationParameterConfig _hideAnimation; 
        [SerializeField] private AnimationParameterConfig _moveAnimation; 

        [SerializeField]private Transform _trophyActivePosition;
        [SerializeField]private float _startActiveTrophyScaleMultiplier;
        [SerializeField]private Vector3 _trophyActiveScale;
        [SerializeField]private GameObject _tapToContinueText;
        [SerializeField]private TMP_Text _trophyText;
        
        private Vector3 _trophyDefaultPosition;
        private Vector3 _trophyDefaultScale;
        
        private UIConfig _uiConfig;
        private LevelTrophyService _levelTrophyService;
        private LevelDataService _levelDataService;
        private string _defaultText;

        [Inject]
        public void Construct(UIConfig uiConfig, LevelTrophyService levelTrophyService, LevelDataService levelDataService)
        {
            _levelDataService = levelDataService;
            _levelTrophyService = levelTrophyService;
            _uiConfig = uiConfig;
        }

        private void Awake()
        {
            _trophyDefaultPosition = _trophyRoot.transform.position;
            _trophyDefaultScale = _trophyRoot.transform.localScale;
            _defaultText = _trophyText.text;
        }

        public void SetupTrophy(bool isTrophyCollected)
        {
            _trophy.sprite = isTrophyCollected ? _uiConfig.ActiveTrophy : _uiConfig.InactiveTrophy;
            _trophyUiEffect.gameObject.SetActive(isTrophyCollected);

            if (isTrophyCollected)
            {
                PlayCollectTrophyAnimation();
            }
            else
            {
                SetupUncollectedTrophy();
            }
        }

        private void SetupUncollectedTrophy()
        {
            _trophyRoot.transform.localScale = _trophyDefaultScale; 
            _trophyRoot.transform.position = _trophyDefaultPosition;
            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.gameObject.SetActive(false);
        }

        private void PlayCollectTrophyAnimation()
        {
            var trophyProbabilityForCollect = _levelDataService.GetCurrentLevelData().TrophyProbabilityForCollect;
            var probability = Random.Range(trophyProbabilityForCollect.x, trophyProbabilityForCollect.y);
// Приводим к двум знакам после запятой, чтобы точно получить дробную часть
            probability = Mathf.Round(probability * 100) / 100f;
            string updatedDescription = _defaultText;
            updatedDescription = updatedDescription.Replace("{x}", probability.ToString("F2") + "%");
            _trophyText.text = updatedDescription;

            
            _canvasGroup.gameObject.SetActive(true);
            _canvasGroup.alpha = 1;
            _canvasGroup.blocksRaycasts = true;

            _trophyRoot.transform.localScale = _trophyActiveScale * _startActiveTrophyScaleMultiplier;
            _trophyRoot.transform.position = _trophyActivePosition.position;
            _trophyRoot.transform.DOScale(_trophyActiveScale, _trophyShowAnimation.Duration)
                .SetEase(_trophyShowAnimation.Ease) // Эффект "отскока"
                .OnComplete(() =>
                {
                    // Ждем N секунд перед ожиданием тапа игрока
                    StartCoroutine(WaitForPlayerTapCoroutine());
                });
        }

        private IEnumerator WaitForPlayerTapCoroutine()
        {
            // Ждем N секунд
            yield return new WaitForSeconds(_uiConfig.TrophyDisplayDuration);
            
            _tapToContinueText.gameObject.SetActive(true);

            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
            
            CloseTrophyUI();
            OnTrophyAnimationComplete?.Invoke(); // Уведомляем, что анимация завершена
        }

        private void CloseTrophyUI()
        {
            // Затемняем экран
            _canvasGroup.DOFade(0, _hideAnimation.Duration)
                .SetEase(_hideAnimation.Ease)
                .OnComplete(() =>
                {
                    // Возвращаем кубок на стандартное место и масштаб
                    _trophyRoot.transform.DOMove(_trophyDefaultPosition, _moveAnimation.Duration)
                        .SetEase(_moveAnimation.Ease);
                    _trophyRoot.transform.DOScale(_trophyDefaultScale, _moveAnimation.Duration)
                        .SetEase(_moveAnimation.Ease);
                    
                    _canvasGroup.blocksRaycasts = false;
                });
        }
    }
}