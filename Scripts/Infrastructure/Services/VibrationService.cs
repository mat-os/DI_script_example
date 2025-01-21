using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Configs;
using Game.Scripts.Infrastructure.Bootstrapper;
using Game.Scripts.Utils.Debug;
using MoreMountains.NiceVibrations;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Infrastructure.Services
{
    public class VibrationService : IInitializable, IDisposable
    {

        private readonly VibrationConfig _vibrationConfig;
        private readonly DataService _dataService;
        
        private readonly Dictionary<VibrationPlaceType, DateTime> _lastCallTimeByType;
        private DateTime _lastCallTime;

        public bool IsVibrationEnabled => _dataService.GameSettings.IsVibrationEnabled.Value == true;
        
        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        
        [Inject]
        public VibrationService(VibrationConfig vibrationConfig, DataService dataService)
        {
            _dataService = dataService;
            _lastCallTimeByType = new Dictionary<VibrationPlaceType, DateTime>();
            _vibrationConfig = vibrationConfig;
            
            GlobalEventSystem.Broker.Receive<CarNPCDestroyedEvent>()
                .Subscribe(CarNpcDestroyedHandler)
                .AddTo(_disposable);
            GlobalEventSystem.Broker.Receive<HitPeopleEvent>()
                .Subscribe(BumpPeopleEventHandler)
                .AddTo(_disposable);
            GlobalEventSystem.Broker.Receive<DestroyObjectEvent>()
                .Subscribe(DestroyObjectEventHandler)
                .AddTo(_disposable);
            GlobalEventSystem.Broker.Receive<PlayerCarEnterBoosterZoneEvent>()
                .Subscribe(PlayerCarEnterBoosterZoneEventHandler)
                .AddTo(_disposable);
        }

        private void CarNpcDestroyedHandler(CarNPCDestroyedEvent carNpcDestroyedEvent)
        {
            Vibrate(VibrationPlaceType.DestroyNpcCar);
        }           
        private void PlayerCarEnterBoosterZoneEventHandler(PlayerCarEnterBoosterZoneEvent enterBooster)
        {
            Vibrate(VibrationPlaceType.CarEnterBooster);
        }        
        private void BumpPeopleEventHandler(HitPeopleEvent hitPeopleEvent)
        {
            Vibrate(VibrationPlaceType.BumpIntoPeople);
        }    
        private void DestroyObjectEventHandler(DestroyObjectEvent destroyObjectEvent)
        {
            Vibrate(VibrationPlaceType.DestroyObject);
        }

        public void Initialize()
        {
            foreach (var place in _vibrationConfig.VibrationPlaces)
            {
                _lastCallTimeByType.Add(place.VibrationPlaceType, DateTime.Now);
            }
        }

        public void Vibrate(VibrationPlaceType place)
        {
            if (IsVibrationEnabled == false)
                return;
            
            VibrationPlace vibrationCase = GetHapticType(place);
            if (vibrationCase == null)
            {
                CustomDebugLog.LogError($"[VIBRO] Vibration place type {place} is not supported");
                return;
            }
            if ((DateTime.Now - _lastCallTimeByType[vibrationCase.VibrationPlaceType]).Milliseconds < vibrationCase.MinDelayBetweenHaptics) 
                return;
            
            CustomDebugLog.Log($"[VIBRO] {place} vibrating");
                
            #if UNITY_IOS
            
            MMVibrationManager.Haptic(vibrationCase.HapticTypes);
            
            #elif UNITY_ANDROID

            MMVibrationManager.Haptic(vibrationCase.HapticTypes);

            #endif
            
            _lastCallTimeByType[vibrationCase.VibrationPlaceType] = DateTime.Now;
        }
        
        private VibrationPlace GetHapticType(VibrationPlaceType type) =>
            _vibrationConfig.VibrationPlaces.FirstOrDefault(place => place.VibrationPlaceType == type);

        public void Dispose()
        {
            _disposable?.Dispose();
        }
    }
}