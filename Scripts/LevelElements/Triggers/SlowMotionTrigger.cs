using Game.Scripts.Configs.Vfx;
using Game.Scripts.Constants;
using Game.Scripts.Infrastructure;
using UnityEngine;

namespace Game.Scripts.LevelElements.Triggers
{
    public class SlowMotionTrigger : MonoBehaviour
    {
        [SerializeField] private ESlowMotionType _slowMotionType;
        [SerializeField] private bool _isStart;
        
        private bool _isActivated;

        private void OnTriggerEnter(Collider other)
        {
            if(_isActivated)
                return;
            
            if (other.gameObject.layer == LayerMask.NameToLayer(LayersConstants.PLAYER_CAR) || 
                other.gameObject.layer == LayerMask.NameToLayer(LayersConstants.PLAYER_HUMANOID))
            {
                _isActivated = true;
                
                GlobalEventSystem.Broker.Publish(new PlayerEnterSlowMotionTriggerEvent() 
                { 
                    SlowMotionType = _slowMotionType, 
                    IsStart = _isStart
                });
            }
        }
    }
}