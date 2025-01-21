using System;
using Game.Scripts.Constants;
using Game.Scripts.Infrastructure;
using Game.Scripts.Infrastructure.Services.Tutorial;
using UnityEngine;

namespace Game.Scripts.LevelElements.Tutorial
{
    public class TutorialTrigger : MonoBehaviour
    {
        [SerializeField] private ETutorialStep _tutorialStep;
        private bool _isActivated;

        private void OnTriggerEnter(Collider other)
        {
            if(_isActivated)
                return;
            
            if (other.gameObject.layer == LayerMask.NameToLayer(LayersConstants.PLAYER_CAR) || other.gameObject.layer == LayerMask.NameToLayer(LayersConstants.PLAYER_HUMANOID))
            {
                _isActivated = true;
                GlobalEventSystem.Broker.Publish(new TriggerTutorialEvent { TutorialStep = _tutorialStep });
                //gameObject.SetActive(false);
            }
        }
        private void Start()
        {
            if (_tutorialStep == ETutorialStep.SwipeToJump)
            {
                GlobalEventSystem.Broker.Publish(new PlayerEnterLevelWithJumpTutorialTrigger {  });
            }
        }
    }
}