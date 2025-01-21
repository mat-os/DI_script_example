using System;
using Game.Scripts.UI.Screens;
using Game.Scripts.UI.Widgets;
using Game.Scripts.Utils;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "UIConfig", menuName = "Configs/UI Config", order = 1)]
    public class UIConfig : ScriptableObject
    {
        [field: Header("Main Menu")]
        [field:SerializeField] public LevelPackUI LevelPackUI{ get; private set; }
        [field: SerializeField] public GarageView GarageView { get; private set; }
        
        [field: Header("Timings")]
        [field:SerializeField] public float DelayBeforeLevelFailedScreen { get; private set; }
        [field:SerializeField] public float DelayBeforeLevelWinScreen { get; private set; }
        [field:SerializeField]public AnimationParameterConfig OpenLevelCompleteUIAnimationParameters { get; private set; }
        [field:SerializeField] public float LevelCompleteScreenHideDelay { get; private set; }

        [field: Header("Stars Missions")]
        [field:SerializeField] public Sprite ActiveStar { get; private set; }
        [field:SerializeField] public Sprite InactiveStar { get; private set; }
        [field:SerializeField] public float ActiveStarScale { get; private set; }
        [field:SerializeField] public AnimationParameterConfig ShowStarScaleAnimation { get; private set; }
        
        [field: Header("Upgrade Buttons")]
        [field:SerializeField] public int LevelToShowUpgradeButtons { get; private set; }  
        
        [field: Header("Progress Bar Fill")]
        [field:SerializeField] public AnimationParameterConfig FinalResultProgressBarFillAnimation { get; private set; }

        [field: Header("Trophy")]
        [field:SerializeField] public Sprite ActiveTrophy { get; private set; }
        [field:SerializeField] public Sprite InactiveTrophy { get; private set; }
        [field:SerializeField] public float TrophyDisplayDuration { get; set; }

        //[field: Header("Main Settings")]
        //[field:SerializeField]public int ShowShopLevelIndex { get; private set; }
        //[field:SerializeField]public int OpenShopLevelIndex { get; private set; }
        //[field:SerializeField]public int OpenUIUpgradeMenuLevelIndex { get; private set; }
        
        //[field: Header("Daily Reward")]
        //[field:SerializeField]public bool IsShowDailyReward { get; private set; }
        //[field:SerializeField] public int FirstDailyRewardShowLevel { get; private set; }
        
        //[field:Header("Key Panel")]
        //[field:SerializeField] public float DurationOfShowKeyPanel { get; private set; }
    }
}