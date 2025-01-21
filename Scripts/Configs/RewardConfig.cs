using System;
using System.Collections.Generic;
using Game.Scripts.Configs.Player;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Configs
{
    [InlineEditor]
    [CreateAssetMenu(fileName = nameof(RewardConfig), menuName = "Configs/" + nameof(RewardConfig))]
    public class RewardConfig : ScriptableObject
    {
        [field:SerializeField] public int StartMoneyAmount { get; private set; }
        
        [field:Header("Score Reward")]
        [field:SerializeField]public float RewardForScoreMultiplier { get; private set; }
        [field:SerializeField]public float RewardForScoreOnFailMultiplier { get; private set; }
        
        [field:Header("Praise Config")]
        [field:SerializeField]public List<PraisePhraseConfig>  PraisePhraseConfigs { get; private set; }
    }
}