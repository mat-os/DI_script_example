using System;
using System.Collections.Generic;
using System.Linq;
using Configs;
using Game.Scripts.Utils.Prefs;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Infrastructure.Services.Level
{
    public class LevelsProgressService : IInitializable
    {
        private LevelProgressList _levelProgressList;
        private DataService _dataService;
        private LevelsRepository _levelsRepository;

        public LevelsProgressService(DataService dataService, LevelsRepository levelsRepository)
        {
            _levelsRepository = levelsRepository;
            _dataService = dataService;
        }
        public void Initialize()
        {
            _levelProgressList = _dataService.Level.LevelProgress.Value ?? new LevelProgressList();

            if (_levelProgressList.Items.Count == 0)
            {
                Debug.Log("No LevelProgress data found. Generating default progress for all levels...");
                GenerateDefaultProgress();
                _dataService.Level.LevelProgress.Value = _levelProgressList; // Сохраняем
            }
            else
            {
                Debug.Log($"Loaded LevelProgress List with {_levelProgressList.Items.Count} items.");
            }
        }

        /// <summary>
        /// Генерирует прогресс для всех уровней и паков
        /// </summary>
        private void GenerateDefaultProgress()
        {
            if (_levelsRepository.IsStartFromLevelZero)
            {
                _levelProgressList.Items.Add(new LevelProgress
                {
                    PackId = -1,
                    LevelIndex = -1,
                    Stars = 0,
                    MissionProgress = new bool[3] { false, false, false },
                    IsTrophyCollected = false
                });
            }
            foreach (ELevelPackId packId in Enum.GetValues(typeof(ELevelPackId)))
            {
                int totalLevels = GetTotalLevelsCountForPack((int)packId);
                for (int levelIndex = 0; levelIndex < totalLevels; levelIndex++)
                {
                    if (!_levelProgressList.Items.Any(lp => lp.PackId == (int)packId && lp.LevelIndex == levelIndex))
                    {
                        _levelProgressList.Items.Add(new LevelProgress
                        {
                            PackId = (int)packId,
                            LevelIndex = levelIndex,
                            Stars = 0,
                            MissionProgress = new bool[3] { false, false, false },
                            IsTrophyCollected = false
                        });
                    }
                }
            }

            Debug.Log($"Generated default progress for {_levelProgressList.Items.Count} levels.");
        }

        /// <summary>
        /// Получает общее количество уровней для указанного LevelPack
        /// </summary>
        private int GetTotalLevelsCountForPack(int packId)
        {
            // Находим конфигурацию пакета с указанным PackId
            var packConfig = _levelsRepository.LevelPackConfigs
                .FirstOrDefault(config => config.PackId == (ELevelPackId)packId);

            if (packConfig == null)
            {
                Debug.LogWarning($"No LevelPackConfig found for PackId: {packId}");
                return 0; // Если пакета нет, возвращаем 0
            }

            // Возвращаем количество уровней
            return packConfig.LevelDataConfigs.Count;
        }

        public void UpdateLevelProgress(int packId, int levelIndex, int stars, bool isTrophyCollected, List<MissionProgress> missionProgressList)
        {
            LevelProgress existingProgress = _levelProgressList.Items.Find(lp => lp.PackId == packId && lp.LevelIndex == levelIndex);
            Debug.Log($"UpdateLevelProgress called with PackId={packId}, LevelIndex={levelIndex}, Stars={stars}, IsTrophyCollected={isTrophyCollected}");

            if (existingProgress != null)
            {
                Debug.Log($"Found existing progress, updating values stars: {stars}; isTrophyCollected: {isTrophyCollected}");
                if(existingProgress.Stars < stars)
                    existingProgress.Stars = stars;
                if(existingProgress.IsTrophyCollected == false)
                    existingProgress.IsTrophyCollected = isTrophyCollected;
                
                for (int i = 0; i < missionProgressList.Count; i++)
                {
                    var mission = missionProgressList[i];
                    if (mission != null && mission.IsCompleted) 
                    {
                        existingProgress.MissionProgress[i] = true; // Миссия считается завершённой
                    }
                }
            }
            else
            {
                Debug.Log("No existing progress found, creating new entry...");
                LevelProgress newProgress = new LevelProgress
                {
                    PackId = packId,
                    LevelIndex = levelIndex,
                    Stars = stars,
                    MissionProgress = new bool[3] { false, false, false },
                    IsTrophyCollected = isTrophyCollected
                };
                
                for (int i = 0; i < missionProgressList.Count; i++)
                {
                    var mission = missionProgressList[i];
                    if (mission != null && mission.IsCompleted) 
                    {
                        existingProgress.MissionProgress[i] = true; // Миссия считается завершённой
                    }
                }
                
                _levelProgressList.Items.Add(newProgress);
            }

            // Save updated progress
            _dataService.Level.LevelProgress.Value = _levelProgressList;
        }


        public LevelProgress GetLevelProgress(int packId, int levelIndex)
        {
            LevelProgress progress = _levelProgressList.Items.Find(lp => lp.PackId == packId && lp.LevelIndex == levelIndex);

            if (progress == null)
            {
                Debug.Log("No progress found for this level, creating default entry...");
                progress = new LevelProgress
                {
                    PackId = packId,
                    LevelIndex = levelIndex,
                    Stars = 0,
                    MissionProgress = new bool[3] { false, false, false },
                    IsTrophyCollected = false
                };

                // Добавляем новый объект в список для последующего сохранения
                _levelProgressList.Items.Add(progress);
            }
            Debug.Log($"Get Progress of level  {levelIndex}. Stars count:{progress.Stars}. Trophy: {progress.IsTrophyCollected}");
            return progress;
        }
        
        public LevelProgress GetNextLevel(int currentPackId, int currentLevelIndex)
        {
            // Найти текущий LevelPack
            var currentPackLevels = _levelProgressList.Items.FindAll(lp => lp.PackId == currentPackId);
            //currentPackLevels.Sort((a, b) => a.LevelIndex.CompareTo(b.LevelIndex)); // Упорядочиваем уровни

            // Найти текущий уровень в текущем паке
            var currentLevel = currentPackLevels.Find(lp => lp.LevelIndex == currentLevelIndex);
            if (currentLevel == null)
            {
                Debug.LogError($"Current level not found in Pack {currentPackId} at Index {currentLevelIndex}.");
                return null;
            }

            // Найти следующий уровень в текущем паке
            var nextLevel = currentPackLevels.Find(lp => lp.LevelIndex == currentLevelIndex + 1);
            if (nextLevel != null)
                return nextLevel;

            // Если текущий пак завершен, ищем следующий пак
            var nextPack = (ELevelPackId)(currentPackId + 1);
            if (!Enum.IsDefined(typeof(ELevelPackId), nextPack))
            {
                Debug.Log("All levels completed! No more levels available.");
                return null; // Все уровни пройдены
            }

            // Найти первый уровень в следующем паке
            var nextPackLevels = _levelProgressList.Items.FindAll(lp => lp.PackId == (int)nextPack);
            nextPackLevels.Sort((a, b) => a.LevelIndex.CompareTo(b.LevelIndex));
            return nextPackLevels.FirstOrDefault();
        }
        public LevelProgress GetPreviousLevel(int currentPackId, int currentLevelIndex)
        {
            // Найти текущий LevelPack
            var currentPackLevels = _levelProgressList.Items.FindAll(lp => lp.PackId == currentPackId);
            // Упорядочиваем уровни по индексу
            //currentPackLevels.Sort((a, b) => a.LevelIndex.CompareTo(b.LevelIndex));

            // Найти текущий уровень
            var currentLevel = currentPackLevels.Find(lp => lp.LevelIndex == currentLevelIndex);
            if (currentLevel == null)
            {
                Debug.LogError($"Current level not found in Pack {currentPackId} at Index {currentLevelIndex}.");
                return null;
            }

            // Найти предыдущий уровень в текущем паке
            var previousLevel = currentPackLevels.FindLast(lp => lp.LevelIndex == currentLevelIndex - 1);
            if (previousLevel != null)
                return previousLevel;

            // Если текущий уровень первый в паке, ищем предыдущий пак
            var previousPack = (ELevelPackId)(currentPackId - 1);
            if (!Enum.IsDefined(typeof(ELevelPackId), previousPack))
            {
                Debug.Log("No previous levels available. This is the first level.");
                return null; // Это первый уровень
            }

            // Найти последний уровень в предыдущем паке
            var previousPackLevels = _levelProgressList.Items.FindAll(lp => lp.PackId == (int)previousPack);
            previousPackLevels.Sort((a, b) => a.LevelIndex.CompareTo(b.LevelIndex));
            return previousPackLevels.LastOrDefault();
        }
    }
    [System.Serializable]
    public class LevelProgress
    {
        public int PackId;      
        public int LevelIndex;
        public int Stars;        
        public bool IsTrophyCollected;

        // Новый массив для прогресса по трём миссиям
        public bool[] MissionProgress = new bool[3]; 
        
        public bool IsCompleted => Stars > 0;
    }
    [System.Serializable]
    public class LevelProgressList
    {
        public List<LevelProgress> Items = new List<LevelProgress>();
    }
    public enum ELevelPackId
    {
        StuntSchool = 0,
        FastAndFurious = 1,
        MadMax = 2
    }
}