using System;
using Configs;
using DanielLochner.Assets.SimpleScrollSnap;
using DG.Tweening;
using Game.Scripts.Core.CurrencyService;
using Game.Scripts.Infrastructure.Services;
using Game.Scripts.Infrastructure.Services.Analytics;
using Game.Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.Widgets
{
    public class LobbySelectPageButton : MonoBehaviour
    {
        [SerializeField] private ESelectPageButtonType _selectPageButtonType;
        [SerializeField] private int pageIndex;  // Индекс страницы, на которую ссылается эта кнопка
        
        [Header("Settings")]
        [SerializeField] private Button button;  // Ссылка на кнопку

        [SerializeField] private int selectedFontSize = 24; 
        [SerializeField] private int defaultFontSize = 18;  
        [SerializeField]private TMP_Text tmpButtonText;  
        
        [Header("Bg")]
        [SerializeField] private Image image;  
        
        [SerializeField] private Sprite defaultSprite;  
        [SerializeField] private Sprite selectedSprite;  
        
        [Header("Icon")]
        [SerializeField] private Image IconImage; 
        
        [Header("Icon")]
        [SerializeField] private Image Notification; 
        
        [Header("Locked Message")]
        [SerializeField] private Transform _lockedMessage; 
        [SerializeField] private CanvasGroup _lockedMessageCanvasGroup; 
        [SerializeField] private float _showDuration = 2f; // Время, которое сообщение будет висеть
        [SerializeField] private float _fadeDuration = 0.5f; // Длительность исчезновения
        [SerializeField] private float _scaleDuration = 0.5f;
        
        [Header("Other")]
        [SerializeField] private float defaultAncoredPos;  
        [SerializeField] private float selectedAncoredPos;  
        [SerializeField] private AnimationParameterConfig _animationParameterConfig;

        [Header("Analytics")]
        [SerializeField] private bool _isSendEvents;
        [SerializeField] private string _eventName;
        
        private SimpleScrollSnap _scrollSnap;
        private Action<int> _onButtonClicked;
        private DataService _dataService;
        
        private Tween _currentTween;
        private LevelsRepository _levelsRepository;
        private UpgradeService _upgradeService;
        private CurrencyService _currencyService;
        private AnalyticService _analyticService;
        public int PageIndex => pageIndex;

        [Inject]
        public void Construct(DataService dataService, LevelsRepository levelsRepository, UpgradeService upgradeService, CurrencyService currencyService, AnalyticService analyticService)
        {
            _analyticService = analyticService;
            _currencyService = currencyService;
            _upgradeService = upgradeService;
            _levelsRepository = levelsRepository;
            _dataService = dataService;
        }
        public void Initialize(SimpleScrollSnap scrollSnap, Action<int> onButtonClicked)
        {
            _onButtonClicked = onButtonClicked;
            _scrollSnap = scrollSnap;
            
            button.onClick.AddListener(OnButtonClickedHandler);
            _lockedMessage.gameObject.SetActive(false);
        }

        public void OnOpenStart()
        {
            if (Notification != null)
            {
                var isCanShow = _upgradeService.IsHasAffordableUpgrade(_currencyService.GetCurrencyValue(ECurrencyType.Coins)) &&
                    _dataService.Upgrades.IsShowUpgradeButtons.Value == true || _levelsRepository.IsAllLevelsUnlocked;
                Notification.gameObject.SetActive(isCanShow);
            }
        }

        private void OnButtonClickedHandler()
        {
            if (_selectPageButtonType == ESelectPageButtonType.Garage)
            {
                if (_dataService.Upgrades.IsShowUpgradeButtons.Value == true || _levelsRepository.IsAllLevelsUnlocked)
                {
                    _scrollSnap.GoToPanel(pageIndex);
                    _onButtonClicked?.Invoke(pageIndex);
                    
                    _analyticService.LogUIClick(_eventName);
                }
                else
                {
                    ShowLockedMessage();
                }
            }
            else if (_selectPageButtonType is ESelectPageButtonType.Levels 
                     or ESelectPageButtonType.Challanges 
                     or ESelectPageButtonType.Customize)
            {
                _scrollSnap.GoToPanel(pageIndex);
                _onButtonClicked?.Invoke(pageIndex);
            }
            else
            {
                ShowLockedMessage();
            }
        }

        public void ShowLockedMessage()
        {
            // Убиваем предыдущую анимацию, чтобы избежать конфликтов
            _currentTween?.Kill();

            // Сбрасываем начальные параметры
            _lockedMessage.localScale = Vector3.zero;
            _lockedMessageCanvasGroup.alpha = 1;
            _lockedMessage.gameObject.SetActive(true);

            // Последовательность анимаций
            _currentTween = DOTween.Sequence()
                .Append(_lockedMessage.DOScale(Vector3.one, _scaleDuration).SetEase(Ease.OutBack)) // Масштабируем от 0 до 1
                .AppendInterval(_showDuration) // Делаем паузу
                .Append(_lockedMessageCanvasGroup.DOFade(0, _fadeDuration)) // Исчезаем в альфу
                .OnComplete(() => 
                {
                    _lockedMessage.localScale = Vector3.zero;
                    _lockedMessage.gameObject.SetActive(false);
                });
        }

        // Метод для обновления стиля кнопки в зависимости от того, выбрана ли она

        public void UpdateButtonStyle(bool isSelected)
        {
            Debug.Log("isSelected  " + isSelected + gameObject);
            tmpButtonText.fontSize = isSelected ? selectedFontSize : defaultFontSize;
            image.sprite = isSelected ? selectedSprite : defaultSprite;
            image.SetNativeSize();
            //image.rectTransform.anchoredPosition
            var imagePos = isSelected ? selectedAncoredPos : defaultAncoredPos;
            IconImage.transform.DOLocalMoveY(imagePos, _animationParameterConfig.Duration)
                .SetEase(_animationParameterConfig.Ease);
            
            if(Notification != null)
                Notification.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            _onButtonClicked = null;
        }
    }

    public enum ESelectPageButtonType
    {
        None,
        Shop,
        Levels,
        Garage,
        Challanges,
        Customize
    }
}