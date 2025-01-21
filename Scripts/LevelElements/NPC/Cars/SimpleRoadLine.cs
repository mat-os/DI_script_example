using System;
using System.Collections.Generic;
using Game.Scripts.Infrastructure;
using UniRx;
using UnityEngine;

namespace Game.Scripts.LevelElements.NPC.Cars
{
    public class SimpleRoadLine : MonoBehaviour
    {
        public enum MovementType
        {
            Fixed,
            Loop
        }

        [SerializeField] private bool _isStartWithLevelPlayEvent;
        
        [Header("Road Settings")]
        [SerializeField] private Transform _startPoint; // Начальная точка дороги
        [SerializeField] private Transform _endPoint; // Конечная точка дороги
        [SerializeField] private MovementType _movementType = MovementType.Fixed; // Тип движения
        [SerializeField] private List<SimpleCarMover> _carsOnRoad = new List<SimpleCarMover>(); // Список машин на дороге
        
        private readonly CompositeDisposable _disposable = new CompositeDisposable();


        private void Awake()
        {
            if (_isStartWithLevelPlayEvent)
            {
                GlobalEventSystem.Broker.Receive<StartPlayLevelEvent>()
                    .Subscribe(StartCars)
                    .AddTo(_disposable);
            }
            else
            {
                StartCars(null);
            }
        }

        private void OnDestroy()
        {
            _disposable?.Dispose();
        }
        
        // Метод для запуска движения всех машин
        public void StartCars(StartPlayLevelEvent startPlayLevelEvent)
        {
            foreach (var car in _carsOnRoad)
            {
                if (car != null)
                {
                    car.StartMovement(_startPoint.position, _endPoint.position, _movementType);
                }
            }
        }

        // Gizmos для отображения начальной и конечной точек, а также линии между ними
        private void OnDrawGizmos()
        {
            if (_startPoint != null && _endPoint != null)
            {
                // Рисуем линию от начала до конца
                Gizmos.color = Color.green;
                Gizmos.DrawLine(_startPoint.position, _endPoint.position);

                // Рисуем точки
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(_startPoint.position, 0.3f); // Начальная точка
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(_endPoint.position, 0.3f);   // Конечная точка
            }
        }
    }
}
