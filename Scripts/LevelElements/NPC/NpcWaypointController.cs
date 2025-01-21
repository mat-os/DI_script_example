using SWS;
using UnityEngine;

namespace Game.Scripts.LevelElements.NPC
{
    [RequireComponent(typeof(splineMove))]
    public class NpcWaypointController : MonoBehaviour
    {
        [Header("Waypoint Path")] 
        public PathManager PathManager;

        [Header("Speed Settings")]
        public float MinSpeed = 2f;
        public float MaxSpeed = 5f;

        private splineMove _splineMoveComponent;

        void Start()
        {
            // Получаем компонент splineMove (Simple Waypoint System)
            _splineMoveComponent = GetComponent<splineMove>();

            if (_splineMoveComponent != null && PathManager != null)
            {
                // Привязываем путь к машине
                _splineMoveComponent.SetPath(PathManager);

                // Устанавливаем случайную скорость
                float randomSpeed = Random.Range(MinSpeed, MaxSpeed);
                _splineMoveComponent.speed = randomSpeed;
                
                FindClosestWaypoint();
                
                // Запускаем движение
                _splineMoveComponent.StartMove();
            }
            else
            {
                Debug.LogError("PathManager или splineMove не присвоены для объекта: " + gameObject.name);
            }
        }
        private void FindClosestWaypoint()
        {
            // Находим ближайшую точку на пути
            float nearestDistance = Mathf.Infinity;
            int closestWaypointIndex = 0;

            for (int i = 0; i < PathManager.waypoints.Length; i++)
            {
                float distance = Vector3.Distance(transform.position, PathManager.waypoints[i].position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    closestWaypointIndex = i;
                }
            }

            // Устанавливаем ближайшую точку в качестве начальной
            _splineMoveComponent.startPoint = closestWaypointIndex;
        }

    }
}