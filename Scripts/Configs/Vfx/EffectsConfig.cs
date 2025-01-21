using System;
using System.Collections.Generic;
using Game.Configs.PrefabsCollection.PrefabsSettings;
using Game.Scripts.Configs.Vfx;
using Game.Scripts.LevelElements.Car;
using Game.Scripts.Utils;
using LevelElements.Vfx;
using RotaryHeart.Lib.SerializableDictionary;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "EffectsConfig", menuName = "Configs/Effects Config", order = 1)]
    public class EffectsConfig : SerializedScriptableObject
    {
        [Header("Wind Fx")]
        public float MinCarSpeedKmhForWind;

        [Header("Color Grading")]
        public ColorGradingConfig ColorGradingConfig;
        
        [Header("Post Processing Config")]
        public PostProcessingConfig PostProcessingConfig;

        [Header("Player Damage Text")]
        public DamageTextConfig DamageTextConfig;
        public DamageTextConfig ScoreTextConfig;

        [Header("VFXs")]
        [ListDrawerSettings(ShowPaging = true, ShowItemCount = true, ShowFoldout = true)]
        public List<VfxConfig> VFXs = new List<VfxConfig>();

        [Header("Ground Hit Fx")] 
        public GroundHitFxConfig GroundHitFxConfig;
        
        [Header("UI FX")]
        public UIFXSettings UIFXSettings;

    }
    [Serializable]
    public class GroundHitFxConfig
    {
        public SerializableDictionaryBase<PhysicMaterial, VfxEffectType> GroundHitVfxTypes;
        public float SpawnCooldown;
        public float MinImpactForce;

    }
    public enum ESurfaceType
    {
        Dirt,
        Grass,
        Asphalt,
        Concrete,
        Water,
    }
    
    [Serializable]
    public class ColorGradingConfig
    {
        public float VignetteIntensityOnJump;
        public float ContrastOnJump;
        public AnimationParameterConfig ShowVignetteAnimationParameter;
        public AnimationParameterConfig HideVignetteAnimationParameter;
    }  
    [Serializable]
    public class PostProcessingConfig
    {
        public float BloomIntensityOnJump;
        public AnimationParameterConfig ShowBloomAnimationParameter;
        public AnimationParameterConfig HideBloomAnimationParameter;
    }

    [System.Serializable]
    public class VfxConfig
    {
        public VfxEffectType EffectType;
        public GameObject EffectPrefab;
        public int InitialPoolSize = 10;
        public int MaxPoolSize = 50;
    }
    
    [System.Serializable]
    public class DamageTextConfig
    {
        [field: SerializeField] public TextMeshPro DamageTextPrefab { get; private set; } // Префаб TMP текста для отображения урона
        [field: SerializeField] public AnimationParameterConfig ScaleConfig { get; private set; } // Конфигурация для увеличения
        [field: SerializeField] public AnimationParameterConfig MoveUpConfig { get; private set; } // Конфигурация для перемещения вверх
        [field: SerializeField] public AnimationParameterConfig FadeConfig { get; private set; } // Конфигурация для исчезновения
        [field: SerializeField] public Color EndColor { get; private set; } // Конфигурация для исчезновения
        [field: SerializeField] public float MoveUpOffset { get; private set; } = 2.0f; // Смещение вверх
        [field: SerializeField] public float FadeDelay { get; private set; } = 0.5f; // Задержка перед началом исчезновения
        [field: SerializeField]public int DamageTextSpawnThreshold { get; set; }
    }
}