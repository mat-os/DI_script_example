using Game.Scripts.Constants;
using Game.Scripts.Infrastructure;
using UnityEngine;

namespace Game.Scripts.LevelElements.Collisions
{
    public class TargetHitView : MonoBehaviour
    {
        [field:SerializeField] public Transform TargetCenter; // Центр мишени
        [field:SerializeField] public float InnerZoneRadius = 1.0f;   // Радиус центральной зоны
        [field:SerializeField] public float MiddleZoneRadius = 2.0f;  // Радиус средней зоны
        [field:SerializeField] public float OuterZoneRadius = 3.0f;   // Радиус внешней зоны

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer(LayersConstants.PLAYER_HUMANOID))
            {
                Vector3 hitPosition = collision.GetContact(0).point;
                GlobalEventSystem.Broker.Publish(new TargetHitEvent { TargetHitView = this, HitPosition = hitPosition });
            }
        }
        
        // Рисуем зоны мишени через Gizmos
        private void OnDrawGizmos()
        {
            if (TargetCenter == null) return;

            // Учитываем локальный скейл объекта мишени
            float scaleFactor = transform.lossyScale.x;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(TargetCenter.position, InnerZoneRadius * scaleFactor);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(TargetCenter.position, MiddleZoneRadius * scaleFactor);

            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(TargetCenter.position, OuterZoneRadius * scaleFactor);
        }
    }
}
