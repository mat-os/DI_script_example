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
    public class DestroyCarMissionService : IDisposable
    {
        public Action<int> OnCarDestroyed;
        
        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        
        private int _destroyedCar = 0;
        
        [Inject]
        public DestroyCarMissionService()
        {
            GlobalEventSystem.Broker.Receive<CarNPCDestroyedEvent>()
                .Subscribe(CarNpcDestroyedHandler)
                .AddTo(_disposable);
        }

        private void CarNpcDestroyedHandler(CarNPCDestroyedEvent carNpcDestroyedEvent)
        {
            _destroyedCar++;
            Debug.Log("CarNpcDestroyedHandler " + _destroyedCar);
            OnCarDestroyed?.Invoke(_destroyedCar);
        }

        public void Clear()
        {
            _destroyedCar = 0;
        }
        public void Dispose()
        {
            OnCarDestroyed = null;
            _disposable.Dispose();
        }
    }
}