using System;
using Game.Scripts.Configs.Player;
using Game.Scripts.Utils;
using RootMotion.Dynamics;
using RotaryHeart.Lib.SerializableDictionary;
using Sirenix.OdinInspector;
using UnityEngine;


    [CreateAssetMenu(fileName = nameof(PlayerConfig), menuName = "Configs/" + nameof(PlayerConfig))]
    [InlineEditor]
    public class PlayerConfig : ScriptableObject
    {
        [field:SerializeField] public PuppetMasterHumanoidConfig CarHumanoidConfig {get;private set;}
        [field:SerializeField] public PuppetMasterHumanoidConfig AirHumanoidConfig {get;private set;}
        
        [field:Header("Car")]
        [field:SerializeField] public CarConfig CarConfig {get;private set;}
        [field:SerializeField] public CarMovementConfig CarMovementConfig {get;private set;}
                
        [field:Header("Fly Start")]
        [field: SerializeField] public FlyStartConfig FlyStartConfig { get; private set; }
        
        [field: Header("Flying End")] 
        [field: SerializeField] public FlyEndConfig FlyEndConfig { get; private set; }
        
        [field: Header("Flying Configurations")]
        [field: SerializeField] public SwipeConfig SwipeConfig { get; private set; }
        [field: SerializeField] public JumpConfig JumpConfig { get; private set; }
        [field: SerializeField] public DragConfig DragConfig { get; private set; }
        [field: SerializeField] public LineConfig LineConfig { get; private set; }
        
        [field: Header("Energy")]
        [field: SerializeField] public EnergyConfig EnergyConfig { get; private set; }
        
        [field: Header("Damage")]
        [field: SerializeField] public DamageConfig DamageConfig { get; private set; }
        
        [field:Header("Swipe Force Limits")]
        //TODO:
        [field: SerializeField]public float MinSwipeForce { get; private set; }
        [field: SerializeField]public float MaxSwipeForce { get; private set; }

        [field:Header("Bones")]
        [field: SerializeField]public PlayerBonesConfig BonesConfig{ get; private set; }

        [field:Header("Tap Game")]
        [field: SerializeField]public TapGameConfig TapGameConfig{ get; private set; }
    }
    [Serializable]
    public class CarConfig 
    {
        [field: SerializeField]public float MaxHealth { get; private set; }
        [field: SerializeField]public float MinInertiaForWheelTrail { get; set; }
    }
    [Serializable]
    public class SwipeConfig
    {
        [field:Header("Swipe")]
        [field: SerializeField]public float MinSwipeDistance { get; private set; }
        [field: SerializeField]public float MaxSwipeDistance { get; private set; }
    }

    [Serializable]
    public class DragConfig
    {
        [field: Header("Drag Sensitivity")]
        [field: SerializeField] public float VerticalSensitivity { get; private set; } = 0.2f;
        [field: SerializeField] public float HorizontalSensitivity { get; private set; } = 0.2f;
        [field: SerializeField] public float ForwardSensitivity { get; private set; } = 0.2f;
        
        [field:Header("Torque")]
        [field: SerializeField]public float TorqueToAddMultiplier { get; private set; }
        
        [field: Header("Drag Decay")]
        [field: Range(0f, 1f)]
        [field: SerializeField] public float DragForceDecayFactor { get; private set; }

        [field: SerializeField]public AnimationCurve DragForceCurve{ get; private set; }
        [field: SerializeField]public float DragDuration { get; private set; }
    }

    [Serializable]
    public class LineConfig
    {
        [field: Header("Drag Line")]
        [field: SerializeField] public float MinDragLineDistance { get; private set; }
        [field: SerializeField] public float MaxDragLineDistance { get; private set; }
    }

    [Serializable]
    public class DamageConfig
    {
        [field:SerializeField] public float DamageMultiplier {get;private set;}
        [field:SerializeField] public SerializableDictionaryBase<Muscle.Group, int> DamageThresholds {get;private set;}
    }

    [Serializable]
    public class FlyEndConfig
    {
        [field:SerializeField]public float InitialFlyCompleteCountDelay { get;private set; }
        [field:SerializeField]public float StopCheckDelay { get; private set; }
        [field:SerializeField]public float SpeedThresholdToEndFly{get;private set;}
        [field:SerializeField]public float FallThresholdToEndFly{get;private set;}
        [field:SerializeField]public float AngularSpeedThresholdToEndFly{get;private set;}
    }
    [Serializable]
    public class FlyStartConfig
    {
        [field:Header("Fly Start")]
        [field:SerializeField] public float FlyForwardForce {get;private set;}
        [field:SerializeField] public float FlyUpForce {get;private set;}
        [field:SerializeField] public float DelayBeforeLaunchPlayer { get;private set; }
        
        [field:Header("Car Force Multiplier")]
        [field: SerializeField] public float MinCollisionSpeed { get; private set; } = 5f;  // Minimum speed for scaling the flight force
        [field: SerializeField] public float MaxCollisionSpeed { get; private set; } = 50f; // Maximum speed for scaling the flight force
        [field: SerializeField] public float MinFlightForce { get; private set; } = 10f;    // Force at minimum collision speed
        [field: SerializeField] public float MaxFlightForce { get; private set; } = 100f;   // Force at maximum collision speed
    }

    [Serializable]
    public class FlightForceConfig
    {
        [field: SerializeField] public float MinCollisionSpeed { get; private set; } = 5f;
        [field: SerializeField] public float MaxCollisionSpeed { get; private set; } = 50f;

        [field: SerializeField] public float MinFlightForce { get; private set; } = 10f;
        [field: SerializeField] public float MaxFlightForce { get; private set; } = 100f;
    }

    [Serializable]
    public class JumpConfig
    {
        [field: SerializeField] public float VerticalSensitivity { get; private set; } = 0.2f;
        [field: SerializeField] public float HorizontalSensitivity { get; private set; } = 0.2f;
        [field: SerializeField] public float ForwardSensitivity { get; private set; } = 0.2f;
        
        /*[field:Header("Swipe Force Limits")]
        [field: SerializeField]public float MinJumpForce { get; private set; }
        [field: SerializeField]public float MaxJumpForce { get; private set; }*/
        
        [field:Header("Cooldown")]
        [field: SerializeField]public float JumpCooldown { get; private set; }

        [field:Header("Limits")]
        [field: SerializeField] public float MaxSpeedX { get; private set; } = 0.2f;
        [field: SerializeField] public float MaxSpeedY { get; private set; } = 0.2f;
        [field: SerializeField] public float MinSpeedY { get; private set; } = 0.2f;

        [field:Header("Forward Force")]
        [field: SerializeField] public AnimationCurve ForwardSpeedToForceCurve { get; private set; }
        [field: SerializeField] public float MaxForceZ { get; private set; } = 0.2f;
        [field: SerializeField] public float MaxSpeedZ { get; private set; } = 0.2f;

        [field:Header("Energy Cost")]
        [field: SerializeField]public float MinJumpEnergyCost { get; private set; }
        [field: SerializeField]public float MaxJumpEnergyCost { get; private set; }
        
        [field:Header("JumpForceDecayFactor")]
        [field:Range(0f,1f)]
        [field: SerializeField]public float JumpForceDecayFactor { get; private set; }
        //[field: SerializeField] public AnimationCurve JumpForceDecayCurve { get; private set; }
    }
    [Serializable]
    public class EnergyConfig
    {
        [field: SerializeField] public float EnergyRecoveryRate { get; private set; } = 0.5f;
        [field: SerializeField] public float MaxEnergy { get; private set; } = 100f;
        [field:Range(0f,1f)]
        [field: SerializeField] public float EnergyDecayFactor { get; private set; } = 0.5f;
    }
    [Serializable]
    public class TapGameConfig
    {
        [field: Header("Game Settings")]
        [field: SerializeField]public float TotalGameTime { get; private set; } = 7f; 
        [field: SerializeField]public float TapIncreaseAmount { get; private set; } = 0.03f; 
        [field: SerializeField]public float DecreaseRate { get; private set; } = 0.01f; 
        [field: SerializeField]public float SuccessThreshold { get; private set; } = 0.9f; 
    }

    [Serializable]
    public class PlayerBonesConfig
    {
        [field: SerializeField]public Gradient BoneColorGradient { get; private set; }
        [field:Range(0f,1f)]
        [field: SerializeField]public float MinDamagePercentageToShowBone { get; private set; }
        
        [Header("Blinking Settings")]
        public Color BlinkStartColor = Color.white;
        public Color BlinkShadingStartColor = Color.white;
        public Color BlinkEndColor = Color.red;
        public Color BlinkShadingEndColor = Color.red;
        public float BlinkDuration = 0.5f; // Длительность одного цикла (в секундах)
        public int BlinkCount = 3; // Количество миганий
    }

