using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Scripts.Customization
{
    public class PreviewRotator : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField] private float rotationSpeed = 50.0f;
        [SerializeField] private float _automaticRotationSpeed = 50.0f;
        [SerializeField] private float _startRotation = 50.0f;
        [SerializeField] private float _dragCooldown = 2.0f;

        private Transform _target;
        private Vector2 _lastTouchPos;
        private bool isRotating = true;

        public void SetTarget(Transform target)
        {
            _target = target;
        }

        public void SetDefaultRotation()
        { 
            if(_target == null)
                return;
            _target.rotation = Quaternion.Euler(new Vector3(0, _startRotation, 0));
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 currentTouchPos = eventData.position;
            float swipeDelta = (currentTouchPos.x - _lastTouchPos.x) * rotationSpeed * Time.deltaTime;
            _target.transform.Rotate(Vector3.up, swipeDelta);
            _lastTouchPos = currentTouchPos;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _lastTouchPos = eventData.position;
            isRotating = false;
            StopCoroutine(EnableRotationAfterDrag());
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            StartCoroutine(EnableRotationAfterDrag());
        }

        private IEnumerator EnableRotationAfterDrag()
        {
            yield return new WaitForSeconds(_dragCooldown);
            isRotating = true;
        }

        private void Update()
        {
            if(_target == null)
                return;
            if (isRotating)
            {
                _target.transform.Rotate(Vector3.up, _automaticRotationSpeed * Time.deltaTime);
            }
        }
    }
}