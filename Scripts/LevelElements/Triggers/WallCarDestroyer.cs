using System;
using DG.Tweening;
using Game.Scripts.Constants;
using UnityEngine;

namespace Game.Scripts.LevelElements.Triggers
{
    public class WallCarDestroyer : MonoBehaviour
    {
        [SerializeField]private Transform _wallsDestroyer;
        [SerializeField]private float _moveDistance;
        [SerializeField]private float _duration;
        
        private bool _isMove = false;
        
        public void DestroyCar()
        {
            if(_isMove)
                return;
            _wallsDestroyer.DOLocalMoveZ(_wallsDestroyer.transform.localPosition.z + _moveDistance, _duration).SetEase(Ease.InOutSine);
            _isMove = true;
        }
    }
}