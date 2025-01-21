using System.Linq;
using Configs;
using Game.Scripts.Configs.Level;
using Game.Scripts.Infrastructure.Services.Level;
using Game.Scripts.Utils.Debug;
using UnityEngine;

namespace Game.Scripts.Infrastructure.Services
{
    public class LevelDataService
    {
        private readonly LevelsRepository _levelsRepository;
        private readonly LevelPackService _levelPackService;
        private readonly DataService _dataService;

        private LevelDataConfig _levelDataConfig;
        
        private LevelDataConfig _previousLevelDataConfig;
        private int _previousLevelId;
        
        private int _levelId;
        public int LevelId => _levelId;
        
        public bool IsStartFromLevelZero => 
            _dataService.Level.IsCompleteLevelZero.Value == false && _levelsRepository.IsStartFromLevelZero;
        public bool IsStartDebugLevel => 
            _levelsRepository.IsDebugMode && Application.isEditor;
        
        public string GetGlobalLevelId(int levelIndex, int packIndex)
        {
            //var levelId = $"{levelIndex.ToString()}.{packIndex.ToString()}";
            Debug.Log($"GetGlobalLevelId {levelIndex}:{packIndex}");
            var globalLevelIndex = levelIndex;
            for (int i = 0; i < packIndex; i++)
            {
                globalLevelIndex += _levelsRepository.LevelPackConfigs[i].LevelDataConfigs.Count;
            }
            return globalLevelIndex.ToString();
        }


        public LevelDataService(
            LevelsRepository levelsRepository,
            LevelPackService levelPackService,
            DataService dataService)
        {
            _dataService = dataService;
            _levelPackService = levelPackService;
            _levelsRepository = levelsRepository;
        }
        public void SetCurrentLevelData(int levelId)
        {
            var currentLevelPack = _levelPackService.GetCurrentLevelPack();
            if (levelId <= currentLevelPack.LevelDataConfigs.Count)
            {
                _previousLevelDataConfig = _levelDataConfig;
                _previousLevelId = levelId;
                
                _levelDataConfig = currentLevelPack.LevelDataConfigs[levelId];
                _levelId = levelId;
                _dataService.Level.LevelIndex.Value = _levelId;
            }
            else
            {
                CustomDebugLog.LogError("No level with index " + levelId);
            }
        }
        public void RestartLevel()
        {
            _levelDataConfig = _previousLevelDataConfig;
            _levelId = _previousLevelId;
            _dataService.Level.LevelIndex.Value = _levelId;
        }
        public LevelDataConfig GetCurrentLevelData()
        {
            if (IsStartFromLevelZero)
            {
                _levelId = -1;
                _dataService.Level.LevelIndex.Value = _levelId;

                return _levelsRepository.LevelZero;
            }
#if UNITY_EDITOR
            if (IsStartDebugLevel)
            {
                CustomDebugLog.Log("Playing Debug level data ");
                return _levelsRepository.DebugLevelDataConfig;
            }
#endif
            
            if (_levelDataConfig != null)
            {
                CustomDebugLog.Log("Loading level data " + _levelDataConfig);
                return _levelDataConfig;
            }
            else
            {
                CustomDebugLog.Log("NOT FOUND level data!!! ");
                return _levelsRepository.LevelZero;
            }
        }
        public void SetDailyChallenge()
        {
            _levelDataConfig = _levelsRepository.DailyChallenge;
        }
        /*
        public void SetLevelDataByLastPlayedPack()
        {
            var currentLevelIndex = _dataService.Level.LevelIndex.Value;
            SetCurrentLevelData(currentLevelIndex);
        }
        */
        

    }
}