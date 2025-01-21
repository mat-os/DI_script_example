using System;
using DG.Tweening;
using Game.Scripts.Utils;
using UnityEngine;

namespace Game.Scripts.UI.Widgets
{
    public class ShowMoveUIAnimation : MonoBehaviour, IDisposable
    {
        [SerializeField] private AnimationParameterConfig _animationParameterConfig;

        [SerializeField] private Transform _showPosition;
        [SerializeField] private Transform _hidePosition;
        
        private Tween _planeAnimationTween;
        public void Show(Transform plane)
        {
            DisposeTween();

            plane.gameObject.SetActive(true);
            _planeAnimationTween = plane.DOLocalMove(_showPosition.localPosition, _animationParameterConfig.Duration)
                .SetEase(_animationParameterConfig.Ease)
                .From(_hidePosition.localPosition);
        }
        public void Hide(Transform plane)
        {
            //DisposeTween();
            _planeAnimationTween = plane.DOLocalMove(_hidePosition.localPosition, _animationParameterConfig.Duration)
                .SetEase(_animationParameterConfig.Ease)
                .From(_showPosition.localPosition)
                .OnComplete(() => plane.gameObject.SetActive(false));
        }
        private void DisposeTween()
        {
            if (_planeAnimationTween != null)
            {
                _planeAnimationTween.Complete();
                _planeAnimationTween.Kill();
            }
        }

        public void Dispose()
        {
            DisposeTween();
        }
    }
}