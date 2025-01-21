using System;
using Configs;
using Game.Scripts.Configs;
using Game.Scripts.Core.Update;
using Game.Scripts.Infrastructure.LevelStateMachin;
using Game.Scripts.Infrastructure.Services;
using Game.Scripts.LevelElements.Player;
using PG;
using UniRx;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Game.Scripts.Infrastructure.States
{
    public class PlayerService : IDisposable
    {
        public Action<PlayerHumanoid> OnPlayerHumanoidCreated;
        
        private readonly GameConfig _gameConfig;
        private readonly PrefabRepository _prefabRepository;
        
        private PlayerView _playerView;

        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        private PlayerHumanoid _playerHumanoid;

        [Inject]
        public PlayerService(GameConfig gameConfig,
            PrefabRepository prefabRepository)
        {
            _prefabRepository = prefabRepository;
            _gameConfig = gameConfig;
            
            GlobalEventSystem.Broker.Receive<PlayerExplodeEvent>()
                .Subscribe(PlayerExplodeHandle)
                .AddTo(_disposable);
        }

        public PlayerView CreatePlayer(Transform playerRoot, Transform playerConstraintTarget)
        {
            _playerView = Object.Instantiate(_prefabRepository.PlayerView, playerRoot);
            _playerHumanoid = new PlayerHumanoid(_playerView, _gameConfig.PlayerConfig, playerConstraintTarget);
            OnPlayerHumanoidCreated?.Invoke(_playerHumanoid);
            return _playerView;
        }

        private void PlayerExplodeHandle(PlayerExplodeEvent explodeEvent)
        {
            _playerHumanoid.AddExplosionForce(explodeEvent.ExplodePosition, explodeEvent.ExplosionForce);
        }

        private void SubscribeEvents()
        {
        }
        private void UnsubscribeEvents()
        {
        }

        public void Clear()
        {
            _playerView = null;
            _playerHumanoid = null;
        }

        public void Dispose()
        {
            _disposable?.Dispose();
            
            UnsubscribeEvents();
        }
    }
}