using System;
using System.Collections.Generic;
using System.Linq;
using Configs;
using Game.Scripts.Infrastructure;
using Game.Scripts.Infrastructure.Services;
using Game.Scripts.Infrastructure.Services.Player;
using Game.Scripts.LevelElements.Player;
using RotaryHeart.Lib.SerializableDictionary;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.Widgets
{
    public class BoneDamageUI: MonoBehaviour, IDisposable
    {
        [SerializeField] private SerializableDictionaryBase<EExtendedMuscleGroup, Image> _muscleImageMap;
        [SerializeField]private Gradient _damageGradient;
        [SerializeField]private TMP_Text _damageText;
        
        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        private PlayerConfig _playerConfig;
        
        private PlayerDamageService _playerDamageService;

        [Inject]
        public void Construct(GameConfig gameConfig, PlayerDamageService playerDamageService)
        {
            _playerDamageService = playerDamageService;
            _playerConfig = gameConfig.PlayerConfig;

            _playerDamageService.OnChangeTotalDamage += OnChangeTotalDamageHandler;
            
            GlobalEventSystem.Broker.Receive<PlayerBodyPartTakeDamageEvent>()
                .Subscribe(PlayerBodyPartTakeDamageHandler)
                .AddTo(_disposable);
        }

        private void OnChangeTotalDamageHandler(float totalDamage)
        {
            var damagePercent = totalDamage / _playerDamageService.GetMaxPossibleDamage() ;
            _damageText.text =  Mathf.RoundToInt(damagePercent * 100f) + "%";
        }

        private void PlayerBodyPartTakeDamageHandler(PlayerBodyPartTakeDamageEvent damageEventEvent)
        {
            if (_muscleImageMap.TryGetValue(damageEventEvent.EExtendedMuscleGroup, out var boneImage))
            {
                var thresholds = _playerConfig.DamageConfig.DamageThresholds[damageEventEvent.MuscleGroup];
                float damagePercentage = Mathf.Clamp01((float)damageEventEvent.TotalDamageOnBodyPart / thresholds);
                Color boneColor = _damageGradient.Evaluate(Mathf.Clamp01(damagePercentage));
                boneImage.color = boneColor;
            }
        }
        public void Reset()
        {
            foreach (var muscleImage in _muscleImageMap.Values)
            {
                muscleImage.color = Color.white;
            }

            _damageText.text = "0%";
        }

        public void Dispose()
        {
            _disposable.Dispose();
            _playerDamageService.OnChangeTotalDamage -= OnChangeTotalDamageHandler;
        }
    }
}