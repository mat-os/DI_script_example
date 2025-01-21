using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.Configs.Player
{
    [CreateAssetMenu(fileName = nameof(CarMovementConfig), menuName = "Configs/" + nameof(CarMovementConfig))]
    [InlineEditor]
    public class CarMovementConfig  : ScriptableObject
    {
        [field:Header("Steering")]
        [field:SerializeField] public float SteeringSensitivity {get;private set;}  
        [field: SerializeField] public float InputSteeringSensitivity { get; private set; } = 5f; 

        [field:Header("Rotation")]
        [field:SerializeField] public float RotationSensitivityLerpSpeed {get;private set;}  
        [field:SerializeField] public float RotationLerpSpeed {get;private set;}  
        [field:SerializeField] public float RotationAngle {get;private set;}  
        //[field:SerializeField] public AnimationCurve RotationCurve {get;private set;}  
        [field:SerializeField] public float MaxNormalizedSteerInput {get;private set;}
        
        [field:Header("Rotation")]
        [field:SerializeField] public float MaxRotationAngle {get;private set;}
        [field:SerializeField] public float RotationInputSmoothingSpeed {get;private set;}
        [field:SerializeField] public float RotationSpeed {get;private set;}
        [field:SerializeField] public float ReturnRotationSpeed {get;private set;}
        
        [field:Header("Rotation Inertia")]
        [field:SerializeField] public float SteeringResponsiveness {get;private set;}
        [field:SerializeField] public float InertiaDecaySpeed {get;private set;}
        public AnimationCurve SteeringResponsivenessCurveMultiplier; // Кривая зависимости SteeringResponsiveness от скорости
        public AnimationCurve InertiaDecaySpeedCurveMultiplier;      // Кривая зависимости InertiaDecaySpeed от скорости
        
        [field:Header("Forward Movement")]
        [field:SerializeField] public float Acceleration {get;private set;}     
        [field:SerializeField] public AnimationCurve AccelerationCurve {get;private set;}     
        [field:SerializeField] public float MaxSpeedKmh {get;private set;}  
        [field:SerializeField] public float InclineAdjustmentSpeed {get;private set;}  
        
        [field: Header("Braking")]
        [field: SerializeField] public float BrakeDeceleration { get; private set; } 
        [field: SerializeField] public AnimationCurve BrakeCurve { get; private set; } 
        
        [field:Header("Gravity")]
        [field:SerializeField] public float GravityForce {get;private set;}  
        [field:SerializeField] public float GroundRaycastCheckDistance {get;private set;}  
        [field:SerializeField] public float GroundRaycastUpOffset {get;private set;}  
        
        [field:Header("Wheels")]
        [field:SerializeField] public float WheelRotationSpeed {get;private set;}  
        [field:SerializeField] public float WheelMaxSteerAngle {get;private set;}  
        [field:SerializeField] public float WheelSteerLerpSpeed {get;private set;}  
        
        [field:Header("Boost")]
        [field: SerializeField] public float BoostDuration { get; private set; }
        [field: SerializeField] public float BoostBaseMultiplier { get; private set; }
        [field: SerializeField] public AnimationCurve BoostIntensityCurve { get; private set; }
        [field: SerializeField] public float ImmediateBoostSpeedAmount { get; private set; }

        [field:Header("Other")]
        [field:SerializeField] public float RoadWidth {get;private set;}  


        public float MaxSpeedMs => MaxSpeedKmh / 3.6f;
    }
}