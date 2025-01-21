using System;
using DG.Tweening;
using Game.Scripts.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "CamerasConfig", menuName = "Configs/Cameras Config", order = 1)]
    [InlineEditor()]
    public class CamerasConfig : ScriptableObject
    {
        [field:Header("Gameplay Camera")]
        [field:SerializeField]public float MinGameplayCameraDistance { get; private set; }
        [field:SerializeField]public float MinGameplayCameraHeight { get; private set; }
        [field:SerializeField]public float MaxGameplayCameraDistance { get; private set; }
        [field:SerializeField]public float MaxGameplayCameraHeight { get; private set; }
        [field:SerializeField]public float ChangeSpeed { get; private set; }
        
        [field:Header("Fly Camera")]
        [field:SerializeField]public float MinFlyCameraDistance { get; private set; }
        [field:SerializeField]public float MinFlyCameraHeight { get; private set; }
        [field:SerializeField]public float MaxFlyCameraDistance { get; private set; }
        [field:SerializeField]public float MaxFlyCameraHeight { get; private set; }
        [field:SerializeField]public float FlyCameraChangeSpeed { get; private set; }
        [field:SerializeField]public float MaxPlayerSpeed { get; private set; }
        
        [field:Header("Gameplay Camera Noise")]
        [field:SerializeField]public float MinCameraNoiseAmplitude { get; private set; }
        [field:SerializeField]public float MaxCameraNoiseAmplitude { get; private set; }
        [field:SerializeField]public float MinCameraNoiseFrequencyGain { get; private set; }
        [field:SerializeField]public float MaxCameraNoiseFrequencyGain { get; private set; }
        [field:SerializeField]public AnimationCurve NoiseAmplitudeAnimationCurve { get; private set; }
        
        [field:Header("Gameplay Camera Fov On Boost")]
        [field:SerializeField]public float FovOnBoostMultiplyer { get; private set; }
        [field:SerializeField]public AnimationParameterConfig EnterBoostFovAnimation { get; private set; }
        [field:SerializeField]public AnimationParameterConfig ExitBoostFovAnimation { get; private set; }
    }
}