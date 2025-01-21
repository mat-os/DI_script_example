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
    [CreateAssetMenu(fileName = nameof(WeeklyMissionsConfig), menuName = "Configs/Missions/" + nameof(WeeklyMissionsConfig))]
    public class WeeklyMissionsConfig : ScriptableObject
    {
        [field:SerializeField]public List<LevelMission> WeeklyMissions { get; private set; }
    }
}