using System;
using DG.Tweening;
using Game.Scripts.Utils;
using UnityEngine;

namespace Game.Scripts.UI.Widgets.Base
{
    public abstract class UIAnimatedWidget : MonoBehaviour
    {
        [SerializeField] private WidgetSettings _widgetSettings;
        [SerializeField] private bool _activateOnEnable;

        protected WidgetSettings WidgetSettings => _widgetSettings;

        private Tween _enableTween;

        public virtual void OnShowStart()
        {
            _widgetSettings.AnimatedRoot.blocksRaycasts = true;
            _widgetSettings.AnimatedRoot.alpha = 1f;
        }

        public Tween Show() => AnimateWidget(_widgetSettings.ShowAnimationParameter, 1.0f);

        public virtual void OnShowComplete()
        {
        }

        public virtual void OnHideStart()
        {
        }

        public Tween Hide() => AnimateWidget(_widgetSettings.HideAnimationParameter, 0.0f);

        public virtual void OnHideComplete()
        {
            _widgetSettings.AnimatedRoot.blocksRaycasts = false;
            _widgetSettings.AnimatedRoot.alpha = 0f;
        }

        private Tween AnimateWidget(AnimationParameterConfig animationParameter, float alpha, Action onCompleted = null)
        {
            KillTween();
            _enableTween = _widgetSettings.AnimatedRoot.DOFade(alpha, animationParameter.Duration)
                .SetEase(animationParameter.Ease);

            return _enableTween;
        }

        private void OnEnable()
        {
            if (_activateOnEnable)
            {
                _widgetSettings.AnimatedRoot.blocksRaycasts = true;
                _widgetSettings.AnimatedRoot.alpha = 1f;
            }
            else
            {
                _widgetSettings.AnimatedRoot.blocksRaycasts = false;
                _widgetSettings.AnimatedRoot.alpha = 0f;
            }
        }

        private void KillTween() => _enableTween?.Kill();

        protected virtual void OnDestroy()
        {
            KillTween();
        }
    }
}