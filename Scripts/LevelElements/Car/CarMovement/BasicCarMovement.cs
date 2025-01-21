using Configs;
using Game.Scripts.Configs.Player;
using Game.Scripts.Utils;
using UnityEngine;

namespace Game.Scripts.LevelElements.Car.CarMovement
{
    public class BasicCarMovement : ICarMovement
    {
        private readonly Rigidbody _rb;
        private readonly CarMovementConfig _config;
        private readonly Transform _carModelTransform;
        private readonly float _difficultyAccelerationMultiplier;

        private readonly float _maxSpeed;
        private float _boostSpeedMultiplier;

        private float _currentSpeed;
        private float _fallSpeed;
        private Vector3 _inAirForwardVelocity;
        
        private float _smoothedSteerInput = 0f; 
        private float _smoothedSteerMoveInput = 0f;

        private readonly float _roadWidth;
        
        private  float _accelerationMultiplier;
        private  float _maxSpeedMultiplier;
        
        private float currentAngle = 0f; // Текущий угол поворота
        private float _currentLateralVelocity = 0f; // Текущая боковая инерция
        private Vector3 _startPosition;
        private float _leftBoundary;
        private float _rightBoundary;

        public BasicCarMovement(CarView carView, CarMovementConfig config,
            CarUpgradeStep carUpgradeStep, float difficultyCarAccelerationMultiplier, float roadWidth)
        {
            _rb = carView.Rigidbody;
            _config = config;
            _currentSpeed = 0f;
            _carModelTransform = carView.CarModel;
            _boostSpeedMultiplier = 1;
            _difficultyAccelerationMultiplier = difficultyCarAccelerationMultiplier;

            _roadWidth = roadWidth;
            _startPosition = _carModelTransform.position;
            _leftBoundary = _startPosition.x - _roadWidth;
            _rightBoundary = _startPosition.x + _roadWidth;
            
            UpgradeCarParameters(carUpgradeStep);
            
            _maxSpeed = _config.MaxSpeedMs * _maxSpeedMultiplier;
        }

        // Main movement function
        public void Move(Vector3 input)
        {
            bool isGrounded = CheckGround(out RaycastHit hit);

            Vector3 forwardMove = isGrounded ? 
                CalculateForwardMove(input.z) : 
                ApplyInAirInertia();

            Vector3 sideMove = CalculateSteeringMove(input.x);
            Vector3 moveDirection = forwardMove + sideMove;

            if (!isGrounded)
            {
                moveDirection += ApplyInAirGravity();
            }

            ApplyMovement(moveDirection);
            RotateCarOnSlope(hit, isGrounded);
        }
        // Limit the car's position to the road boundaries
        private Vector3 LimitPositionWithinRoad(Vector3 targetPos)
        {
            // Ограничиваем позицию машины в пределах рассчитанных границ
            targetPos.x = Mathf.Clamp(targetPos.x, _leftBoundary, _rightBoundary);
           // targetPos.x = Mathf.Clamp(targetPos.x, - _roadWidth, _roadWidth);
            return targetPos;
        }
        private void ApplyMovement(Vector3 moveDirection)
        {
            Vector3 targetPos = _rb.position + moveDirection;
            targetPos = LimitPositionWithinRoad(targetPos);
            _rb.MovePosition(targetPos);
        }

        public void Rotate(Vector3 input)
        {
            ApplyVisualSteeringRotation(input.x);
        }
        private bool CheckGround(out RaycastHit hit)
        {
            Vector3 rayOrigin = _rb.position + Vector3.up * _config.GroundRaycastUpOffset;
            return Physics.Raycast(rayOrigin, Vector3.down, out hit, _config.GroundRaycastCheckDistance, LayerMask.GetMask("Ground"));
        }

        // Calculate forward movement when grounded
        private Vector3 CalculateForwardMove(float forwardInput)
        {
            if (_boostSpeedMultiplier > 1)
                forwardInput = 1f;
            
            if (forwardInput > 0)
            {
                float normalizedSpeed = _currentSpeed / _maxSpeed;

                float accelerationFactor = _config.AccelerationCurve.Evaluate(normalizedSpeed) 
                                           * _accelerationMultiplier 
                                           * _boostSpeedMultiplier
                                           * _difficultyAccelerationMultiplier;

                if (_currentSpeed > _maxSpeed)
                {
                    if (Mathf.Approximately(_boostSpeedMultiplier, 1f))
                    {
                        // Без boost: жесткое ограничение скорости
                        _currentSpeed = Mathf.Clamp(_currentSpeed, 0, _maxSpeed);
                    }
                    else
                    {
                        // С boost: замедление ускорения
                        float excessSpeed = _currentSpeed - _maxSpeed;
                        float speedReductionFactor = Mathf.Lerp(0.1f, 1f, _boostSpeedMultiplier - 1f); // Чем ближе boost к 1, тем сильнее замедление
                        accelerationFactor *= 1f / (1f + excessSpeed * speedReductionFactor); // Уменьшаем ускорение пропорционально превышению скорости
                    }
                }

                _currentSpeed = Mathf.Lerp(_currentSpeed, _maxSpeed * forwardInput * accelerationFactor, _config.Acceleration * Time.fixedDeltaTime);

                if (Mathf.Approximately(_boostSpeedMultiplier, 1f))
                    _currentSpeed = Mathf.Clamp(_currentSpeed, 0, _maxSpeed);

                // Сохраняем вектор скорости для использования в воздухе
                _inAirForwardVelocity = _rb.transform.forward * _currentSpeed;

                return _inAirForwardVelocity * Time.fixedDeltaTime;
            }
            else
            {
                return ApplyBraking();
            }
        }

