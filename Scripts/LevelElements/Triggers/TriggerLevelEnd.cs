using DG.Tweening;
using Game.Scripts.Infrastructure;
using UnityEngine;

namespace Game.Scripts.LevelElements.Triggers
{
    public class TriggerLevelEnd : MonoBehaviour
    {
        public float Delay;
        
        private bool _isTriggered = false;
        public void LevelEndTrigger()
        {
            if (_isTriggered == false)
            {
                _isTriggered = true;
                DOVirtual.DelayedCall(Delay, Callback);
            }
        }

        private void Callback()
        {
            GlobalEventSystem.Broker.Publish(new PlayerEnterLevelCompleteTrigger {  });

        }
    }
}