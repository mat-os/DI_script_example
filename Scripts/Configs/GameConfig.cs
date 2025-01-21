using System;
using System.Collections.Generic;
using Game.Scripts.Configs;
using Game.Scripts.Configs.Dialogue;
using Game.Scripts.Configs.Level;
using Game.Scripts.Configs.Vfx;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Configs/Game Config", order = 0)]
    public class GameConfig : ScriptableObject
    {
        [field:Header("Player")]
        [field:SerializeField]public PlayerConfig PlayerConfig { get; private set; }
        [field:SerializeField]public SlowMotionConfig SlowMotionConfig { get; private set; }
        [field:SerializeField]public UpgradesConfig UpgradesConfig { get; private set; }
        [field:SerializeField]public TutorialConfig TutorialConfig { get; private set; }
        
        [field:Header("Camera")]
        [field:SerializeField]public CamerasConfig CamerasConfig { get; private set; }

        [field:Header("Dialogue")]
        [field:SerializeField]public FungusConfig FungusConfig { get; private set; }
        
        [field:Header("Missions")]
        [field:SerializeField]public GlobalMissionsConfig GlobalMissionsConfig { get; private set; }
        [field:SerializeField]public WeeklyMissionsConfig WeeklyMissionsConfig { get; private set; }
        
        [field:Header("Reward")]
        [field:SerializeField]public RewardConfig RewardConfig { get; private set; }
        [field:SerializeField]public ScoreConfig ScoreConfig { get; private set; }
        [field:SerializeField] public ReviewConfig ReviewConfig { get; set; }

        [field:Header("Analytic")]
        [field:SerializeField] public string AppMetricaKey { get; private set; }

        
    }
}