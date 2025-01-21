using System;
using System.Collections;
using Cinemachine;
using Configs;
using DG.Tweening;
using Game.Scripts.Constants;
using Game.Scripts.Infrastructure.Bootstrapper;
using Game.Scripts.Infrastructure.Services.Car;
using Game.Scripts.Infrastructure.States;
using Game.Scripts.LevelElements;
using Game.Scripts.LevelElements.Player;
using Game.Scripts.Utils.Debug;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Infrastructure.Services
{
    public enum EGameCameraType
    {
        Menu,
        Gameplay,
        FlyEnd,  // Камера для кинематографических сцен
        Fly,         // Камера для свободного полета
        WallHit
    }

    public class CameraService 
    {
        private CinemachineVirtualCamera _gameplayCamera;
        private CinemachineVirtualCamera _menuCamera;
        private CinemachineVirtualCamera _flyEndCamera; 
        private CinemachineVirtualCamera _flyCamera;       
        private CinemachineVirtualCamera _wallHitCamera;

        private readonly CamerasConfig _camerasConfig;
        
        private Transform _car;
        private Transform _playerCameraTarget;
        
        private Cinemachine3rdPersonFollow _thirdPersonGameplay;
        private LevelCameraView _levelCameraView;
        private CinemachineBasicMultiChannelPerlin _noiseComponent;

        private Tween _fovTween;
        
        private float _defaultFov;
        private Cinemachine3rdPersonFollow _flyCameraFollow;
        private EGameCameraType _currentCameraType;
        
        private Cinemachine3rdPersonFollow _thirdPersonWallHit;

        private Coroutine _adjustFlyCameraToGroundRoutine;
        
        private readonly ICoroutineRunnerService _coroutineRunnerService = CoroutineRunner.Instance;
        
        private float _currentCameraDistance;
        private float _currentCameraHeight;
        private Rigidbody _playerRigidbody;
        private float _gameplayCameraChangeSpeed;

        [Inject]
        public CameraService(GameConfig gameConfig)
        {
            _camerasConfig = gameConfig.CamerasConfig;
        }

        public void SetLevelCameras(LevelCameraView levelCameraView)
        {
            _levelCameraView = levelCameraView;
            
            _gameplayCamera = levelCameraView.GameplayCamera;
            _wallHitCamera = levelCameraView.WallHitCamera;
            _menuCamera = levelCameraView.MenuCamera;
            _flyEndCamera = levelCameraView.FlyEndCamera;  
            _flyCamera = levelCameraView.FlyCamera;

            _defaultFov = _gameplayCamera.m_Lens.FieldOfView;
        }

        public void SetCar(Transform car)
        {
            _car = car;
            
            _gameplayCamera.Follow = _car;
            _gameplayCamera.LookAt = _car;
            _thirdPersonGameplay = _gameplayCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
            _noiseComponent = _gameplayCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();            
            
            _wallHitCamera.Follow = _car;
            _wallHitCamera.LookAt = _car;
            _thirdPersonWallHit = _wallHitCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
            
            _flyCameraFollow = _flyCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();

            _menuCamera.Follow = _car;
            _menuCamera.LookAt = _car;
        }
        public void SetPlayerHumanoid(PlayerView player)
        {
            _playerCameraTarget = player.CameraTarget;
            _playerRigidbody = player.RigidbodyRoot;
            
            if (_flyEndCamera != null)
            {
                _flyEndCamera.Follow = _playerCameraTarget;
                _flyEndCamera.LookAt = _playerCameraTarget;
            }

            if (_flyCamera != null)
            {
                _flyCamera.Follow = _playerCameraTarget;
                _flyCamera.LookAt = _playerCameraTarget;
            }

            foreach (var cam in _levelCameraView.OtherCamerasForPlayerHumanoidFollow)
            {
                cam.Follow = _playerCameraTarget;
                cam.LookAt = _playerCameraTarget;
            }
        }
        public void SetActiveCamera(EGameCameraType cameraType)
        {
            CustomDebugLog.Log("SetActiveCamera " + cameraType);
            _currentCameraType = cameraType;

            DisableAllCameras();  
            
            switch (cameraType)
            {
                case EGameCameraType.Gameplay:
                    if (_gameplayCamera != null)
                        _gameplayCamera.gameObject.SetActive(true);
                    break;
                               
                
                case EGameCameraType.WallHit:
                    if (_wallHitCamera != null)
                        _wallHitCamera.gameObject.SetActive(true);
                    break;
                
                case EGameCameraType.Menu:
                    if (_menuCamera != null)
                        _menuCamera.gameObject.SetActive(true);
                    break;

                case EGameCameraType.FlyEnd:
                    if (_flyEndCamera != null)
                        _flyEndCamera.gameObject.SetActive(true);
                    break;

                case EGameCameraType.Fly:
                    if (_flyCamera != null)
                    {
                        _flyCamera.transform.position = _gameplayCamera.transform.position;
                        _flyCamera.gameObject.SetActive(true);
                        //_adjustFlyCameraToGroundRoutine = _coroutineRunnerService.StartCoroutine(AdjustFlyCameraToSpeedRoutine());
                        _adjustFlyCameraToGroundRoutine = _coroutineRunnerService.StartCoroutine(AdjustFlyCameraToSpeedRoutine());
                    }
                    break;

                default:
                    Debug.LogWarning("Unknown camera type: " + cameraType);
                    break;
            }
        }

        // Отключаем все камеры перед активацией нужной
        private void DisableAllCameras()
        {
            if (_gameplayCamera != null)
                _gameplayCamera.gameObject.SetActive(false);

            if (_menuCamera != null)
                _menuCamera.gameObject.SetActive(false);

            if (_flyEndCamera != null)
                _flyEndCamera.gameObject.SetActive(false);

            if (_flyCamera != null)
                _flyCamera.gameObject.SetActive(false);    
            
            if (_wallHitCamera != null)
                _wallHitCamera.gameObject.SetActive(false);
        }

        public void HandleCarSpeedChange(float carSpeed, float maxCarSpeed)
        {
            var t = carSpeed / maxCarSpeed;
            _gameplayCameraChangeSpeed = Mathf.Lerp(_gameplayCameraChangeSpeed, t, Time.deltaTime * _camerasConfig.ChangeSpeed);
            ChangeGameplayCameraDistance(_gameplayCameraChangeSpeed);
            ChangeCameraNoiseAmplitude(_gameplayCameraChangeSpeed);
        }

        private void ChangeGameplayCameraDistance(float t)
        {
            var distance = Mathf.Lerp(_camerasConfig.MinGameplayCameraDistance, _camerasConfig.MaxGameplayCameraDistance, t);
            _thirdPersonGameplay.CameraDistance = distance;
            
            var height = Mathf.Lerp(_camerasConfig.MinGameplayCameraHeight, _camerasConfig.MaxGameplayCameraHeight, t);
            _thirdPersonGameplay.VerticalArmLength = height;
        }
        private void ChangeCameraNoiseAmplitude(float t)
        {
            float easeMultiplier = _camerasConfig.NoiseAmplitudeAnimationCurve.Evaluate(t);
            var noiseAmplitude = Mathf.Lerp(_camerasConfig.MinCameraNoiseAmplitude, _camerasConfig.MaxCameraNoiseAmplitude, t);
            var noiseFrequencyGain = Mathf.Lerp(_camerasConfig.MinCameraNoiseFrequencyGain, _camerasConfig.MaxCameraNoiseFrequencyGain, t);
            _noiseComponent.m_AmplitudeGain = noiseAmplitude * easeMultiplier;
            _noiseComponent.m_FrequencyGain = noiseFrequencyGain * easeMultiplier;
        }

        public void ChangeCameraFovOnBoost()
        {
            _fovTween?.Kill();

            var fovOnBoost = _defaultFov * _camerasConfig.FovOnBoostMultiplyer;
            _fovTween = DOTween.To(
                () => _gameplayCamera.m_Lens.FieldOfView,      
                value => _gameplayCamera.m_Lens.FieldOfView = value, 
                fovOnBoost,                                
                _camerasConfig.EnterBoostFovAnimation.Duration                                 
            ).SetEase(_camerasConfig.EnterBoostFovAnimation.Ease);                     
        }
        public void ResetCameraFov()
        {
            // Останавливаем предыдущую анимацию, если она существует
            _fovTween?.Kill();
            
            // Анимация возврата к стандартному FOV
            _fovTween = DOTween.To(
                () => _gameplayCamera.m_Lens.FieldOfView,
                value => _gameplayCamera.m_Lens.FieldOfView = value,
                _defaultFov,
                _camerasConfig.ExitBoostFovAnimation.Duration
            ).SetEase(_camerasConfig.ExitBoostFovAnimation.Ease);
        }

        private IEnumerator AdjustFlyCameraToSpeedRoutine()
        {
            while (_currentCameraType == EGameCameraType.Fly)
            {
                AdjustFlyCameraToSpeed(_playerCameraTarget);
                yield return null; 
            }
        }
        public void AdjustFlyCameraToSpeed(Transform player)
        {
            // Calculate player speed
            Vector3 playerVelocity = _playerRigidbody.velocity; // Assuming _playerRigidbody is a Rigidbody attached to the player
            float playerSpeed = playerVelocity.magnitude;

            // Normalize speed within the expected range
            float maxPlayerSpeed = _camerasConfig.MaxPlayerSpeed; // Define the maximum expected speed in your configuration
            float t = Mathf.InverseLerp(0, maxPlayerSpeed, playerSpeed);

            // Calculate target camera distance and height based on speed
            float targetCameraDistance = Mathf.Lerp(_camerasConfig.MinFlyCameraDistance, _camerasConfig.MaxFlyCameraDistance, t);
            float targetCameraHeight = Mathf.Lerp(_camerasConfig.MinFlyCameraHeight, _camerasConfig.MaxFlyCameraHeight, t);

            // Smoothly interpolate current camera settings towards the target values
            _currentCameraDistance = Mathf.Lerp(_currentCameraDistance, targetCameraDistance, Time.deltaTime * _camerasConfig.FlyCameraChangeSpeed);
            _currentCameraHeight = Mathf.Lerp(_currentCameraHeight, targetCameraHeight, Time.deltaTime * _camerasConfig.FlyCameraChangeSpeed);

            // Apply the calculated values to the camera
            _flyCameraFollow.CameraDistance = _currentCameraDistance;
            _flyCameraFollow.VerticalArmLength = _currentCameraHeight;
        }
        private IEnumerator AdjustFlyCameraToGroundRoutine()
        {
            while (_currentCameraType == EGameCameraType.Fly)
            {
                AdjustFlyCameraToGround(_playerRigidbody.transform);
                yield return null; // Correctly yield control
            }
        }
        public void AdjustFlyCameraToGround(Transform player)
        {
            if (_flyCamera == null)
                return;

            int groundLayer = LayerMask.GetMask(LayersConstants.GROUND);
            if (Physics.Raycast(player.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundLayer))
            {
                float groundHeight = hit.point.y;
                float playerHeight = player.position.y - groundHeight;

                float t = Mathf.InverseLerp(0, _camerasConfig.MaxFlyCameraHeight, playerHeight);
                float targetCameraDistance = Mathf.Lerp(_camerasConfig.MinFlyCameraDistance, _camerasConfig.MaxFlyCameraDistance, t);
                float targetCameraHeight = Mathf.Lerp(_camerasConfig.MinFlyCameraHeight, _camerasConfig.MaxFlyCameraHeight, t);

                _currentCameraDistance = Mathf.Lerp(_currentCameraDistance, targetCameraDistance, Time.deltaTime * _camerasConfig.FlyCameraChangeSpeed);
                _currentCameraHeight = Mathf.Lerp(_currentCameraHeight, targetCameraHeight, Time.deltaTime * _camerasConfig.FlyCameraChangeSpeed);

                _flyCameraFollow.CameraDistance = _currentCameraDistance;
                _flyCameraFollow.VerticalArmLength = _currentCameraHeight;
            }
        }

        public void Clear()
        {
            if (_adjustFlyCameraToGroundRoutine != null)
            {
                _coroutineRunnerService.StopCoroutine(_adjustFlyCameraToGroundRoutine);
                _adjustFlyCameraToGroundRoutine = null;
            }
            
            _fovTween?.Kill();

            _currentCameraHeight = _camerasConfig.MinFlyCameraHeight;
            _currentCameraDistance = _camerasConfig.MinFlyCameraDistance;
            
            _gameplayCamera = null;
            _menuCamera = null;
            _flyEndCamera = null;
            _flyCamera = null;
        }
    }
}