using System.Collections.Generic;
using Configs;
using Game.Scripts.Configs.Vfx;
using Game.Scripts.Infrastructure.Services;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;
using Zenject;

public class GroundParticleSpawnService
{
    private VfxEffectsService _vfxEffectsService;
    
    private readonly SerializableDictionaryBase<PhysicMaterial, VfxEffectType> _groundHitVfxTypes;
    private readonly float _spawnCooldown;
    private readonly float _minImpactForce;
    
    private float _lastSpawnTime;

    [Inject]
    public GroundParticleSpawnService(EffectsConfig effectsConfig, VfxEffectsService vfxEffectsService)
    {
        _vfxEffectsService = vfxEffectsService;
        _groundHitVfxTypes = effectsConfig.GroundHitFxConfig.GroundHitVfxTypes;
        _spawnCooldown = effectsConfig.GroundHitFxConfig.SpawnCooldown;
        _minImpactForce = effectsConfig.GroundHitFxConfig.MinImpactForce;
    }

    public void TrySpawnParticle(Vector3 position, float impactForce, Collision collision)
    {
        if(impactForce < _minImpactForce)
            return;
        
        // Проверяем кулдаун и минимальную силу удара
        if (Time.time - _lastSpawnTime < _spawnCooldown)
            return;
        
        PhysicMaterial material = collision.collider.sharedMaterial;

        if(material == null)
            return;
        
        // Получаем префаб для указанного типа поверхности
        if (_groundHitVfxTypes.TryGetValue(material, out var effectType))
        {
            _vfxEffectsService.SpawnEffect(effectType, position);
            _lastSpawnTime = Time.time; // Обновляем время последнего спавна
        }
        /*else
        {
            Debug.LogWarning($"No particle prefab found for surface type: {material}");
        }*/
    }
}