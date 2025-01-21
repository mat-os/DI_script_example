using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = nameof(UpgradesConfig), menuName = "Configs/" + nameof(UpgradesConfig), order = 0)]
    [InlineEditor]
    public class UpgradesConfig : ScriptableObject
    {
        [TitleGroup("Car Upgrades")]
        public string CarUpgradeDescriptionMessageText;
        [ListDrawerSettings( DraggableItems = true, ShowItemCount = true, ShowIndexLabels = true)]
        public List<UpgradePack<CarUpgradeStep>> CarUpgrades = new List<UpgradePack<CarUpgradeStep>>();
        
        [TitleGroup("Player Humanoid Upgrades")]
        public string StuntUpgradeDescriptionMessageText;
        [ListDrawerSettings( DraggableItems = true, ShowItemCount = true, ShowIndexLabels = true)]
        public List<UpgradePack<StuntUpgradeStep>> StuntUpgrades = new List<UpgradePack<StuntUpgradeStep>>();
        
        [TitleGroup("Income Upgrades")]
        public string IncomeUpgradeDescriptionMessageText;
        [ListDrawerSettings( DraggableItems = true, ShowItemCount = true, ShowIndexLabels = true)]
        public List<UpgradePack<IncomeUpgradeStep>> IncomeUpgrades = new List<UpgradePack<IncomeUpgradeStep>>();
        
    }
    [Serializable]
    public class UpgradePack<T> where T : UpgradeStep
    {
        public List<T> Steps;
    }

    [Serializable]
    public class CarUpgradeStep : UpgradeStep
    {
        [VerticalGroup("Upgrade/Right")]
        [LabelWidth(150)]
        public float CarAccelerationMultiplier;
        [VerticalGroup("Upgrade/Right")]
        [LabelWidth(150)]
        public float MaxCarSpeedMultiplier;
    }   

    [Serializable]
    public class StuntUpgradeStep : UpgradeStep
    {
        [VerticalGroup("Upgrade/Right")]
        [LabelWidth(150)]
        public float FlyForceMultiplier;
        
        [VerticalGroup("Upgrade/Right")]
        [LabelWidth(150)]
        public float MaxEnergyAmountMultiplier = 1;
        
        [VerticalGroup("Upgrade/Right")]
        [LabelWidth(150)]
        public float EnergyRecoveryRateMultiplier = 1;       

    }    
    [Serializable]
    public class IncomeUpgradeStep : UpgradeStep
    {
        [VerticalGroup("Upgrade/Right")]
        [LabelWidth(150)]
        public float EarningMultiplier;
    }
    [Serializable]
    public class UpgradeStep
    {
        [HorizontalGroup("Upgrade")]
        [VerticalGroup("Upgrade/Left")]
        [LabelWidth(100)]
        public int Price; 
    }
    public enum EUpgradeType
    {
        Car,
        Stunt,
        Income
    }
}