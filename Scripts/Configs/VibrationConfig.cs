using System;
using System.Collections.Generic;
using MoreMountains.NiceVibrations;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = nameof(VibrationConfig), menuName = "Configs/" + nameof(VibrationConfig))]

    public class VibrationConfig : ScriptableObject
    {
        [ListDrawerSettings(ShowIndexLabels = true)]
        [field:SerializeField] public List<VibrationPlace> VibrationPlaces;
    }
    
    [Serializable]
    public class VibrationPlace
    {
        public VibrationPlaceType VibrationPlaceType;
        public HapticTypes HapticTypes;
        public float MinDelayBetweenHaptics;
    }

    public enum VibrationPlaceType
    {
        None = 0,
        VibrationEnabled = 1,
        BreakBone = 2, 
        CurrencyEffectFinishedSpread = 3,
        BuyUpgrade = 4,
        CurrencyEffectReachedTarget = 5,
        DestroyNpcCar = 6,
        HitWall = 7,
        BumpIntoPeople = 8,
        DestroyObject = 9,
        PlayerJump = 10,
        CarEnterBooster = 11,
    }
}