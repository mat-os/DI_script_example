using Game.Scripts.Configs.Vfx;
using Game.Scripts.Constants;
using Game.Scripts.Infrastructure;
using UnityEngine;

namespace Game.Scripts.LevelElements.Collisions
{
    public class VfxTrigger : MonoBehaviour
    {
        [SerializeField] private VfxEffectType _effectType = VfxEffectType.Explosion; // Тип эффекта взрыва
        [SerializeField] private Transform _effectSpawnPosition; // Тип эффекта взрыва
        
        private bool _isVfxPlayed = false;
        private void OnCollisionEnter(Collision collision)
        {
            if(_isVfxPlayed)
                return;
            if (collision.gameObject.layer == LayerMask.NameToLayer(LayersConstants.PLAYER_CAR) || 
                collision.gameObject.layer == LayerMask.NameToLayer(LayersConstants.PLAYER_HUMANOID))
            {
                TriggerVfx();
            }
        }

        private void TriggerVfx()
        {
            GlobalEventSystem.Broker.Publish(new PlayVfxEvent { Position = _effectSpawnPosition.position, VfxEffectType = _effectType});
            _isVfxPlayed = true;
        }
    }
}