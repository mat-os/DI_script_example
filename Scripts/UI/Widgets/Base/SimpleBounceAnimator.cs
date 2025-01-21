using DG.Tweening;
using UnityEngine;

namespace Game.Scripts.UI.Widgets.Base
{
    public class SimpleBounceAnimator : MonoBehaviour
    {
        [Header("Animation Settings")] 
        [SerializeField]private Transform _target;
        [SerializeField] private float _scaleAnimationDuration;
        [SerializeField] private Vector3 _selectedScale;
        [SerializeField] private Ease _scaleEase;


        private void OnDisable() => DOTween.Kill(this);

        public void Bounce()
        {
            _target.DOKill(true);
            _target.DOScale(new Vector3(_selectedScale.x, _selectedScale.y, _selectedScale.x), _scaleAnimationDuration).SetEase(_scaleEase)
                .SetUpdate(true).OnComplete(OnComplete_Handler).SetId(this);
        }

        private void OnComplete_Handler()
        {
            _target.DOKill(true);
            _target.DOScale(Vector3.one, _scaleAnimationDuration).SetEase(_scaleEase).SetUpdate(true).SetId(this);
        }
    }
}