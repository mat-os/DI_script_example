using System;
using System.Collections;
using Game.Scripts.Infrastructure.Bootstrapper;
using Game.Scripts.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Scripts.Infrastructure.Services
{
    public class InputService : IDisposable
    {
        private readonly ICoroutineRunnerService _coroutineRunnerService = CoroutineRunner.Instance;
        private readonly float _screenWidth = Screen.width;

        private InputHandler _inputHandler;
        private bool _isActive;
        private float _oldXPosition;
        private Vector3 _screenInput;
        private Coroutine _touchHoldCoroutine;
        private bool _isPointerHandled = false;

        public Vector3 InputVector
        {
            get
            {
                Vector3 worldInput = Vector3.zero;

                worldInput.x = (_screenInput.x - _oldXPosition) / _screenWidth;
                worldInput.z = _screenInput.z;
                return worldInput;
            }
        }


        public void SetInputHandler(InputHandler inputHandler)
        {
            _inputHandler = inputHandler; 
            
            _inputHandler.OnDown += OnDown_Handler;
            _inputHandler.OnUp += OnUp_Handler;
        }
        
        public void EnableInput(bool isActive)
        {
            Debug.Log("[INPUT] EnableInput called " + isActive);
            if (isActive)
            {
                _isActive = true;
            }
            else
            {
                _isActive = false;

                if (_touchHoldCoroutine != null)
                    _coroutineRunnerService.StopCoroutine(_touchHoldCoroutine);

                _screenInput = Vector3.zero;
                _oldXPosition = 0;
            }
            _inputHandler.gameObject.SetActive(_isActive);
        }

        private void OnDown_Handler(PointerEventData eventData)
        {
            if (!_isActive)
                return;

            if(_isPointerHandled)
                return;

            _isPointerHandled = true;
            
            _oldXPosition = _screenInput.x == 0 ? eventData.position.x : _screenInput.x;
            _screenInput.x = eventData.position.x;
            _screenInput.z = 1f;

            _touchHoldCoroutine = _coroutineRunnerService.StartCoroutine(Coroutine_HoldButton());
        }

        private void OnUp_Handler(PointerEventData eventData)
        {
            if (!_isActive)
                return;
            
            _isPointerHandled = false;

            if (_touchHoldCoroutine != null)
                _coroutineRunnerService.StopCoroutine(_touchHoldCoroutine);

            _oldXPosition = 0f;
            _screenInput.x = 0f;
            _screenInput.z = 0f;
        }

        private IEnumerator Coroutine_HoldButton()
        {
            while (true)
            {
                if (!_isActive)
                    yield return null;

                _oldXPosition = _screenInput.x;

                #if UNITY_EDITOR
                
                _screenInput.x = Input.mousePosition.x;
                
                #else
                
                if (Input.touches.Length != 0)
                {
                    _screenInput.x = Input.touches[0].position.x;
                }
                
                #endif

                _screenInput.z = 1;
                
                yield return null;
            }
        }
        
        public void Dispose()
        {
            if (_inputHandler != null)
            {
                _inputHandler.OnDown -= OnDown_Handler;
                _inputHandler.OnUp -= OnUp_Handler;
            }
        }
    }
}