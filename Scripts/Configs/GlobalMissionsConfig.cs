using System.Collections.Generic;
using Configs;
using Fungus;
using Game.Scripts.LevelElements.Car;
using Game.Scripts.LevelElements.Player;
using Game.Scripts.Utils;
using PG;
using RotaryHeart.Lib.SerializableDictionary;
using TMPro;
using UnityEngine;

namespace Game.Scripts.Configs
{
    [CreateAssetMenu(fileName = nameof(GlobalMissionsConfig), menuName = "Configs/Missions/" + nameof(GlobalMissionsConfig))]
    public class GlobalMissionsConfig : ScriptableObject
    {
        [field: SerializeField] public int GlobalMissionsCountOnUI { get; private set; } = 3;
        [field:SerializeField]public List<LevelMission> GlobalMissions { get; private set; }
    }
}