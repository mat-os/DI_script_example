using System.Collections.Generic;
using Game.Scripts.Configs.Level;
using Game.Scripts.LevelElements.Car;
using RotaryHeart.Lib.SerializableDictionary;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = nameof(MissionRepository), menuName = "Missions/" + nameof(MissionRepository), order = 0)]
    public class MissionRepository: ScriptableObject
    {
        [field: SerializeField] public SerializableDictionaryBase<EMissionType, MissionConfig> MissionSettings { get; private set; }
    }
}