using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Scripts.Configs;
using Game.Scripts.Configs.Vfx;
using Game.Scripts.Infrastructure.Services.Player;
using Game.Scripts.LevelElements.Collisions;
using Game.Scripts.Utils;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Infrastructure.Services.Level
{
    public class TargetHitMissionService : IDisposable
    {
        public Action<ETargetHitResult> OnPlayerHitTarget;
        
        private readonly PrefabRepository _prefabRepository;
        
        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        
        private bool _isHit;
        private TargetHitView _targetHitView;
        private SlowMotionService _slowMotionService;


        [Inject]
        public TargetHitMissionService(PrefabRepository prefabRepository, SlowMotionService slowMotionService)
        {
            _slowMotionService = slowMotionService;
            _prefabRepository = prefabRepository;
            
            GlobalEventSystem.Broker.Receive<TargetHitEvent>()
                .Subscribe(OnHitTargetHandle)
                .AddTo(_disposable);
        }

        private void OnHitTargetHandle(TargetHitEvent targetHitEvent)
        {
            if(_isHit)
                return;

            _targetHitView = targetHitEvent.TargetHitView;
            float distanceFromCenter = GetDistanceFromCenter(targetHitEvent.HitPosition);

            DetermineHitZone(distanceFromCenter);
            //SpawnFlagWithAnimation(targetHitEvent.HitPosition, targetHitEvent.TargetHitView.transform);
            
            _slowMotionService.StartSlowMo(ESlowMotionType.TargetHit, true);
            _isHit = true;
        }
        private float GetDistanceFromCenter(Vector3 hitPosition)
        {
            return Vector3.Distance(_targetHitView.TargetCenter.position, hitPosition);
        }
        private void SpawnFlagWithAnimation(Vector3 position, Transform parent)
        {
            GameObject flagInstance = GameObject.Instantiate(_prefabRepository.FlagPrefab, position, Quaternion.identity);
            flagInstance.transform.localScale = Vector3.zero;
            flagInstance.transform.parent = parent;
            flagInstance.transform.DOScale(Vector3.one, _prefabRepository.FlagSpawnAnimationConfig.Duration).SetEase(_prefabRepository.FlagSpawnAnimationConfig.Ease);
        }

        private void DetermineHitZone(float distance)
        {
            // Учитываем скейл мишени при проверке зон
            float scaleFactor = _targetHitView.transform.lossyScale.x; // Предполагаем, что мишень равномерно скейлится

            var result = ETargetHitResult.Miss;
            if (distance <= _targetHitView.InnerZoneRadius * scaleFactor)
            {
                result = ETargetHitResult.InnerZone;
            }
            else if (distance <= _targetHitView.MiddleZoneRadius * scaleFactor)
            {
                result = ETargetHitResult.MiddleZone;
            }
            else if (distance <= _targetHitView.OuterZoneRadius * scaleFactor)
            {
                result = ETargetHitResult.OuterZone;
            }
            OnPlayerHitTarget?.Invoke(result);
            Debug.Log($"Hit detected in zone: {result}");
        }

        public void Clear()
        {
            _isHit = false;
            _targetHitView = null;
        }
        public void Dispose()
        {
            OnPlayerHitTarget = null;
            _disposable.Dispose();
        }
    }

    public enum ETargetHitResult
    {
        Miss,
        InnerZone,
        MiddleZone,
        OuterZone,
    }
}