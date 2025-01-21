using System;
using Configs;
using DG.Tweening;
using Game.Scripts.Configs.Vfx;
using Game.Scripts.Infrastructure.Services.Car;
using Game.Scripts.LevelElements;
using Game.Scripts.LevelElements.Triggers;
using Game.Scripts.Utils.Debug;
using PG;
using UniRx;
using UnityEngine;

namespace Game.Scripts.Infrastructure.Services.Player
{
    public class SlowMotionService : IDisposable
    {
        public Action OnSlowMotionStarted;
        public Action OnSlowMotionStopped;
        
        private readonly PlayerFlightLaunchService _playerFlightLaunchService;
        private readonly SlowMotionConfig _slowMotionConfig;

        private bool _isSlowMoActive;
        private Tween _timeScaleTween;

        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        private float _timeForNextSlowMotion;
        
        private readonly float _defaultTimeScale;
        private readonly float _defaultFixedTimeScale;
        private Tween _delayedCall;

        public SlowMotionService(GameConfig gameConfig, PlayerFlightLaunchService playerFlightLaunchService)
        {
            _playerFlightLaunchService = playerFlightLaunchService;
            _slowMotionConfig = gameConfig.SlowMotionConfig;

            _playerFlightLaunchService.OnPlayerFlyStart += OnPlayerFlyStartHandler;
            
            GlobalEventSystem.Broker.Receive<PlayerBodyPartTakeDamageEvent>()
                .Subscribe(PlayerBodyPartTakeDamageHandler)
                .AddTo(_disposable);

            GlobalEventSystem.Broker.Receive<TapGameStartEvent>()
                .Subscribe(TapGameStartEventHandler)
                .AddTo(_disposable);   
            GlobalEventSystem.Broker.Receive<TapGameEndEvent>()
                .Subscribe(TapGameEndEventHandler)
                .AddTo(_disposable);         
            GlobalEventSystem.Broker.Receive<PlayerEnterSlowMotionTriggerEvent>()
                .Subscribe(PlayerEnterSlowMotionTriggerEventHandler)
                .AddTo(_disposable);
            
            _defaultTimeScale = Time.timeScale;
            _defaultFixedTimeScale = Time.fixedDeltaTime;
        }

        private void PlayerBodyPartTakeDamageHandler(PlayerBodyPartTakeDamageEvent damageEventEvent)
        {
            if (damageEventEvent.Damage >= _slowMotionConfig.SlowMoActivateDamageThresholds && 
                Time.time >= _timeForNextSlowMotion)
            {
                _timeForNextSlowMotion = Time.time + _slowMotionConfig.DelayBetweenSlowMo;
                
                StartSlowMo(ESlowMotionType.PlayerGetDamage, true);
            }
        }       
        private void TapGameStartEventHandler(TapGameStartEvent tapGameStartEvent)
        {
            StartSlowMo(ESlowMotionType.TapMiniGame);
        }      
        private void TapGameEndEventHandler(TapGameEndEvent tapGameEndEvent)
        {
            StopSlowMo(ESlowMotionType.TapMiniGame);
        }
        
        private void OnPlayerFlyStartHandler()
        {
            if(_slowMotionConfig.IsUseSlowMotion == false)
                return;
            StopSlowMo(ESlowMotionType.HitWall);
        }
        
        
        private void PlayerEnterSlowMotionTriggerEventHandler(PlayerEnterSlowMotionTriggerEvent slowMotionTriggerEvent)
        {
            CustomDebugLog.Log("[Slow Mo] PlayerEnterSlowMotionTriggerEvent " + slowMotionTriggerEvent.SlowMotionType);
            switch (_isSlowMoActive)
            {
                case true when slowMotionTriggerEvent.IsStart == false:
                    StopSlowMo(slowMotionTriggerEvent.SlowMotionType);
                    break;
                case false when slowMotionTriggerEvent.IsStart:
                    StartSlowMo(slowMotionTriggerEvent.SlowMotionType);
                    break;
            }
        }

        public void StartSlowMo(ESlowMotionType slowMotionType, bool isHasDuration = false)
        {
            if (_isSlowMoActive)
                return;
            if (_slowMotionConfig.IsUseSlowMotion == false)
                return;

            CustomDebugLog.Log("[Slow Mo] StartSlowMo " + slowMotionType);
            var settings = _slowMotionConfig.SlowMotionSettings[slowMotionType];
            var enableConfig = settings.SlowMotionEnableConfig;
            
            _isSlowMoActive = true;
            OnSlowMotionStarted?.Invoke();

            _timeScaleTween?.Kill();
            _timeScaleTween = DOTween.To(() => Time.timeScale, x => Time.timeScale = x, settings.SlowMotionTimeScale, enableConfig.Duration)
                .OnUpdate(() =>
                {
                    Time.fixedDeltaTime = Time.timeScale * 0.02f;
                })
                .SetEase(enableConfig.Ease)
                .SetUpdate(true);

            if (isHasDuration)
            {
                _delayedCall = DOVirtual.DelayedCall(settings.DurationOfSlowMo, 
                    () => StopSlowMo(slowMotionType));
            }
        }

        public void StopSlowMo(ESlowMotionType slowMotionType)
        {
            if (!_isSlowMoActive)
                return;
            if (_slowMotionConfig.IsUseSlowMotion == false)
                return;

            CustomDebugLog.Log("[Slow Mo] StopSlowMo " + slowMotionType);

            _isSlowMoActive = false;
            OnSlowMotionStopped?.Invoke();

            var slowMotionDisableConfig = _slowMotionConfig.SlowMotionSettings[slowMotionType].SlowMotionDisableConfig;

            _timeScaleTween?.Kill();
            _timeScaleTween = DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1f, slowMotionDisableConfig.Duration)
                .OnUpdate(() =>
                {
                    Time.fixedDeltaTime = Time.timeScale * 0.02f;
                })
                .SetEase(slowMotionDisableConfig.Ease)
                .OnComplete(() =>
                {
                    Time.timeScale = 1f;
                    Time.fixedDeltaTime = 0.02f; 
                })
                .SetUpdate(true);
        }
        public void StopAllSlowMo()
        {
            if (_slowMotionConfig.IsUseSlowMotion == false)
                return;
            if (_isSlowMoActive)
            {
                _isSlowMoActive = false;
                _timeScaleTween?.Kill();
                _delayedCall?.Kill();
                Time.timeScale = _defaultTimeScale;
                Time.fixedDeltaTime = _defaultFixedTimeScale;
            }
        }
        public void Clear()
        {
            _timeScaleTween?.Kill();
            _delayedCall?.Kill();
            _isSlowMoActive = false;
            
            Time.timeScale = _defaultTimeScale;
            Time.fixedDeltaTime = _defaultFixedTimeScale;
        }
        public void Dispose()
        {
            _disposable.Dispose();
        }


    }
}