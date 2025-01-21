using System;
using System.Collections.Generic;
using Game.Scripts.LevelElements;
using RootMotion.Dynamics;
using RotaryHeart.Lib.SerializableDictionary;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.Configs
{
    [InlineEditor]
    [CreateAssetMenu(fileName = nameof(ScoreConfig), menuName = "Configs/" + nameof(ScoreConfig))]
    public class ScoreConfig : ScriptableObject
    {
        [field:Header("Score")]
        [field:SerializeField]public SerializableDictionaryBase<EScoreType, int> ScoreByType { get; private set; }
        //[field:SerializeField]public SerializableDictionaryBase<LevelPhysicGameObject, EScoreType> ObjectsByScore { get; private set; }
        [field:SerializeField] public SerializableDictionaryBase<Muscle.Group, int> ScoreForBreakingBones {get;private set;}

        [field:Header("Score Multiplier")]
        [field:SerializeField]public int ObjectDestroyScoreMultiplier { get; private set; }
        [field:SerializeField]public int PlayerDamageScoreMultiplier { get; private set; }
        
        [field:Header("Air Time")]
        [field:SerializeField]public float AirTimeScoreInterval { get; private set; }
        [field:SerializeField]public int AirTimeScoreToAdd { get; private set; }

        [field:Header("Combo")]
        [field:ListDrawerSettings(ShowIndexLabels = true)]
        [field:SerializeField]public List<ComboSettings> ComboSettings { get; private set; }
    }
    [Serializable]
    public class ComboSettings
    {
        public int ComboThreshold;
        public Gradient ComboUIColor;
    }
    public enum EScoreType
    {
        Cheap,
        Medium,
        Expensive,
        None
    }
}