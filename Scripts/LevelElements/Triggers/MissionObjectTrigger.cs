using Game.Scripts.Infrastructure;
using UnityEngine;

namespace Game.Scripts.LevelElements.Triggers
{
    public class MissionObjectTrigger : MonoBehaviour
    {
        [SerializeField]private bool _isCountForMission = true;

        public void TriggerMission()
        {
            if (_isCountForMission)
            {
                GlobalEventSystem.Broker.Publish(new MissionObjectDestroyEvent { HitPosition = transform.position });
            }
        }
    }
}