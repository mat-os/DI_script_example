using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Scripts.Configs;
using Game.Scripts.Configs.Vfx;
using Game.Scripts.LevelElements.Collisions;
using Game.Scripts.Utils;
using UniRx;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Game.Scripts.Infrastructure.Services.Level
{
    public class PeopleHitMissionService : IDisposable
    {
        public Action<int> OnHitPeople;

        private readonly VfxEffectsService _vfxEffectsService;
        
        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        private int _hitPeopleCount = 0;

        private  int _currentEffectIndex = 0;

        [Inject]
        public PeopleHitMissionService(VfxEffectsService vfxEffectsService)
        {
            _vfxEffectsService = vfxEffectsService;
            GlobalEventSystem.Broker.Receive<HitPeopleEvent>()
                .Subscribe(HitPeopleEventHandler)
                .AddTo(_disposable);
        }

        private void HitPeopleEventHandler(HitPeopleEvent hitPeopleEvent)
        {
            _hitPeopleCount++;
            OnHitPeople?.Invoke(_hitPeopleCount);
            var randomFx = GetNextNpcHitEffect();
            _vfxEffectsService.SpawnEffect(randomFx, hitPeopleEvent.VfxPosition);
        }

        private VfxEffectType GetNextNpcHitEffect()
        {
            // Получаем текущий эффект
            var nextEffect = NpcHitEffects[_currentEffectIndex];
            // Увеличиваем индекс, сбрасывая его в начало при достижении конца массива
            _currentEffectIndex = (_currentEffectIndex + 1) % NpcHitEffects.Length;
            return nextEffect;
        }
        private static readonly VfxEffectType[] NpcHitEffects =
        {
            VfxEffectType.HitNpcBang,
            VfxEffectType.HitNpcBlam,
            VfxEffectType.HitNpcPow,
            VfxEffectType.HitNpcWtf
        };

        public void Clear()
        {
            _hitPeopleCount = 0;
        }
        public void Dispose()
        {
            OnHitPeople = null;
            _disposable.Dispose();
        }
    }
}