using System.Collections.Generic;
using Configs;
using Game.Scripts.Infrastructure.Services.Level;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.Configs.Level
{
    [CreateAssetMenu(fileName = nameof(LevelPackConfig), menuName = "Configs/Level/" + nameof(LevelPackConfig))]
    [InlineEditor]
    public class LevelPackConfig : ScriptableObject
    {
        [field: SerializeField] public ELevelPackId PackId;
        [field:SerializeField]public string Name { get; private set; }
        [field:SerializeField]public Sprite Image { get; private set; }
        
        [field:ListDrawerSettings(ShowItemCount = true)]
        [field: SerializeField] public List<LevelDataConfig> LevelDataConfigs { get; private set; }
        
        public int GetPackId()
        {
            return (int)PackId;
        }
    }
}