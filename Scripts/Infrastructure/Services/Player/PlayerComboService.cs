using System;
using Configs;
using Game.Scripts.Configs;
using Game.Scripts.Utils.Debug;
using UnityEngine;

namespace Game.Scripts.Infrastructure.Services.Player
{
    public class PlayerComboService
    {
        public Action<int> OnComboChange;
        
        private int _currentComboLevel = 1;
        public int CurrentComboLevel => _currentComboLevel;
        
        private readonly ScoreConfig _scoreConfig;

        public PlayerComboService(GameConfig gameConfig)
        {
            _scoreConfig = gameConfig.ScoreConfig;
        }
        public void UpdateComboLevel(int currentScore)
        {
            while (_currentComboLevel < _scoreConfig.ComboSettings.Count &&
                   currentScore >= _scoreConfig.ComboSettings[_currentComboLevel - 1].ComboThreshold)
            {
                _currentComboLevel++;
                OnComboChange?.Invoke(_currentComboLevel);
                CustomDebugLog.Log($"[Combo] New Combo Level: {_currentComboLevel}");
            }
        }
        // Получаем прогресс текущего уровня комбо как значение от 0 до 1
        public float GetComboProgress(int currentScore)
        {
            if (_currentComboLevel >= _scoreConfig.ComboSettings.Count)
                return 1f; // Максимальный уровень комбо

            int currentThreshold = _scoreConfig.ComboSettings[_currentComboLevel - 1].ComboThreshold;
            int nextThreshold = _scoreConfig.ComboSettings[_currentComboLevel].ComboThreshold;

            return Mathf.Abs((float)(currentScore - currentThreshold) / (nextThreshold - currentThreshold));
        }
        
        public void Clear()
        {
            _currentComboLevel = 1;
            OnComboChange?.Invoke(_currentComboLevel);
        }
    }
}