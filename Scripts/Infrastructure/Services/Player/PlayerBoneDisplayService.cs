using System;
using System.Collections.Generic;
using Configs;
using DG.Tweening;
using Game.Scripts.Infrastructure.States;
using Game.Scripts.LevelElements.Player;
using UnityEngine;

namespace Game.Scripts.Infrastructure.Services.Player
{
    public class PlayerBoneDisplayService : IDisposable
    {
        private readonly Dictionary<EExtendedMuscleGroup, PlayerBoneView> _boneViews = new();
        private readonly Dictionary<EExtendedMuscleGroup, bool> _brokenBones = new();
        private readonly Dictionary<EExtendedMuscleGroup, bool> _isBlinking = new(); 
        private readonly Dictionary<EExtendedMuscleGroup, float> _bonesDamagePersent = new(); 

        private readonly PlayerBonesConfig _bonesConfig;
        private PlayerService _playerService;
        private DamageTextEffectService _damageTextEffectService;
        private PlayerFlightLandingService _playerFlightLandingService;

        public PlayerBoneDisplayService(GameConfig gameConfig, PlayerService playerService, DamageTextEffectService damageTextEffectService, PlayerFlightLandingService playerFlightLandingService)
        {
            _playerFlightLandingService = playerFlightLandingService;
            _damageTextEffectService = damageTextEffectService;
            _playerService = playerService;
            _bonesConfig = gameConfig.PlayerConfig.BonesConfig;
            
            _playerService.OnPlayerHumanoidCreated += PlayerCreatedHandler;
            _playerFlightLandingService.OnPlayerFlyComplete += OnPlayerFlyCompleteHandler;
        }

        private void OnPlayerFlyCompleteHandler()
        {
            DisableAllBoneDisplays();
        }

        private void PlayerCreatedHandler(PlayerHumanoid playerHumanoid)
        {
            SetPlayer(playerHumanoid.PlayerView);
        }

        public void SetPlayer(PlayerView playerViews)
        {
            _boneViews.Clear();
            _brokenBones.Clear();
            _isBlinking.Clear();
            _bonesDamagePersent.Clear();
            
            foreach (var boneView in playerViews.PlayerBones)
            {
                _boneViews[boneView.ExtendedMuscleGroup] = boneView;
                boneView.gameObject.SetActive(false); // Изначально кости не видны
            }
        }

        public void UpdateBoneDisplay(EExtendedMuscleGroup muscleGroup, float damagePercentage)
        {
            if (!_boneViews.TryGetValue(muscleGroup, out var boneView))
                return;

            if (damagePercentage < _bonesConfig.MinDamagePercentageToShowBone)
                return;

            _bonesDamagePersent[muscleGroup] = damagePercentage;

            // Если кость мигает, не обновляем цвет
            if (_isBlinking.TryGetValue(muscleGroup, out bool isBlinking) && isBlinking)
                return;

            if (!_brokenBones.ContainsKey(muscleGroup))
            {
                _brokenBones[muscleGroup] = true;
                StartBoneBlinking(muscleGroup, boneView); // Старт мигания при первом обнаружении сломанной кости
                
                GlobalEventSystem.Broker.Publish(new BrokeBoneEvent() { TotalBrokenBones = _brokenBones.Count});
            }
            
            var color = _bonesConfig.BoneColorGradient.Evaluate(damagePercentage);
            boneView.Renderer.material.color = color;

            boneView.gameObject.SetActive(true);
            
            _damageTextEffectService.CreateDamageTextEffect(boneView.RendererCenter, (int)(damagePercentage * 100));
        }

        private void StartBoneBlinking(EExtendedMuscleGroup muscleGroup, PlayerBoneView boneView)
        {
            // Добавляем в список мигающих
            _isBlinking[muscleGroup] = true;

            var initialColor = _bonesConfig.BlinkStartColor;
            var initialShadingColor = _bonesConfig.BlinkShadingStartColor;
            var blinkEndColor = _bonesConfig.BlinkEndColor;
            var blinkShadingEndColor = _bonesConfig.BlinkShadingEndColor;
            var blinkDuration = _bonesConfig.BlinkDuration;

            // Создаем последовательность миганий
            Sequence blinkSequence = DOTween.Sequence()
                .Append(boneView.Renderer.material.DOColor(blinkEndColor, "_Color",blinkDuration / 2f))
                .Insert(0,boneView.Renderer.material.DOColor(blinkShadingEndColor, "_ShadingColor",blinkDuration / 2f))
                .Append(boneView.Renderer.material.DOColor(initialColor, "_Color",blinkDuration / 2f))
                .Insert(blinkDuration / 2f,boneView.Renderer.material.DOColor(initialShadingColor, "_ShadingColor",blinkDuration / 2f))
                .SetLoops(_bonesConfig.BlinkCount, LoopType.Restart)
                .OnComplete(() =>
                {
                    // После завершения мигания возвращаем управление цветом
                    _isBlinking[muscleGroup] = false;

                    // Устанавливаем цвет, соответствующий текущему damagePercentage
                    var finalColor = _bonesConfig.BoneColorGradient.Evaluate(_bonesDamagePersent[muscleGroup]); // Поломка 100%
                    boneView.Renderer.material.color = finalColor;
                });

            // Опционально: привязываем к объекту, чтобы отменить анимацию при уничтожении
            blinkSequence.SetTarget(boneView.gameObject);
        }

        public void DisableAllBoneDisplays()
        {
            foreach (var bonePair in _boneViews)
            {
                var boneView = bonePair.Value;

                // Завершить все твины, связанные с объектом кости
                DOTween.Kill(boneView.gameObject);

                // Убедиться, что кость выключена
                boneView.gameObject.SetActive(false);
            }

            // Очищаем состояния
            _brokenBones.Clear();
            _isBlinking.Clear();
            _bonesDamagePersent.Clear();
        }

        public void Dispose()
        {
            _playerService.OnPlayerHumanoidCreated -= PlayerCreatedHandler;
        }
    }
}