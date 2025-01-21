using System;
using Game.Scripts.Constants;
using Game.Scripts.Infrastructure;
using UniRx;
using UnityEngine;

namespace Game.Scripts.LevelElements.Triggers
{
    public class TrophyTrigger : MonoBehaviour
    {
        private bool _isActivated = false;
        
        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        private void Awake()
        {
            GlobalEventSystem.Broker.Receive<DisableTrophyOnLevelEvent>()
                .Subscribe(DisableTrophyOnLevelEventHandler)
                .AddTo(_disposable);
        }

        private void DisableTrophyOnLevelEventHandler(DisableTrophyOnLevelEvent disableEvent)
        {
            gameObject.SetActive(false);
        }
        private void OnTriggerEnter(Collider other)
        {
            if(_isActivated)
                return;
            if (other.gameObject.layer == LayerMask.NameToLayer(LayersConstants.PLAYER_CAR) || other.gameObject.layer == LayerMask.NameToLayer(LayersConstants.PLAYER_HUMANOID))
            {
                _isActivated = true;
                GlobalEventSystem.Broker.Publish(new PlayerCollectTrophyEvent() {  });
                gameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            _disposable?.Dispose();
        }
    }
}