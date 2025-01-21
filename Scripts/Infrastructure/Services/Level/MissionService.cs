using System;
using System.Collections.Generic;
using Configs;
using Game.Scripts.Configs.Level;
using Game.Scripts.Infrastructure.Services.Level;
using Game.Scripts.Infrastructure.Services.Player;
using Game.Scripts.Utils.Debug;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Infrastructure.Services
{
    public class MissionService :  IDisposable
    {
        public event Action<List<MissionProgress>> OnTotalMissionValueChange;
        
        private List<LevelMission> _activeMissions;
        private List<MissionProgress> _currentMissionProgress; 
        
        private readonly PlayerDamageService _playerDamageService;
        private readonly TargetHitMissionService _targetHitMissionService;
        private readonly ObjectsDestroyMissionService _objectsDestroyMissionService;
        private readonly PlayerScoreService _scoreService;
        private readonly BowlingMissionService _bowlingMissionService;
        private readonly DestroyCarMissionService _destroyCarMissionService;
        private readonly LevelTrophyService _levelTrophyService;
        private readonly PeopleHitMissionService _hitMissionService;
        private readonly LevelsProgressService _levelsProgressService;
        private readonly LevelPackService _levelPackService;
        private readonly LevelDataService _levelDataService;
        private readonly PlayerFlightTrackerService _playerFlightTrackerService;
        
        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        [Inject]
        public MissionService(LevelDataService levelDataService, 
            PlayerFlightTrackerService playerFlightTrackerService,
            PlayerDamageService playerDamageService,
            TargetHitMissionService targetHitMissionService,
            ObjectsDestroyMissionService objectsDestroyMissionService,
            PlayerScoreService scoreService,
            BowlingMissionService bowlingMissionService,
            DestroyCarMissionService destroyCarMissionService,
            LevelTrophyService levelTrophyService,
            PeopleHitMissionService hitMissionService,
            LevelsProgressService levelsProgressService,
            LevelPackService levelPackService)
        {
            _levelPackService = levelPackService;
            _levelsProgressService = levelsProgressService;
            _hitMissionService = hitMissionService;
            _levelTrophyService = levelTrophyService;
            _destroyCarMissionService = destroyCarMissionService;
            _bowlingMissionService = bowlingMissionService;
            _scoreService = scoreService;
            _objectsDestroyMissionService = objectsDestroyMissionService;
            _targetHitMissionService = targetHitMissionService;
            _playerDamageService = playerDamageService;
            _playerFlightTrackerService = playerFlightTrackerService;
            _levelDataService = levelDataService;

            SubscribeEvents();
        }
        
        public void InitializeCurrentLevelMission(LevelDataConfig levelData)
        {
            _activeMissions = new List<LevelMission>(levelData.LevelMissions);
            _currentMissionProgress = new List<MissionProgress>();

            foreach (var mission in _activeMissions)
            {
                _currentMissionProgress.Add(new MissionProgress(mission)); // Привязать прогресс к каждой миссии
            }
            
            var currentProgress = _levelsProgressService.GetLevelProgress(_levelPackService.LevelPackIndex, _levelDataService.LevelId);
            for (int i = 0; i < _currentMissionProgress.Count; i++)
            {
                _currentMissionProgress[i].IsCompleted = currentProgress.MissionProgress[i];
                if (currentProgress.MissionProgress[i])
                    Debug.Log("ALREADY COMPLETE MISSION " + i);
            }
        }

        public List<LevelMission> GetCurrentMissionConfigs()
        {
            if (_activeMissions == null || _activeMissions.Count == 0)
                CustomDebugLog.LogWarning("Mission config is null");

            return _activeMissions;
        }

        public List<MissionProgress> GetCurrentMissionsProgress()
        {
            if (_currentMissionProgress == null || _currentMissionProgress.Count == 0)
                CustomDebugLog.LogWarning("Mission progress is null");

            return _currentMissionProgress;
        }

        public int GetTotalMissionsProgress()
        {
            var totalProgress = 0f;
            foreach (var missionProgress in _currentMissionProgress)
            {
                totalProgress += missionProgress.CurrentValue;
            }
            return (int)totalProgress;
        }
        public int GetMaxMissionsProgress()
        {
            var maxProgress = 0f;
            foreach (var missionProgress in _currentMissionProgress)
            {
                maxProgress += missionProgress.Mission.TargetValue;
            }
            return (int)maxProgress;
        }
        private void PlayerHitTargetHandler(ETargetHitResult targetHitResult)
        {
            switch (targetHitResult)
            {
                case ETargetHitResult.Miss:
                    UpdateMissionProgress(EMissionType.TargetHit, 0);
                    break;
                case ETargetHitResult.InnerZone:
                    UpdateMissionProgress(EMissionType.TargetHit, 3);
                    break;
                case ETargetHitResult.MiddleZone:
                    UpdateMissionProgress(EMissionType.TargetHit, 2);
                    break;
                case ETargetHitResult.OuterZone:
                    UpdateMissionProgress(EMissionType.TargetHit, 1);
                    break;
            }
        }
        
        private void TrophyCollectHandler() => 
            UpdateMissionProgress(EMissionType.CollectTrophy, 1);      
        private void HitPeopleHandler(int count) => 
            UpdateMissionProgress(EMissionType.HitPeople, count);
        private void CarDestroyedHandler(int carsCount) => 
            UpdateMissionProgress(EMissionType.HitCars, carsCount);
        private void OnObjectsDestroyHandler(int destroyedObjectsCount) => 
            UpdateMissionProgress(EMissionType.ObjectsDestroyed, destroyedObjectsCount);
        private void PinFallHandler(int pinsCount) => 
            UpdateMissionProgress(EMissionType.Bowling, pinsCount);
        private void ScoreChangedHandler(int score) => 
            UpdateMissionProgress(EMissionType.EarnScore, score);
        private void ChangeBrokenBonesHandler(int totalBrokenBones) => 
            UpdateMissionProgress(EMissionType.BonesBroken, totalBrokenBones);
        private void ChangeTotalDamageHandler(float totalDamage) => 
            UpdateMissionProgress(EMissionType.DealDamageToPlayer, Mathf.RoundToInt(totalDamage));
        private void FlyDistanceChangeHandler(float distance) => 
            UpdateMissionProgress(EMissionType.Distance, Mathf.RoundToInt(distance));
        
        public void UpdateMissionProgress(EMissionType eMissionType, int value)
        {
            foreach (var progress in _currentMissionProgress)
            {
                // Если миссия соответствует текущему типу
                if (progress.Mission.EMissionType == eMissionType)
                {
                    if (progress.CurrentValue < value && progress.IsCompleted == false)
                    {
                        progress.CurrentValue = value;
                        progress.IsCompleted = IsMissionCompleted(progress.Mission.TargetValue, value);

                        OnTotalMissionValueChange?.Invoke(_currentMissionProgress);

                        /*if (isCompleted)
                        {
                            CustomDebugLog.Log($"Mission {progress.Mission.EMissionType.ToString()} completed!", DebugColor.Orange);
                        }*/
                    }
                }
            }
        }
        private bool IsMissionCompleted(int targetValue, int currentValue)
        {
            return currentValue >= targetValue;
        }
        public int GetCountOfEarnedStarsOnCurrentLevel()
        {
            int totalStars = 0;

            foreach (var progress in _currentMissionProgress)
            {
                var missionValue = progress.CurrentValue;
                var mission = progress.Mission;

                if (missionValue >= mission.TargetValue)
                    totalStars ++;
            }

            return totalStars;
        }
        public bool IsAnyCurrentMissionCompleted()
        {
            foreach (var progress in _currentMissionProgress)
            {
                if (progress.IsCompleted)
                {
                    return true;
                }
            }
            return false;
        }
        public List<MissionProgress> GetActiveMissionsOnCurrentLevel()
        {
            var mission = new List<MissionProgress>();
            foreach (var progress in _currentMissionProgress)
            {
                if (progress.IsCompleted == false)
                {
                    mission.Add(progress);
                }
            }
            return mission;
        }
        private void SubscribeEvents()
        {
            _playerFlightTrackerService.OnFlyDistanceChange += FlyDistanceChangeHandler;
            _playerDamageService.OnChangeTotalDamage += ChangeTotalDamageHandler;
            _playerDamageService.OnChangeBrokenBones += ChangeBrokenBonesHandler;
            _targetHitMissionService.OnPlayerHitTarget += PlayerHitTargetHandler;
            _objectsDestroyMissionService.OnObjectDestroy += OnObjectsDestroyHandler;
            _scoreService.OnScoreChanged += ScoreChangedHandler;
            _bowlingMissionService.OnPinFall += PinFallHandler;
            _destroyCarMissionService.OnCarDestroyed += CarDestroyedHandler;
            _levelTrophyService.OnTrophyCollect += TrophyCollectHandler;
            _hitMissionService.OnHitPeople += HitPeopleHandler;
        }

        private void UnsubscribeEvents()
        {
            _playerFlightTrackerService.OnFlyDistanceChange -= FlyDistanceChangeHandler;
            _playerDamageService.OnChangeTotalDamage -= ChangeTotalDamageHandler;
            _playerDamageService.OnChangeBrokenBones -= ChangeBrokenBonesHandler;
            _targetHitMissionService.OnPlayerHitTarget -= PlayerHitTargetHandler;
            _objectsDestroyMissionService.OnObjectDestroy -= OnObjectsDestroyHandler;
            _scoreService.OnScoreChanged -= ScoreChangedHandler;
            _bowlingMissionService.OnPinFall -= PinFallHandler;
            _destroyCarMissionService.OnCarDestroyed -= CarDestroyedHandler;
            _levelTrophyService.OnTrophyCollect -= TrophyCollectHandler;
            _hitMissionService.OnHitPeople -= HitPeopleHandler;
        }

        public void Dispose()
        {
            UnsubscribeEvents();
            _disposable?.Dispose();
        }
    }
}