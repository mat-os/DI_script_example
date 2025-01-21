using System.Collections.Generic;
using Game.Scripts.Configs.Level;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "Level Repository", menuName = "Configs/Level/Level Repository", order = 0)]
    public class LevelsRepository: ScriptableObject
    {
        [field:ListDrawerSettings(ShowItemCount = true)]
        [field: SerializeField] public List<LevelPackConfig> LevelPackConfigs { get; private set; }
        
        [field:Title("Daily Challange")]
        [field: SerializeField] public LevelDataConfig DailyChallenge { get; private set; }
        
        [field:Title("Level Zero")]
        [field: SerializeField] public LevelDataConfig LevelZero { get; private set; }
        [field: SerializeField] public bool IsStartFromLevelZero { get; private set; }
        
        [Space(10)]
        [Title("Debug")]
        public bool IsDebugMode;
        public LevelDataConfig DebugLevelDataConfig;
        
        [Title("Cheats")]
        public bool IsAllLevelsUnlocked;
        
#if !UNITY_EDITOR
        private void OnValidate()
        {
            IsStartFromLevelZero = true;
        }
#endif
    }
}