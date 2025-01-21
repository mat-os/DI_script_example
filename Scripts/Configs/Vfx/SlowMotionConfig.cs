using System;
using Game.Scripts.Utils;
using RotaryHeart.Lib.SerializableDictionary;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.Configs.Vfx
{
    [CreateAssetMenu(fileName = nameof(SlowMotionConfig), menuName = "Configs/" + nameof(SlowMotionConfig), order = 1)]
    [InlineEditor]
    public class SlowMotionConfig : ScriptableObject
    {
        [field:SerializeField] public bool IsUseSlowMotion {get;private set;}
        
        [field:SerializeField] public SerializableDictionaryBase<ESlowMotionType, SlowMotionSettings> SlowMotionSettings {get;private set;}
        
        [field:Header("On Fly")]
        [field:SerializeField] public float SlowMoActivateDamageThresholds {get;private set;}
        [field:SerializeField] public float DelayBetweenSlowMo {get;private set;}
    }

    [Serializable]
    public class SlowMotionSettings
    {
        [field:SerializeField] public float SlowMotionTimeScale {get;private set;}
        [field:SerializeField] public float DurationOfSlowMo {get;private set;}
        [field:SerializeField] public AnimationParameterConfig SlowMotionEnableConfig {get;private set;}
        [field:SerializeField] public AnimationParameterConfig SlowMotionDisableConfig {get;private set;}
    }
    public enum ESlowMotionType
    {
        HitWall,
        PlayerGetDamage,
        FlightChangeDirection,
        HoldToRideTutorial,
        SimpleJumpTutorial,
        JumpTutorial,
        TargetHit,
        TapMiniGame,
        PlayerBreakBone
    }
}