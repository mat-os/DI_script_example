using System;
using System.Collections.Generic;
using System.Linq;
using Configs;
using Game.Scripts.Configs;
using Game.Scripts.Utils.Prefs;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Infrastructure.Services.Level
{
    public class GlobalMissionProgressService
    {
        private MissionProgressList _missionsProgressList;
        private DataService _dataService;
        private GameConfig _gameConfig;
        private GlobalMissionsConfig _globalMissionsConfig;
        private bool _isInitialized;

        public GlobalMissionProgressService(DataService dataService, GameConfig gameConfig)
        {
            _gameConfig = gameConfig;
            _globalMissionsConfig = _gameConfig.GlobalMissionsConfig;
            _dataService = dataService;
        }
        public void Initialize()
        {
            if(_isInitialized)
                return;

            _missionsProgressList = _dataService.Missions.GlobalMissionProgress.Value ?? new MissionProgressList();

            if (_missionsProgressList.Items.Count == 0)
            {
                Debug.Log("No _missionsProgressList data found. Generating default progress for all levels...");
                GenerateDefaultProgress();
                _dataService.Missions.GlobalMissionProgress.Value = _missionsProgressList; // Сохраняем
            }
            else
            {
                foreach (var item in _missionsProgressList.Items)
                {
                    Debug.Log($"Mission Progress: {item.CurrentValue} {item.IsActive}");
                }
                Debug.Log($"Loaded _missionsProgressList List with {_missionsProgressList.Items.Count} items.");
            }

            _isInitialized = true;
        }

        private void GenerateDefaultProgress()
        {
            foreach (var mission in _globalMissionsConfig.GlobalMissions)
            {
                _missionsProgressList.Items.Add(new GlobalMissionProgress
                {
                    IsCompleted =  false,
                    IsActive =  false,
                    CurrentValue = 0
                });
            }
            Debug.Log($"Generated default progress for {_missionsProgressList.Items.Count} missions.");
        }
        
        public void UpdateMissionProgress(int missionId, MissionProgress missionProgress)
        {
            GlobalMissionProgress existingProgress = _missionsProgressList.Items[missionId];
            Debug.Log($"Get existingProgress with missionId={missionId}");

            if (existingProgress != null)
            {
                Debug.Log($"Found existing progress missionId = {missionId}, updating values newValue: {missionProgress.CurrentValue}; isCompleted: {missionProgress.IsCompleted}");
                existingProgress.CurrentValue = missionProgress.CurrentValue;
                existingProgress.IsCompleted = missionProgress.IsCompleted;
                existingProgress.IsActive = missionProgress.IsActive;
            }
            else
            {
                Debug.Log($"No existing progress found, creating new entry missionId = {missionId}...");
                GlobalMissionProgress newProgress = new GlobalMissionProgress
                {
                    CurrentValue = missionProgress.CurrentValue,
                    IsActive = missionProgress.IsCompleted,
                    IsCompleted = missionProgress.IsActive
                };
                
                _missionsProgressList.Items.Add(newProgress);
            }

            // Save updated progress
            _dataService.Missions.GlobalMissionProgress.Value = _missionsProgressList;
        }

        public MissionProgressList GetMissionsProgressList()
        {
            if(!_isInitialized)
                Initialize();

            Debug.Log($"Get MissionsProgressList List with {_missionsProgressList.Items.Count} items.");
            return _missionsProgressList;
        }
        public GlobalMissionProgress GetMissionProgress(int missionId)
        {
            GlobalMissionProgress progress = _missionsProgressList.Items[missionId];

            if (progress == null)
            {
                Debug.Log("No progress found for this level, creating default entry...");
                progress = new GlobalMissionProgress
                {
                    CurrentValue = 0,
                    IsCompleted = false,
                    IsActive = false
                };

                // Добавляем новый объект в список для последующего сохранения
                _missionsProgressList.Items.Add(progress);
            }
            Debug.Log($"GetMissionProgress  {missionId}. CurrentValue:{progress.CurrentValue}. IsCompleted: {progress.IsCompleted}");
            return progress;
        }
        public void ResetMissionProgress()
        {
            
        }
    }
    [System.Serializable]
    public class MissionProgressList
    {
        public List<GlobalMissionProgress> Items = new List<GlobalMissionProgress>();
    }
    [System.Serializable]
    public class GlobalMissionProgress
    {
        public float CurrentValue;
        public bool IsCompleted;
        public bool IsActive;
    }
}