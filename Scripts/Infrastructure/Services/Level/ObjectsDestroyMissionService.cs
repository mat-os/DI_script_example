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
    public class ObjectsDestroyMissionService : IDisposable
    {
        public Action<int> OnObjectDestroy;
        
        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        
        private int _objectsDestroyed = 0;
        
        [Inject]
        public ObjectsDestroyMissionService()
        {
            GlobalEventSystem.Broker.Receive<MissionObjectDestroyEvent>()
                .Subscribe(ObjectDestroyHandle)
                .AddTo(_disposable);
        }

        private void ObjectDestroyHandle(MissionObjectDestroyEvent missionObjectDestroyEvent)
        {
            _objectsDestroyed++;
            Debug.Log("ObjectDestroyHandle " + _objectsDestroyed);
            OnObjectDestroy?.Invoke(_objectsDestroyed);
        }

        public void Clear()
        {
            _objectsDestroyed = 0;
        }
        public void Dispose()
        {
            OnObjectDestroy = null;
            _disposable.Dispose();
        }
    }
}