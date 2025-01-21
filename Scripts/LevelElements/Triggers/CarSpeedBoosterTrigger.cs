using Game.Scripts.Constants;
using Game.Scripts.Infrastructure;
using UnityEngine;

namespace Game.Scripts.LevelElements.Triggers
{
    public class CarSpeedBoosterTrigger : MonoBehaviour
    {
        private bool _isActivated = false;

        private void OnTriggerEnter(Collider other)
        {
            if(_isActivated)
                return;
            if (other.gameObject.layer == LayerMask.NameToLayer(LayersConstants.PLAYER_CAR))
            {
                _isActivated = true;
                GlobalEventSystem.Broker.Publish(new PlayerCarEnterBoosterZoneEvent() {  });
            }
        }
        private void OnDrawGizmos()
        {
            // Access the BoxCollider component
            BoxCollider boxCollider = GetComponent<BoxCollider>();

            if (boxCollider != null)
            {
                // Set the color for the Gizmo
                Gizmos.color = Color.red;

                // Calculate the box center in world space
                Vector3 boxCenter = transform.TransformPoint(boxCollider.center);

                // Draw a wireframe box with the BoxCollider's center and size
                Gizmos.DrawWireCube(boxCenter, boxCollider.size);
            }
        }
    }
}