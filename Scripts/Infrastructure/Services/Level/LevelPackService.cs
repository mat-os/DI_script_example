using Configs;
using Game.Scripts.Configs.Level;
using Game.Scripts.Utils.Debug;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Infrastructure.Services.Level
{
    public class LevelPackService : IInitializable
    {
        private readonly LevelsRepository _levelsRepository;
        private readonly DataService _dataService;
        
        private LevelPackConfig _currentLevelPack;
        private LevelsProgressService _levelsProgressService;

        public bool IsAllLevelsUnlocked => 
            _levelsRepository.IsAllLevelsUnlocked;

        public int LevelPackIndex { get; private set; }

        public LevelPackService(LevelsRepository levelsRepository, DataService dataService, LevelsProgressService levelsProgressService)
        {
            _levelsProgressService = levelsProgressService;
            _dataService = dataService;
            _levelsRepository = levelsRepository;
        }

        public void Initialize()
        {
            var index = _dataService.Level.LevelPackIndex.Value;
            SetLevelPackByIndex(index);
        }
        public LevelPackConfig GetCurrentLevelPack()
        {
            if(_currentLevelPack == null)
                CustomDebugLog.LogWarning("No level pack configured!");
            
            return _currentLevelPack;
        }

        //TODO:
        public void SetLevelPackByIndex(int index)
        {
            LevelPackIndex = index;
            _dataService.Level.LevelPackIndex.Value = LevelPackIndex;

            _currentLevelPack = _levelsRepository.LevelPackConfigs[index];
        }

        public int GetCountOfCompletedLevelsInPack(LevelPackConfig levelPackConfig)
        {
            int totalLevelCompleted = 0;
            var packId = levelPackConfig.GetPackId();
            for (int i = 0; i < levelPackConfig.LevelDataConfigs.Count; i++)
            {
                totalLevelCompleted += IsLevelCompleted(packId, i) ? 1 : 0;
            }

            return totalLevelCompleted;
        }

        public bool IsLevelCompleted(int packId, int levelId)
        {
            var progress = _levelsProgressService.GetLevelProgress(packId, levelId);
            return progress.IsCompleted;
        }
        /*public void SetLevelPackByLastPlayedPack()
        {
            var currentLevelPack = _dataService.Level.CurrentLevelPackIndex.Value;
            SetLevelPackByIndex(currentLevelPack);
        }*/
    }
}