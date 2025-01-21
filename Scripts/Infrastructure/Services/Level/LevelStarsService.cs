using Configs;
using Game.Scripts.Infrastructure.Services;
using Game.Scripts.Infrastructure.Services.Level;
using Game.Scripts.Utils.Prefs;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Configs.Level
{
    public class LevelStarsService
    {
        private readonly LevelsProgressService _levelsProgressService;
        private readonly RewardConfig _rewardConfig;

        private int _rewardForEarnedStars;
        private LevelDataService _levelDataService;

        [Inject]
        public LevelStarsService(DataService dataService, LevelsProgressService levelsProgressService,
            GameConfig gameConfig, LevelDataService levelDataService)
        {
            _levelDataService = levelDataService;
            _rewardConfig = gameConfig.RewardConfig;
            _levelsProgressService = levelsProgressService;
        }
        public int GetCountOfStarsOnLevel(LevelPackConfig currentLevelPack, LevelDataConfig levelDataConfig)
        {
            var progress = _levelsProgressService.GetLevelProgress(currentLevelPack.GetPackId(), currentLevelPack.LevelDataConfigs.IndexOf(levelDataConfig));
            return progress.Stars;
        }
        public int GetCountOfStarsOnLevel(int packId, int levelId)
        {
            var progress = _levelsProgressService.GetLevelProgress(packId, levelId);
            return progress.Stars;
        }
        public int GetCountOfEarnedStarsInPack(LevelPackConfig levelPackConfig)
        {
            int totalStars = 0;
            var packId = levelPackConfig.GetPackId();
            for (int i = 0; i < levelPackConfig.LevelDataConfigs.Count; i++)
            {
                totalStars += GetCountOfStarsOnLevel(packId, i);
            }

            return totalStars;
        }
        public void SetRewardForStars(int previousStarsEarned, int starsEarned)
        {
            // Получаем текущее количество звезд для уровня
            int currentStars = previousStarsEarned;

            var currentLevelData = _levelDataService.GetCurrentLevelData();

            // Вычисляем новые звезды
            int newStars = Mathf.Clamp(starsEarned - currentStars, 0, currentLevelData.LevelMissions.Count);

            if (newStars <= 0)
                _rewardForEarnedStars = 0;

            int reward = 0;
            for (int i = currentStars; i < currentStars + newStars; i++)
            {
                reward += currentLevelData.LevelMissions[i].Reward;
            }

            _rewardForEarnedStars = reward;
        }

        public int GetRewardForStars()
        {
            return _rewardForEarnedStars;
        }

        public void Clear()
        {
            _rewardForEarnedStars = 0;
        }
    }
}