using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Scripts.Utils;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Game.Scripts.UI.Screens.Base
{
    public abstract class BaseScreen : MonoBehaviour
    {
        #region Nested

        public class Factory : PlaceholderFactory<Transform, Object, BaseScreen>
        {
        }

        #endregion

        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private AnimationParameterConfig _showConfig;
        [SerializeField] private AnimationParameterConfig _hideConfig;

        private Tween _changeAlphaTween;

        public virtual void OnCreate()
        {
            SetActiveScreen(false);
            SetAlpha(0.0f);
        }

        #region Open

        public virtual UniTask OnOpenStart()
        {
            SetActiveScreen(true);
            SetAlpha(0.0f);

            return UniTask.CompletedTask;
        }

        public virtual async UniTask PlayOpenAnimation() => await PlayFadeTween(0.0f, 1.0f, _showConfig).AsyncWaitForCompletion();
        public virtual UniTask OnOpenComplete() => UniTask.CompletedTask;

        #endregion

        #region Close

        public virtual UniTask OnCloseStart() => UniTask.CompletedTask;
        public virtual async UniTask PlayCloseAnimation() => await PlayFadeTween(1.0f, 0.0f, _hideConfig).AsyncWaitForCompletion();

        public virtual UniTask OnCloseComplete()
        {
            SetActiveScreen(false);
            SetAlpha(0.0f);

            return UniTask.CompletedTask;
        }

        #endregion

        private void SetActiveScreen(bool isActive) => gameObject.SetActive(isActive);
        private void SetAlpha(float alpha) => _canvasGroup.alpha = alpha;

        private Tween PlayFadeTween(float from, float to, AnimationParameterConfig config)
        {
            KillTween();
            _changeAlphaTween = DOVirtual.Float(from, to, config.Duration, SetAlpha).SetEase(config.Ease).SetUpdate(true);

            return _changeAlphaTween;
        }

        private void KillTween() => _changeAlphaTween?.Kill();

        protected virtual void OnDestroy()
        {
            KillTween();
        }
    }
}