using System;
using System.Collections;
using System.Collections.Generic;
using Configs;
using Game.Scripts.Configs.Vfx;
using Game.Scripts.Infrastructure.Bootstrapper;
using LevelElements.Vfx;
using UniRx;
using UnityEngine;
using UnityEngine.Pool;
using Zenject;
using Object = UnityEngine.Object;

namespace Game.Scripts.Infrastructure.Services
{
    public class VfxEffectsService  : MonoBehaviour, IDisposable, IInitializable
    {
        [SerializeField] private Transform _effectPoolRoot;
        
        private EffectsConfig _effectsConfig;
        
        private Dictionary<VfxEffectType, ObjectPool<Vfx>> _effectsPool;
        
        private CoroutineRunner _coroutineRunner => CoroutineRunner.Instance;
        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        
        private ParticleSystem _windFx;

        [Inject]
        public void Construct(EffectsConfig effectsConfig)
        {
            _effectsConfig = effectsConfig;
            
            GlobalEventSystem.Broker.Receive<PlayVfxEvent>()
                .Subscribe(OnPlayVfxEventHandler)
                .AddTo(_disposable);
        }
        public void Initialize()
        {
            _effectsPool = new Dictionary<VfxEffectType, ObjectPool<Vfx>>();

            foreach (var vfxConfig in _effectsConfig.VFXs)
            {
                // Создаем пул
                var pool = new ObjectPool<Vfx>(
                    () => InstantiateEffect(vfxConfig.EffectPrefab),
                    OnActivateEffect,
                    OnDeactivateEffect,
                    OnDestroyEffect,
                    true,
                    vfxConfig.InitialPoolSize,
                    vfxConfig.MaxPoolSize
                );

                // Сохраняем пул в словаре
                _effectsPool[vfxConfig.EffectType] = pool;

                // Предзагружаем объекты в пул
                PreloadPool(pool, vfxConfig.InitialPoolSize);
            }
        }

// Метод для предзагрузки пула
        private void PreloadPool(ObjectPool<Vfx> pool, int count)
        {
            var listOfVfx = new List<Vfx>();
            for (int i = 0; i < count; i++)
            {
                var effect = pool.Get(); // Создаем объект
                listOfVfx.Add(effect);
            }
            for (int i = 0; i < listOfVfx.Count; i++)
            {
                pool.Release(listOfVfx[i]);   // Немедленно возвращаем в пул
            }
            listOfVfx.Clear();
        }
        public void SetWindFx(ParticleSystem windFx)
        {
            _windFx = windFx;
            _isWindFxPlaying = false;
        }
        private void OnPlayVfxEventHandler(PlayVfxEvent playVfxEvent)
        {
            SpawnEffect(playVfxEvent.VfxEffectType, playVfxEvent.Position);
        }

        public IVfx SpawnEffect(VfxEffectType effectType, Vector3 position, Quaternion? rotation = null, Transform parent = null)
        {
            if (_effectsPool.TryGetValue(effectType, out var pool))
            {
                Vfx effect = pool.Get();

                if (parent != null)
                    effect.transform.SetParent(parent);

                effect.transform.position = position;
                effect.transform.rotation = rotation ?? Quaternion.identity;

                _coroutineRunner.StartCoroutine(ReturnToPoolAfterDelay(pool, effect, effect.SecondsToDestroy));
                return effect;
            }

            throw new KeyNotFoundException($"Effect with type {effectType} not found.");
        }

        private Vfx InstantiateEffect(GameObject effectPrefab)
        {
            GameObject instance = Object.Instantiate(effectPrefab, _effectPoolRoot.transform);
            //Debug.Log("InstantiateEffect " + instance.gameObject);
            instance.SetActive(false);
            return instance.GetComponent<Vfx>();
        }

        private void OnActivateEffect(Vfx effect)
        {
            effect.gameObject.SetActive(true);
        }

        private void OnDeactivateEffect(Vfx effect)
        {
            effect.gameObject.SetActive(false);
            effect.transform.SetParent(_effectPoolRoot);
        }

        private IEnumerator ReturnToPoolAfterDelay(ObjectPool<Vfx> pool, Vfx effect, float delay)
        {
            yield return new WaitForSeconds(delay);
            pool.Release(effect);
        }

        private void OnDestroyEffect(Vfx vfx)
        {
            if (vfx != null && vfx.gameObject != null)
            {
                Object.Destroy(vfx.gameObject);
            }
        }
        
        public void Dispose()
        {
            _disposable.Dispose();
            foreach (var pool in _effectsPool.Values)
            {
                pool.Clear();
            }
        }

        private bool _isWindFxPlaying;
        public void HandleCarSpeedChange(float carSpeedKmh)
        {
            if (carSpeedKmh < _effectsConfig.MinCarSpeedKmhForWind && _isWindFxPlaying)
            {
                _isWindFxPlaying = false;
                _windFx.Stop();
            }
            else if (carSpeedKmh > _effectsConfig.MinCarSpeedKmhForWind && _isWindFxPlaying == false)
            {
                _windFx.Play();
                _isWindFxPlaying = true;
            }
        }
    }
}