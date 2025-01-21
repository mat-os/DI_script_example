using System;
using System.Collections.Generic;
using System.Linq;
using Configs;
using Game.Scripts.Configs;
using Game.Scripts.Configs.Level;
using Game.Scripts.Infrastructure.Services.Level;
using Game.Scripts.Infrastructure.Services.Player;
using Game.Scripts.Utils.Debug;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Infrastructure.Services
{
    public class GlobalMissionService :  IDisposable
    {
        public event Action OnMissionCompleted;
        public event Action<MissionProgress> OnMissionValueChange;
        public event Action<List<MissionProgress>> OnTotalMissionValueChange;
        public event Action<MissionProgress> OnActivateNewMission;
        
        private List<MissionProgress> _allMissionProgress;
        private List<MissionProgress> _activeMissionProgress;
        private List<MissionProgress> _completedMissionProgress;

        private readonly PlayerDamageService _playerDamageService;
        private readonly TargetHitMissionService _targetHitMissionService;
        private readonly ObjectsDestroyMissionService _objectsDestroyMissionService;
        private readonly PlayerScoreService _scoreService;
        private readonly BowlingMissionService _bowlingMissionService;
        private readonly DestroyCarMissionService _destroyCarMissionService;
        private readonly LevelTrophyService _levelTrophyService;
        private readonly PeopleHitMissionService _hitMissionService;
        private readonly PlayerFlightTrackerService _playerFlightTrackerService;
        
        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        private GlobalMissionsConfig _globalMissionsConfig;
        private GlobalMissionProgressService _globalMissionProgressService;
        private readonly int _activeMissionsMaxCount;

        [Inject]
        public GlobalMissionService( 
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
            GameConfig gameConfig,
            GlobalMissionProgressService globalMissionProgressService)
        {
            _globalMissionProgressService = globalMissionProgressService;
            _hitMissionService = hitMissionService;
            _levelTrophyService = levelTrophyService;
            _destroyCarMissionService = destroyCarMissionService;
            _bowlingMissionService = bowlingMissionService;
            _scoreService = scoreService;
            _objectsDestroyMissionService = objectsDestroyMissionService;
            _targetHitMissionService = targetHitMissionService;
            _playerDamageService = playerDamageService;
            _playerFlightTrackerService = playerFlightTrackerService;

            _globalMissionsConfig = gameConfig.GlobalMissionsConfig;
            _activeMissionsMaxCount = _globalMissionsConfig.GlobalMissionsCountOnUI;
            
            SubscribeEvents();
        }
        
        public void InitializeMissions()
        {
            _allMissionProgress = new List<MissionProgress>();
            _activeMissionProgress = new List<MissionProgress>();
            _completedMissionProgress = new List<MissionProgress>();

            foreach (var mission in _globalMissionsConfig.GlobalMissions)
            {
                _allMissionProgress.Add(new MissionProgress(mission)); // Привязать прогресс к каждой миссии
            }
            
            MissionProgressList missionsProgressList = _globalMissionProgressService.GetMissionsProgressList();
            for (int i = 0; i < _allMissionProgress.Count; i++)
            {
                _allMissionProgress[i].CurrentValue = missionsProgressList.Items[i].CurrentValue;
                _allMissionProgress[i].IsCompleted = missionsProgressList.Items[i].IsCompleted;
                _allMissionProgress[i].IsActive = missionsProgressList.Items[i].IsActive;
                Debug.Log($"Mission {i} IsActive = {missionsProgressList.Items[i].IsActive} - CurrentValue {missionsProgressList.Items[i].CurrentValue}");
                if (_allMissionProgress[i].IsCompleted)
                {
                    _completedMissionProgress.Add(_allMissionProgress[i]);
                    Debug.Log("ALREADY COMPLETE MISSION " + i);
                }
            }

            InitializeActiveMissions();
        }
        private void InitializeActiveMissions()
        {
            while (_activeMissionProgress.Count < _activeMissionsMaxCount)
            {
                var newMission = GetNextMission();
                if (newMission == null)
                {
                    Debug.LogWarning("No new missions available to initialize.");
                    break;
                }

                if (newMission.IsActive == false)
                {
                    Debug.Log("newMission.IsActive false!!!!!!!");
                    newMission.IsActive = true;
                    _globalMissionProgressService.UpdateMissionProgress(_allMissionProgress.IndexOf(newMission), newMission);
                }

                _activeMissionProgress.Add(newMission);
                Debug.Log($"Initialized active mission: {newMission.Mission.EMissionType}");
            }

            // Генерация события для обновления UI.
            OnTotalMissionValueChange?.Invoke(_activeMissionProgress);
        }
        public void CompleteMission(LevelMission levelMission)
        {
            var completedMission = _activeMissionProgress.Find(x => x.Mission == levelMission);
            if (completedMission == null || !_activeMissionProgress.Contains(completedMission))
            {
                Debug.LogWarning("Trying to complete an invalid or non-active mission.");
                return;
            }

            // Обновляем статус миссии.
            completedMission.IsCompleted = true;
            completedMission.IsActive = false;
            _activeMissionProgress.Remove(completedMission);
            _completedMissionProgress.Add(completedMission);

            // Сохранение прогресса.
            _globalMissionProgressService.UpdateMissionProgress(_allMissionProgress.IndexOf(completedMission), completedMission);
            Debug.Log($"Mission completed: {completedMission.Mission.EMissionType}");

            // Добавляем новую миссию (если есть).
            var newMission = GetNextMission();
            if (newMission != null)
            {
                newMission.IsActive = true;
                _globalMissionProgressService.UpdateMissionProgress(_allMissionProgress.IndexOf(newMission), newMission);
                _activeMissionProgress.Add(newMission);
                OnActivateNewMission?.Invoke(newMission);
                Debug.Log($"New mission activated: {newMission.Mission.EMissionType}");
            }

            // Генерация события для UI.
            OnMissionCompleted?.Invoke();
            OnTotalMissionValueChange?.Invoke(_activeMissionProgress);
        }
        public MissionProgress GetNextMission()
        {
            var nextMission = _allMissionProgress.FirstOrDefault(mp => !mp.IsCompleted
                                                                       && _activeMissionProgress.Contains(mp) == false
                                                                       && _completedMissionProgress.Contains(mp) == false);
            if (nextMission == null)
            {
                Debug.LogWarning("No new missions available.");
            }

            return nextMission;
        }
        public List<MissionProgress> GetActiveMissionsProgress()
        {
            if (_activeMissionProgress == null || _activeMissionProgress.Count == 0)
            {
                Debug.LogWarning("Active missions list is empty.");
            }

            return _activeMissionProgress;
        }

        public void SaveAllProgress()
        {
            Debug.Log("Save Global Missions");
            foreach (var missions in _activeMissionProgress)
            {
                _globalMissionProgressService.UpdateMissionProgress(_allMissionProgress.IndexOf(missions), missions);
            }
        }
        /*private void ResetMissionProgress()
        {
            _allMissionProgress.Clear();
            _globalMissionProgressService.ResetMissionProgress();
            InitializeMissions(_globalMissionsConfig.GlobalMissions);
        }*/
        
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
        private void PlayerHitTargetHandler(ETargetHitResult targetHitResult)
        {
            switch (targetHitResult)
            {
                case ETargetHitResult.Miss:
                    UpdateMissionProgress(EMissionType.TargetHit, 0);
                    break;
                case ETargetHitResult.InnerZone:
                    UpdateMissionProgress(EMissionType.TargetHit, _activeMissionsMaxCount);
                    break;
                case ETargetHitResult.MiddleZone:
                    UpdateMissionProgress(EMissionType.TargetHit, 2);
                    break;
                case ETargetHitResult.OuterZone:
                    UpdateMissionProgress(EMissionType.TargetHit, 1);
                    break;
            }
        }
        
        public void UpdateMissionProgress(EMissionType eMissionType, int value)
        {
            foreach (var progress in _activeMissionProgress)
            {
                // Если миссия соответствует текущему типу
                if (progress.Mission.EMissionType == eMissionType)
                {
                    if (progress.CurrentValue < value && progress.IsCompleted == false)
                    {
                        progress.CurrentValue = value;
                        OnMissionValueChange?.Invoke(progress);
                        OnTotalMissionValueChange?.Invoke(_allMissionProgress);

                        if (IsMissionCompleted(progress.Mission.TargetValue, value))
                        {
                            progress.IsCompleted = true;
                            OnMissionCompleted?.Invoke();
                            _globalMissionProgressService.UpdateMissionProgress(_allMissionProgress.IndexOf(progress), progress);
                            
                            CustomDebugLog.Log($"Mission {progress.Mission.EMissionType.ToString()} completed!", DebugColor.Orange);
                        }
                    }
                }
            }
        }
        private bool IsMissionCompleted(int targetValue, int currentValue)
        {
            return currentValue >= targetValue;
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