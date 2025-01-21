using System;
using Configs;
using Game.Scripts.Core.Update.Interfaces;
using Game.Scripts.Infrastructure.Services.Player;
using Game.Scripts.Infrastructure.States;
using Game.Scripts.LevelElements.Player;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Infrastructure.Services.Player
{
    public class PlayerFlightLineDirectionService : IDisposable
    {
        private readonly PlayerSwipeService _swipeService;
        private readonly PlayerService _playerService;
        private readonly PlayerConfig _playerConfig;

        private PlayerView _playerView;
        private LineRenderer _lineRenderer;

        [Inject]
        public PlayerFlightLineDirectionService(
            PlayerSwipeService swipeService,
            PlayerService playerService,
            GameConfig gameConfig)
        {
            _swipeService = swipeService;
            _playerService = playerService;
            _playerConfig = gameConfig.PlayerConfig;

            _playerService.OnPlayerHumanoidCreated += PlayerCreatedHandler;
        }

        private void PlayerCreatedHandler(PlayerHumanoid playerHumanoid)
        {
            _playerView = playerHumanoid.PlayerView;
            _lineRenderer = _playerView.DirectionLineRenderer;
        }

        public void OnSwipeStart()
        {
            ResetLineRenderer(); // Сбрасываем линию перед началом свайпа
        }

        public void OnSwipeUpdate(Vector3 dragDelta)
        {
            if (!_swipeService.IsSwipeValid())
            {
                _lineRenderer.enabled = false;
                return;
            }

            UpdateLineRenderer(dragDelta);
        }

        private void UpdateLineRenderer(Vector3 dragDelta)
        {
            _lineRenderer.enabled = true;
            _lineRenderer.positionCount = 2;

            // Начальная позиция линии совпадает с позицией игрока
            _lineRenderer.SetPosition(0, _playerView.RigidbodyRoot.transform.position);

            // Ограничиваем длину линии на основе нормализованного свайпа
            float lineLength = Mathf.Lerp(
                _playerConfig.LineConfig.MinDragLineDistance,
                _playerConfig.LineConfig.MaxDragLineDistance,
                _swipeService.NormalizedDrag);

            // Вычисляем конечную точку линии
            Vector3 direction = dragDelta.normalized; // Нормализованное направление
            direction.x = -direction.x;
            Vector3 endPoint = _playerView.RigidbodyRoot.position + direction * lineLength;

            // Устанавливаем конечную точку линии
            _lineRenderer.SetPosition(1, endPoint);
        }
        public void OnSwipeEnd()
        {
            ResetLineRenderer();
        }
        private void ResetLineRenderer()
        {
            if (_lineRenderer != null)
            {
                _lineRenderer.SetPosition(0, Vector3.zero);
                _lineRenderer.SetPosition(1, Vector3.zero);
                _lineRenderer.enabled = false;
            }
        }

        public void Dispose()
        {
            _playerService.OnPlayerHumanoidCreated -= PlayerCreatedHandler;
        }

        public void Clear()
        {
            
        }


    }
}
