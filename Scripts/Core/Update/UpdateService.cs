using System.Collections.Generic;
using DG.Tweening;
using Game.Scripts.Core.Update.Interfaces;
using UnityEngine;

namespace Game.Scripts.Core.Update
{
    public class UpdateService : MonoBehaviour
    {
        private readonly List<IUpdate> _updateElements = new();
        private readonly List<IFixedUpdate> _fixedUpdateElements = new();
        private readonly List<ILateUpdate> _lateUpdateElements = new();
        private readonly List<IDrawGizmo> _drawGizmosElements = new();

        private float _updateFactor = 1.0f;
        private bool _isGamePaused = false;
        
        public float DeltaTime { get; private set; }
        public float FixedDeltaTime { get; private set; }

        public void AddUpdateElement(IUpdate update) => _updateElements.Add(update);
        public void RemoveUpdateElement(IUpdate update) => _updateElements.Remove(update);

        public void AddFixedUpdateElement(IFixedUpdate fixedUpdate) => _fixedUpdateElements.Add(fixedUpdate);
        public void RemoveFixedUpdateElement(IFixedUpdate fixedUpdate) => _fixedUpdateElements.Remove(fixedUpdate);
        

        public void AddLateUpdateElement(ILateUpdate lateUpdate) => _lateUpdateElements.Add(lateUpdate);
        public void RemoveLateUpdateElement(ILateUpdate lateUpdate) => _lateUpdateElements.Remove(lateUpdate);

        public void AddDrawGizmoElement(IDrawGizmo drawGizmo) => _drawGizmosElements.Add(drawGizmo);
        public void RemoveDrawGizmoElement(IDrawGizmo drawGizmo) => _drawGizmosElements.Remove(drawGizmo);
        public void Clear()
        {
            _fixedUpdateElements.Clear();
            _updateElements.Clear();
            _lateUpdateElements.Clear();
            _drawGizmosElements.Clear();

            _updateFactor = 1.0f;
        }

        public void SetIsPause(bool isPaused) => 
            _isGamePaused = isPaused;

        private void FixedUpdate()
        {
#if UNITY_EDITOR
            if (Input.GetKey(KeyCode.Q))
                return;
#endif
            if (_isGamePaused)
                return;
            
            FixedDeltaTime = Time.fixedDeltaTime * _updateFactor;

            foreach (IFixedUpdate fixedUpdateElement in _fixedUpdateElements)
                fixedUpdateElement.ManualFixedUpdate(FixedDeltaTime);
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKey(KeyCode.Q))
                return;
#endif
            if (_isGamePaused)
                return;
            
            DeltaTime = Time.deltaTime * _updateFactor;

            foreach (IUpdate updateElement in _updateElements)
                updateElement.ManualUpdate(DeltaTime);

            DOTween.ManualUpdate(DeltaTime, DeltaTime);
        }

        private void LateUpdate()
        {
#if UNITY_EDITOR
            if (Input.GetKey(KeyCode.Q))
                return;
#endif

            if (_isGamePaused)
                return;
            
            foreach (ILateUpdate lateUpdateElement in _lateUpdateElements)
                lateUpdateElement.ManualLateUpdate(DeltaTime);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            foreach (var element in _drawGizmosElements)
            {
                element.ManualDrawGizmo();
            }
        }
#endif

        private void OnDestroy()
        {
            Clear();
        }
    }
}