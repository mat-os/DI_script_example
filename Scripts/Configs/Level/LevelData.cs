using System;
using System.Collections.Generic;
using Game.Scripts.Configs.Dialogue;
using Game.Scripts.Configs.Level;
using Game.Scripts.Customization;
using Game.Scripts.Customization.ItemConfigs;
using Game.Scripts.LevelElements;
using PG;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = nameof(LevelDataConfig), menuName = "Configs/Level/" + nameof(LevelDataConfig))]
    [InlineEditor]
    public class LevelDataConfig : ScriptableObject
    {
        [field:AssetList(Path = "Game/Prefab/Levels")]
        [field:AssetSelector]
        [field:GUIColor(0,1,0)]
        [field: SerializeField] public LevelView Level { get; private set; }
        [field: SerializeField] public string LevelName { get; private set; }
        
        [field:Header("Player Settings")]
        [field: SerializeField] public float RoadWidth { get; private set; } = 4;
        [field: SerializeField] public bool IsSetCustomCarCustomization { get; private set; }
        [field:ShowIf("IsSetCustomCarCustomization")]
        [field: SerializeField] public ShopCarCarColorItemConfig CustomCarColorItemConfig { get; private set; }

        [field:Header("Missions")]
        [field:ListDrawerSettings(ShowIndexLabels = true)]
        [field: SerializeField]public List<LevelMission> LevelMissions { get; private set; }
        //[field: SerializeField]public MissionOnLevel MissionOnLevel { get; private set; }
        
        [field:Header("Environment")] 
        [field: SerializeField]public EnvironmentConfig EnvironmentConfig{ get; private set; }
        
        [field:Header("Dialogue")]
        [field: SerializeField]public DialogueConfig DialogueConfig{ get; private set; }
        [field: SerializeField]public DialogueConfig FinishLevelDialogueConfig{ get; private set; }
        
        [field:Header("Difficulty Settings")]
        [field: SerializeField]public DifficultySettings DifficultySettings{ get; private set; }
        
        [field:Header("Special Settings")]
        [field: SerializeField]public bool IsUseSpecialLevelData { get; private set; }
        [field:ShowIf("IsUseSpecialLevelData")]
        [field: SerializeField]public SpecialLevelDataSettings SpecialLevelDataSettings{ get; private set; }

        [field: Header("Trophy Probability For UI")]
        [field: MinMaxSlider(0, 100)]
        [field: SerializeField] public Vector2 TrophyProbabilityForCollect { get; private set; } = new Vector2(3, 10);
        [field: Header("Review")]
        [field: SerializeField] public bool IsNeedReviewOnThisLevel { get; private set; }
        [field: Header("Image")]
        [field: SerializeField]public Sprite LevelImage { get; private set; } 

        /*private void OnValidate()
        {
            if (LevelMissions == null || LevelMissions.Count != 3)
            {
                LevelMissions = new List<LevelMission> { new LevelMission(), new LevelMission(), new LevelMission() };
            }
        }*/
    }

    [Serializable]
    public class LevelMission
    {
        public EMissionType EMissionType;
        public int TargetValue;
        public int Reward;
    } 

    /*[Serializable]
    public class MissionOnLevel
    {
        public EMissionType MissionType;
        public MissionReward[] MissionTargetReward;
    } */
    /*[Serializable]
    public class MissionReward
    {
        public int TargetValue;
        public int Reward;
    } */
    [Serializable]
    public class SpecialLevelDataSettings
    {
        [Header("Upgrade")]
        public bool IsForceSetUpgrades = false;
        [ShowIf("IsForceSetUpgrades")]
        public CarUpgradeStep CarUpgrade;
        [ShowIf("IsForceSetUpgrades")]
        public StuntUpgradeStep StuntUpgrade;

        [Header("Force Set Impulse")]
        public bool IsForceSetImpulseOnFlightLaunch = false;
        [ShowIf("IsForceSetImpulseOnFlightLaunch")]
        public Vector3 ImpulseOnFlightLaunch;
    }

    public enum ECarType
    {
        None = 0,
        S34_Race = 1,
    }
}