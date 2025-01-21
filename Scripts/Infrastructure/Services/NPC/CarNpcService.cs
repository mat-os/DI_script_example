using System;
using Game.Scripts.Configs.Vfx;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Infrastructure.Services.NPC
{
    public class CarNpcService : IDisposable
    {
        private readonly VfxEffectsService _vfxService;

        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        [Inject]
        private CarNpcService(VfxEffectsService effectsService)
        {
            _vfxService = effectsService;
            
            GlobalEventSystem.Broker.Receive<CarNPCDestroyedEvent>()
                .Subscribe(CarNpcDestroyedHandler)
                .AddTo(_disposable);
        }

        private void CarNpcDestroyedHandler(CarNPCDestroyedEvent carNpcDestroyedEvent)
        {
            SpawnExplosionEffect(carNpcDestroyedEvent.CarNPC.ExplosionSpawnPoint.position);
        }
        private void SpawnExplosionEffect(Vector3 explosionSpawnPoint)
        {
            Debug.Log("[Car NPC] SpawnExplosionEffect!");
            _vfxService.SpawnEffect(VfxEffectType.CarExplosion, explosionSpawnPoint, Quaternion.identity);
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}