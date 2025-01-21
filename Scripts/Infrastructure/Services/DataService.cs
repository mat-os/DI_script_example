using System;
using System.Collections.Generic;
using Configs;
using Game.Scripts.Configs.Level;
using Game.Scripts.Infrastructure.Services.Level;
using Game.Scripts.Utils.Prefs;
using UnityEngine;

namespace Game.Scripts.Infrastructure.Services
{
    public class DataService
    {
        private readonly GameConfig _gameConfig;

        public readonly PurchasesSettings Purchases = new();
        public readonly AdsStats AdsStats = new();
        public readonly Currency Currency = new();
        public readonly Upgrades Upgrades = new();
        
        public readonly GameSettings GameSettings = new();
        public readonly LevelSettings Level = new();
        public readonly MissionsSettings Missions = new();
        public readonly CustomizationSettings CustomizationSettings = new();
        public readonly GiftSettings GiftSettings = new();
        public readonly TutorialData Tutorial = new();
        
        public readonly RateUsData RateUsData = new();
        
        public void Load()
        {
            Level.Load();
            Missions.Load();
            Purchases.Load();
            AdsStats.Load();
            Currency.Load();
            GameSettings.Load();
            CustomizationSettings.Load();
            GiftSettings.Load();
            Upgrades.Load();
            Tutorial.Load();
            RateUsData.Load();
        }
    }
    public class PurchasesSettings
    {
        public readonly PrefsBool IsPurchasesRestored = new();
        public readonly PrefsBool IsNoAdsPurchased = new();

        public void Load()
        {
            IsPurchasesRestored.Load(nameof(IsPurchasesRestored));
            IsNoAdsPurchased.Load(nameof(IsNoAdsPurchased));
        }
    }
    public class AdsStats
    {
        public readonly PrefsInt WatchedIntersCount = new();
        public void Load()
        {
            WatchedIntersCount.Load(nameof(WatchedIntersCount));
        }
    }             
    public class TutorialData
    {
        public readonly PrefsBool IsUpgradeTutorialShowed = new();
        public readonly PrefsBool IsSwipeToJumpTutorialShow = new();
        public void Load()
        {
            IsUpgradeTutorialShowed.Load(nameof(IsUpgradeTutorialShowed));
            IsSwipeToJumpTutorialShow.Load(nameof(IsSwipeToJumpTutorialShow));
        }
    }          
    public class GiftSettings
    {
        public readonly PrefsInt GiftID = new();
        public readonly PrefsInt GiftProgress = new();
        public void Load()
        {
            GiftID.Load(nameof(GiftID));
            GiftProgress.Load(nameof(GiftProgress));
        }
    }        
    public class Upgrades
    {
        public readonly PrefsBool IsShowUpgradeButtons = new();
        
        public readonly PrefsInt CarLevel = new();
        public readonly PrefsInt StuntLevel = new();
        public readonly PrefsInt IncomeLevel = new();
        
        public readonly PrefsInt LastBuyButtonTypeIndex = new();
        public void Load()
        {
            CarLevel.Load(nameof(CarLevel));
            StuntLevel.Load(nameof(StuntLevel));
            IncomeLevel.Load(nameof(IncomeLevel));
            
            IsShowUpgradeButtons.Load(nameof(IsShowUpgradeButtons));
            
            LastBuyButtonTypeIndex.Load(nameof(LastBuyButtonTypeIndex));
        }
    }        
    
    public class CustomizationSettings
    {
        public readonly PrefsString SelectedCarColorName = new();
        public readonly PrefsString SelectedDecalName = new();
        public readonly PrefsString SelectedCarModelName = new();
        public readonly PrefsString PurchasesJSON = new();
        public void Load()
        {
            SelectedCarColorName.Load(nameof(SelectedCarColorName));
            SelectedCarModelName.Load(nameof(SelectedCarModelName));
            SelectedDecalName.Load(nameof(SelectedDecalName));
            PurchasesJSON.Load(nameof(PurchasesJSON));
        }
    }    
    public class LevelSettings
    {
        public PrefsInt LevelIndex = new();
        public PrefsInt LevelPackIndex = new();
        
        public PrefsInt TotalLevelComplete = new();
        
        public PrefsInt AttemptNumber = new();
        public PrefsBool IsCompleteLevelZero = new();
        
        public PrefsJson<LevelProgressList> LevelProgress = new();

        public void Load()
        {
            LevelIndex.Load(nameof(LevelIndex));
            TotalLevelComplete.Load(nameof(TotalLevelComplete));     
            LevelProgress.Load(nameof(LevelProgress));     
            LevelPackIndex.Load(nameof(LevelPackIndex));     
            IsCompleteLevelZero.Load(nameof(IsCompleteLevelZero));     
            AttemptNumber.Load(nameof(AttemptNumber));     
            
            if(TotalLevelComplete.Value == 0)
                TotalLevelComplete.Value = 1;
        }
    }   
    public class MissionsSettings
    {
        public PrefsJson<MissionProgressList> GlobalMissionProgress = new();

        public void Load()
        {
            GlobalMissionProgress.Load(nameof(GlobalMissionProgress));     
        }
    }
    public class Currency
    {
        public readonly PrefsInt Coins = new ();
        public readonly PrefsInt Diamonds = new ();
        public readonly PrefsInt Keys = new ();
        public readonly PrefsInt TotalEarnedStars = new ();
        public void Load()
        {
            Coins.Load(nameof(Coins));
            Diamonds.Load(nameof(Diamonds));
            Keys.Load(nameof(Keys));
            TotalEarnedStars.Load(nameof(TotalEarnedStars));
        }
    }
    public class GameSettings
    {
        public readonly PrefsBool IsVibrationEnabled = new (true, nameof(IsVibrationEnabled));
        public readonly PrefsBool IsMusicEnabled = new(true, nameof(IsMusicEnabled));
        public readonly PrefsBool IsSFXEnabled = new(true, nameof(IsSFXEnabled));
        
        //Используется для Аналитики
        public readonly PrefsBool IsItFirstLaunch = new(true, nameof(IsItFirstLaunch));
        
        public readonly PrefsString RegistrationDate = new();
        
        public void Load()
        {
            IsItFirstLaunch.Load(nameof(IsItFirstLaunch));
            
            if (IsItFirstLaunch.Value == false)
            {
                IsMusicEnabled.Load(nameof(IsMusicEnabled));
                IsSFXEnabled.Load(nameof(IsSFXEnabled));
                IsVibrationEnabled.Load(nameof(IsVibrationEnabled));
                RegistrationDate.Load(nameof(RegistrationDate));
            }
            else
            {
                RegistrationDate.Value = DateTime.Now.ToString("yyyy-MM-dd");
            }
        }
    }
    public class RateUsData
    {
        public readonly PrefsInt RateUsShowCount = new();
        public readonly PrefsInt StarsGainedAmount = new();

        public void Load()
        {
            RateUsShowCount.Load(nameof(RateUsShowCount));
            StarsGainedAmount.Load(nameof(StarsGainedAmount));
        }
    }  
}