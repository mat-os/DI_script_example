using System;
using System.Globalization;
using Configs;
using DG.Tweening;
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
    public class DamageTextUI : MonoBehaviour, IDisposable
    {
        [SerializeField] private TextMeshProUGUI _playerDamage;
        [SerializeField] private TextMeshProUGUI _playerFlyDistance;
    
        private PlayerDamageService _playerDamageService;
        private PlayerFlightTrackerService _playerFlightTrackerService;
        
        private float _currentDamage;
        
        [Inject]
        public void Construct(PlayerDamageService playerDamageService, 
            PlayerFlightTrackerService playerFlightTrackerService)
        {
            _playerFlightTrackerService = playerFlightTrackerService;
            _playerDamageService = playerDamageService;
            
            _playerDamageService.OnChangeTotalDamage += ChangeTotalDamageHandler;
            _playerFlightTrackerService.OnFlyDistanceChange += ChangeFlyDistance;
        }

        public void ChangeFlyDistance(float distance)
        {
            _playerFlyDistance.text = Mathf.RoundToInt(distance).ToString();
        }

        private void ChangeTotalDamageHandler(float newTotalDamage)
        {
            DOTween.Kill(_playerDamage); 
            DOTween.To(() => _currentDamage, x => UpdateDamageText(x), newTotalDamage, 1.0f)
                .SetEase(Ease.OutCubic); 
        }

        public void UpdateDamageText(float value)
        {
            _currentDamage = value;
            _playerDamage.text = _currentDamage.ToString();
        }

        public void Dispose()
        {
            _playerDamageService.OnChangeTotalDamage -= ChangeTotalDamageHandler;
            _playerFlightTrackerService.OnFlyDistanceChange -= ChangeFlyDistance;
        }
    }
}