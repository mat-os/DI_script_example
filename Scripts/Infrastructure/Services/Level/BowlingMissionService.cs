using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Scripts.Configs;
using Game.Scripts.LevelElements.Collisions;
using Game.Scripts.Utils;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Infrastructure.Services.Level
{
    public class BowlingMissionService : IDisposable
    {
        public Action<int> OnPinFall;
        
        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        
        private int _pinsFall = 0;
        
        [Inject]
        public BowlingMissionService()
        {
            GlobalEventSystem.Broker.Receive<BowlingPinFallEvent>()
                .Subscribe(BowlingPinFallHandle)
                .AddTo(_disposable);
        }

        private void BowlingPinFallHandle(BowlingPinFallEvent bowling)
        {
            _pinsFall++;
            Debug.Log("BowlingPinFallHandle " + _pinsFall);
            OnPinFall?.Invoke(_pinsFall);
        }

        public void Clear()
        {
            _pinsFall = 0;
        }
        public void Dispose()
        {
            OnPinFall = null;
            _disposable.Dispose();
        }
    }
}