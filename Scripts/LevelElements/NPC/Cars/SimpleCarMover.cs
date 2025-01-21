using UnityEngine;

namespace Game.Scripts.LevelElements.NPC.Cars
{
    public class SimpleCarMover : MonoBehaviour
    {
        [Header("Car Settings")]
        [SerializeField] private float _speed = 5f; 
        
        private Vector3 _startPosition; // Начальная точка
        private Vector3 _endPosition; // Конечная точка
        private bool _isMoving = false; // Флаг движения
        private SimpleRoadLine.MovementType _movementType;

        private Vector3 _lastPosition; // Позиция в предыдущем кадре
        private Vector3 _currentPosition; // Позиция, вычисленная в FixedUpdate

        public void StartMovement(Vector3 startPosition, Vector3 endPosition, SimpleRoadLine.MovementType movementType)
        {
            _startPosition = startPosition;
            _endPosition = endPosition;
            _movementType = movementType;

            _currentPosition = transform.position;
            _lastPosition = _currentPosition;
            _isMoving = true;
        }

        void FixedUpdate()
        {
            if (_isMoving)
            {
                // Двигаем машину к целевой точке
                _currentPosition = Vector3.MoveTowards(transform.position, _endPosition, _speed * Time.fixedDeltaTime);

                // Проверяем, достигла ли машина конца
                if (Vector3.Distance(_currentPosition, _endPosition) < 0.1f)
                {
                    if (_movementType == SimpleRoadLine.MovementType.Loop)
                    {
                        // Возвращаем машину в начало пути
                        //transform.position = _startPosition;
                        _currentPosition = _startPosition;
                    }
                    else
                    {
                        // Выключаем машину
                        _isMoving = false;
                        gameObject.SetActive(false);
                    }
                }

                // Обновляем позицию для физики
                //transform.position = _currentPosition;
            }
        }

        void Update()
        {
            if (_isMoving)
            {
                // Интерполяция для сглаживания движения
                transform.position = Vector3.Lerp(_lastPosition, _currentPosition, Time.deltaTime / Time.fixedDeltaTime);
            }

            _lastPosition = transform.position;
        }
    }
}
