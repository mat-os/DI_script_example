using System;
using Configs;
using DG.Tweening;
using Game.Scripts.Infrastructure.Services;
using Game.Scripts.Infrastructure.Services.Player;
using Game.Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.Widgets
{
    public class EnergyBarUI : MonoBehaviour, IDisposable
    {
        [Header("Energy Bar Components")]
        [SerializeField] private Image[] _energyProgressBarImages;
        [SerializeField] private GameObject _energyBar;
        [SerializeField] private TextMeshProUGUI _energyText;

        [Header("Energy Bar Colors")]
        [SerializeField] private Color _energyBarEmptyColor;
        [SerializeField] private Color _energyBarPreviewColor;
        [SerializeField] private Color _energyBarFullColor;

        [Header("Low Energy Settings")]
        [SerializeField] private TextMeshProUGUI _lowEnergyText;
        [SerializeField] private Color _lowEnergyTextColor = Color.red; // Цвет текста при низком уровне энергии
        [SerializeField] private Color _defaultTextColor = Color.white; // Стандартный цвет текста
        [SerializeField] private float _lowEnergyThreshold = 10f; // Порог энергии для срабатывания
        [SerializeField] private float _lowEnergyBlinkDuration = 2f; // Длительность мигания текста
        [SerializeField] private int _lowEnergyBlinkCount = 2; // Длительность мигания текста

        private float _maxEnergy;
        private EnergyService _energyService;
        private PlayerFlightLaunchService _playerFlightLaunchService;
        private LevelDataService _levelDataService;
        private Tween _textBlinkTween;

        [Inject]
        public void Construct(EnergyService energyService, GameConfig gameConfig, PlayerFlightLaunchService playerFlightLaunchService, LevelDataService levelDataService)
        {
            _levelDataService = levelDataService;
            _playerFlightLaunchService = playerFlightLaunchService;
            _energyService = energyService;
            _maxEnergy = gameConfig.PlayerConfig.EnergyConfig.MaxEnergy;

            _energyService.OnEnergyChange += EnergyChangeHandler;
            _energyService.OnPreviewEnergyChange += PreviewEnergyChangeHandler;
            _playerFlightLaunchService.OnPlayerFlyStart += PlayerFlyStartHandler;
        }

        private void PlayerFlyStartHandler()
        {
            _lowEnergyText.gameObject.SetActive(false);

            if (_levelDataService.IsStartFromLevelZero == false)
                SetEnergyBarActive(true);
        }

        private void EnergyChangeHandler(float currentEnergy)
        {
            // 1. Рассчитываем количество активных сегментов
            int activeSegments = Mathf.CeilToInt((currentEnergy / _maxEnergy) * _energyProgressBarImages.Length);

            // 2. Проходим по всем сегментам СНИЗУ ВВЕРХ
            for (int i = 0; i < _energyProgressBarImages.Length; i++)
            {
                int index = _energyProgressBarImages.Length - 1 - i; // Обратный индекс

                if (i < activeSegments)
                {
                    // Если сегмент активен — устанавливаем "полный" цвет
                    _energyProgressBarImages[index].color = _energyBarFullColor;
                }
                else
                {
                    // Если сегмент "пустой" — устанавливаем прозрачный цвет
                    _energyProgressBarImages[index].color = _energyBarEmptyColor;
                }
            }

            // 3. Обновляем текстовое отображение энергии
            _energyText.text = Mathf.RoundToInt(currentEnergy).ToString();

            // 4. Проверяем уровень энергии и включаем мигание, если энергии недостаточно
            CheckLowEnergy(currentEnergy);
        }

        private void PreviewEnergyChangeHandler(float currentPreviewEnergy)
        {
            // 1. Рассчитываем количество ячеек, которые будут активны после затрат
            int activeSegments = Mathf.CeilToInt((_energyService.CurrentEnergy / _maxEnergy) * _energyProgressBarImages.Length);
            int previewSegments = Mathf.CeilToInt((currentPreviewEnergy / _maxEnergy) * _energyProgressBarImages.Length);

            // 2. Проходим по всем сегментам и меняем их состояние
            for (int i = 0; i < _energyProgressBarImages.Length; i++)
            {
                int index = _energyProgressBarImages.Length - 1 - i; // Обратный индекс

                if (i < previewSegments)
                {
                    // Ячейки, которые будут потрачены (предпросмотр) — выделяем другим цветом
                    _energyProgressBarImages[index].color = _energyBarPreviewColor;
                }
                else if (i < activeSegments)
                {
                    // Ячейки, которые активны в текущем состоянии — полные
                    _energyProgressBarImages[index].color = _energyBarFullColor;
                }
                else
                {
                    // Пустые ячейки — прозрачные
                    _energyProgressBarImages[index].color = _energyBarEmptyColor;
                }
            }
            
            _energyText.text = Mathf.RoundToInt(_energyService.CurrentEnergy - currentPreviewEnergy).ToString();
        }

        private void CheckLowEnergy(float currentEnergy)
        {
            if (currentEnergy < _lowEnergyThreshold)
            {
                if (_textBlinkTween == null || !_textBlinkTween.IsActive() || !_textBlinkTween.IsPlaying())
                {
                    _lowEnergyText.gameObject.SetActive(true);

                    // Уничтожаем предыдущий Tween, если он существует, чтобы избежать конфликта
                    _textBlinkTween?.Kill();

                    // Запускаем новый мигающий Tween
                    _textBlinkTween = _lowEnergyText.DOColor(_lowEnergyTextColor, _lowEnergyBlinkDuration / 2f)
                        .SetLoops(_lowEnergyBlinkCount * 2, LoopType.Yoyo)
                        .OnComplete(() =>
                        {
                            _lowEnergyText.color = _defaultTextColor;
                            _lowEnergyText.gameObject.SetActive(false);
                        });
                }
            }
            else
            {
                _textBlinkTween?.Kill(); 
                _textBlinkTween = null;
                _lowEnergyText.color = _defaultTextColor;
                _lowEnergyText.gameObject.SetActive(false);
            }
        }

        public void SetEnergyBarActive(bool isActive)
        {
            _energyBar.gameObject.SetActive(isActive);
        }

        public void Dispose()
        {
            _energyService.OnEnergyChange -= EnergyChangeHandler;
            _energyService.OnPreviewEnergyChange -= PreviewEnergyChangeHandler;
            _playerFlightLaunchService.OnPlayerFlyStart -= PlayerFlyStartHandler;

            // Очищаем Tween, чтобы избежать утечек памяти
            _textBlinkTween?.Kill();
        }

        public void Clear()
        {
            _textBlinkTween?.Kill(); // Безопасное уничтожение Tween
            _textBlinkTween = null;
        }
    }
}
