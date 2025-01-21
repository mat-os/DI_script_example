using Game.Scripts.Configs.Player;
using Game.Scripts.LevelElements.Car.CarMovement;
using UnityEngine;

namespace Game.Scripts.LevelElements.Car
{
    public class CarWheelController
    {
        private readonly CarView _carView;
        private readonly CarMovementConfig _carMovementConfig;
        private readonly ICarMovement _carMovement;
        private float _wheelRotationSpeed;

        public CarWheelController(CarView carView, CarMovementConfig carMovementConfig, ICarMovement carMovement)
        {
            _carView = carView;
            _carMovementConfig = carMovementConfig;
            _carMovement = carMovement;

            _wheelRotationSpeed = _carMovementConfig.WheelRotationSpeed / _carView.Wheels[0].SphereCollider.radius * Mathf.Rad2Deg;
        }

        /// <summary>
        /// Обновляет вращение колес.
        /// </summary>
        public void RotateWheels(float deltaTime)
        {
            float carSpeed = _carMovement.GetSpeed();
            float wheelRotationSpeed = carSpeed * _wheelRotationSpeed;
            for (int i = 0; i < _carView.Wheels.Length; i++)
            {
                Transform wheelTransform = _carView.Wheels[i].WheelRoot;

                // Вращение колеса по оси X (вращение колес в зависимости от скорости)
                Quaternion rotationX = Quaternion.Euler(wheelRotationSpeed * deltaTime, 0, 0);

                // Поворот передних колес по оси Y (угол руля)
                Quaternion rotationY = Quaternion.identity;
                /*if (_carView.Wheels[i].IsSteering)
                {
                    float targetSteerAngle = _carMovement.GetCurrentSteerInput() * _carMovementConfig.WheelMaxSteerAngle;
                    Quaternion targetRotationY = Quaternion.Euler(0, targetSteerAngle, 0);

                    // Плавная интерполяция текущего угла колеса к целевому углу
                    rotationY = Quaternion.Lerp(
                        wheelTransform.localRotation,
                        targetRotationY,
                        deltaTime * _carMovementConfig.WheelSteerLerpSpeed
                    );
                }*/

                // Применяем комбинированное вращение (сначала Y, потом X для корректного порядка)
                wheelTransform.localRotation = rotationY * wheelTransform.localRotation * rotationX;
            }
        }
    }
}
