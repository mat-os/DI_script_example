using System;
using Configs;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Game.Scripts.Configs;
using Game.Scripts.Infrastructure.Services;
using Game.Scripts.Infrastructure.Services.Player;
using Game.Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

// Предполагаю, что ты используешь TextMeshPro, если нет, используй UnityEngine.UI для обычного текста.

namespace Game.Scripts.UI.Widgets
{
    public class ScoreCounterUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _comboText;
        [SerializeField] private Image _comboFill;
        [SerializeField] private AnimationParameterConfig _scoreAnimation;
        
        private PlayerComboService _playerComboService;
        private PlayerScoreService _playerScoreService;
        private ScoreConfig _scoreConfig;
        
        private float _currentScore;
        private TweenerCore<float, float, FloatOptions> _updateScore;
        private TweenerCore<Vector3, Vector3, VectorOptions> _score;

        [Inject]
        public void Construct(PlayerScoreService playerScoreService, PlayerComboService playerComboService, GameConfig gameConfig)
        {
            _playerScoreService = playerScoreService;
            _playerComboService = playerComboService;
            _scoreConfig = gameConfig.ScoreConfig;

            Clear();
        }

        public void Clear()
        {
            _comboFill.fillAmount = 0;
            _scoreText.text = 0.ToString();
            _comboText.text = "X" + 1;
        }

        public void Subscribe()
        {
            _playerScoreService.OnScoreChanged += ScoreChangedHandler;
            _playerComboService.OnComboChange += ComboChangeHandler;
        }       
        private void ComboChangeHandler(int combo)
        {
            _comboText.text = "X" + combo;
            _comboText.transform.DOScale(Vector3.one * 1.2f, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
            {
                _comboText.transform.DOScale(Vector3.one, 0.2f);
            });
        }
        private void ScoreChangedHandler(int newScore)
        {
            DOTween.Kill(_updateScore); 
            DOTween.Kill(_score); 
            
            _updateScore = DOTween.To(() => _currentScore, UpdateScoreText, newScore, _scoreAnimation.Duration)
                .SetEase(_scoreAnimation.Ease);
            _score = _scoreText.transform.DOScale(Vector3.one * 1.2f, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
            {
                _scoreText.transform.DOScale(Vector3.one, 0.2f);
            });
            
            UpdateComboFill(newScore);
        }
        private void UpdateScoreText(float score)
        {
            _scoreText.text = score.ToString("N0");
            _currentScore = score;
        }

        private void UpdateComboFill(int currentScore)
        {
            if (_playerComboService.CurrentComboLevel >= _scoreConfig.ComboSettings.Count)
            {
                // Если комбо достигло максимального уровня, заполняем шкалу до конца
                _comboFill.fillAmount = 1f;
                _comboFill.color = _scoreConfig.ComboSettings[^1].ComboUIColor.Evaluate(1f); // Последний уровень
            }
            else
            {
                // Иначе обновляем шкалу на основе текущего прогресса
                float comboProgress = _playerComboService.GetComboProgress(currentScore);
                _comboFill.fillAmount = 1 - comboProgress;
                _comboFill.color = _scoreConfig.ComboSettings[_playerComboService.CurrentComboLevel].ComboUIColor.Evaluate(comboProgress);
            }
        }
        public void Unsubscribe()
        {
            _playerScoreService.OnScoreChanged -= ScoreChangedHandler;
            _playerComboService.OnComboChange -= ComboChangeHandler;
        }
    }
}