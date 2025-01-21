using System;
using Game.Scripts.Infrastructure;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.LevelElements.Triggers
{
    public class StartLevelGameplayTrigger : MonoBehaviour
    {
        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        
        public UnityEvent OnStartEvent;

        void Start() 
        {
            GlobalEventSystem.Broker.Receive<StartPlayLevelEvent>()
                .Subscribe(InvokeEvent)
                .AddTo(_disposable);
        }

        private void InvokeEvent(StartPlayLevelEvent levelEvent)
        {
            OnStartEvent?.Invoke();
        }
        private void OnDestroy()
        {
            OnStartEvent = null;
            _disposable?.Dispose();
        }
    }
}