        // Handle braking if no input is provided
        private Vector3 ApplyBraking()
        {
            float normalizedSpeed = _currentSpeed / _config.MaxSpeedMs;
            float brakingFactor = _config.BrakeCurve.Evaluate(normalizedSpeed);

            _currentSpeed = Mathf.Lerp(_currentSpeed, 0, _config.BrakeDeceleration * brakingFactor * Time.fixedDeltaTime);

            // Store the forward velocity for in-air movement
            _inAirForwardVelocity = _rb.transform.forward * _currentSpeed;

            return _inAirForwardVelocity * Time.fixedDeltaTime;
        }

        // Apply inertia when airborne, using the forward velocity calculated while grounded
        private Vector3 ApplyInAirInertia()
        {
            return _inAirForwardVelocity * Time.fixedDeltaTime;
        }

        // Apply gravity when the car is airborne
        private Vector3 ApplyInAirGravity()
        {
            _fallSpeed += _config.GravityForce * Time.fixedDeltaTime;
            return Vector3.down * (_fallSpeed * Time.fixedDeltaTime);
        }


        private Vector3 CalculateSteeringMove(float steerInput)
        {
            float currentSpeedNormalized = Mathf.Clamp01(_currentSpeed / _config.MaxSpeedMs);
            
            // Плавное изменение текущего ввода для предотвращения резких движений
            _smoothedSteerMoveInput = Mathf.Lerp(_smoothedSteerMoveInput, steerInput, _config.InputSteeringSensitivity * Time.fixedDeltaTime);

            // Вычисляем желаемую боковую скорость на основе текущего ввода
            float targetLateralVelocity = _smoothedSteerMoveInput * _config.SteeringSensitivity;

            if (!Mathf.Approximately(steerInput, 0f))
            {
                float currentSteeringResponsiveness = _config.SteeringResponsivenessCurveMultiplier.Evaluate(currentSpeedNormalized) 
                                                      * _config.SteeringResponsiveness
                                                      * Time.fixedDeltaTime;
                
                // Если есть ввод, обновляем боковую инерцию на основе ввода
                _currentLateralVelocity = Mathf.Lerp(_currentLateralVelocity, targetLateralVelocity, currentSteeringResponsiveness);
            }
            else
            {
                // Если ввода нет, инерция постепенно затухает
                float currentInertiaDecaySpeed = _config.InertiaDecaySpeedCurveMultiplier.Evaluate(currentSpeedNormalized) 
                                                 * _config.InertiaDecaySpeed 
                                                 * Time.fixedDeltaTime;

                _currentLateralVelocity = Mathf.Lerp(_currentLateralVelocity, 0f, currentInertiaDecaySpeed);
            }

            // Возвращаем боковое движение на основе текущей инерции
            return _rb.transform.right * (_currentLateralVelocity * Time.fixedDeltaTime);
        }
        private void ApplyVisualSteeringRotation(float steerInput)
        {
            // Целевой угол на основе ввода
            float targetAngle = steerInput * _config.MaxRotationAngle;

            // Если ввода нет, возвращаемся к дефолтному положению (угол = 0)
            if (Mathf.Approximately(steerInput, 0f))
            {
                targetAngle = 0f;
            }

            // Плавно интерполируем текущий угол к целевому
            float smoothSpeed = Mathf.Approximately(steerInput, 0f) ? _config.ReturnRotationSpeed : _config.RotationSpeed;
            currentAngle = Mathf.Lerp(currentAngle, targetAngle, smoothSpeed * Time.deltaTime);

            // Применяем поворот к объекту
            _carModelTransform.localRotation = Quaternion.Euler(0f, currentAngle, 0f);
        }
        // Rotate the car to match the slope of the ground if grounded
        private void RotateCarOnSlope(RaycastHit hit, bool isGrounded)
        {
            if (isGrounded && hit.collider != null)
            {
                Vector3 surfaceNormal = hit.normal;
                Quaternion targetRotation = Quaternion.FromToRotation(_rb.transform.up, surfaceNormal) * _rb.rotation;
                _rb.MoveRotation(Quaternion.Lerp(_rb.rotation, targetRotation, _config.InclineAdjustmentSpeed * Time.fixedDeltaTime));
                //_rb.MoveRotation(targetRotation);
            }
        }
        

        public void ApplyCollisionSlowdown(float percent)
        {
            percent = Mathf.Clamp(percent, 0, 100);
            float reduction = _currentSpeed * (percent / 100f);
            _currentSpeed = Mathf.Max(0, _currentSpeed - reduction);
            _inAirForwardVelocity = _rb.transform.forward * _currentSpeed;
        }

        public void ApplyImmediateSpeedBoost(float immediateBoostSpeedAmount)
        {
            //TODO:
            _currentSpeed += immediateBoostSpeedAmount;
        }

        public float GetSpeed()
        {
            return _currentSpeed;
        }
        
        public float GetHorizontalInertia()
        {
            return _currentLateralVelocity;
        }

        public float GetMaxSpeed()
        {
            return _maxSpeed;
        }

        public void SetSpeedMultiplier(float speedMultiplier)
        {
            _boostSpeedMultiplier = speedMultiplier;
        }

        public void UpgradeCarParameters(CarUpgradeStep carUpgradeStep)
        {
            _accelerationMultiplier = carUpgradeStep.CarAccelerationMultiplier;
            _maxSpeedMultiplier = carUpgradeStep.MaxCarSpeedMultiplier;
        }
    }
}
