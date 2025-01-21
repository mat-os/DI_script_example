using System.Linq;
using Configs;
using Game.Scripts.Utils.Debug;
using UnityEngine;

namespace Game.Scripts.Infrastructure.Services.Level
{
    public class DifficultyService
    {
        private LevelDataService _levelDataService;
        private DataService _dataService;
        
        private DifficultyStep _currentDifficultyStep;
        
        public DifficultyService(LevelDataService levelDataService, DataService dataService)
        {
            _dataService = dataService;
            _levelDataService = levelDataService;
        }
        public void ApplyDifficulty()
        {
            int attempt = _dataService.Level.AttemptNumber.Value;
            var levelData = _levelDataService.GetCurrentLevelData();

            CustomDebugLog.Log($"[DIFFICULTY] Trying to find Difficulty for Attempt: {attempt}");
            
            if (levelData.DifficultySettings == null || levelData.DifficultySettings.DifficultySteps == null || levelData.DifficultySettings.DifficultySteps.Count == 0)
            {
                CustomDebugLog.Log("[DIFFICULTY] No Difficulty Settings or Difficulty Steps found");
                _currentDifficultyStep = null;
                return;
            }

            // Выбираем самый подходящий шаг
            _currentDifficultyStep = levelData.DifficultySettings.DifficultySteps
                .Where(d => d.AttemptNumber <= attempt)
                .OrderBy(d => d.AttemptNumber) 
                .LastOrDefault(); // Берём последний (наибольший доступный)

            if (_currentDifficultyStep == null)
            {
                // Если ни один шаг не подошёл, возьмём последний 
                _currentDifficultyStep = levelData.DifficultySettings.DifficultySteps
                    .OrderBy(d => d.AttemptNumber) 
                    .LastOrDefault(); // Берём последний
            }

            CustomDebugLog.Log(_currentDifficultyStep != null 
                ? $"[DIFFICULTY] Applied Difficulty Step for Attempt {attempt}, using Step for Attempt { _currentDifficultyStep.AttemptNumber }" 
                : "[DIFFICULTY] No Difficulty Step applied");
        }
        public float GetPlayerFlyForceMultiplier()
        {
            if (_currentDifficultyStep != null)
            {
                return _currentDifficultyStep.FlyForceMultiplier;
            }

            return 1;
        }       
        public float GetPlayerJumpForceMultiplier()
        {
            if (_currentDifficultyStep != null)
            {
                return _currentDifficultyStep.JumpForceMultiplier;
            }

            return 1;
        }

        public float GetCarAccelerationMultiplier()
        {
            if (_currentDifficultyStep != null)
            {
                return _currentDifficultyStep.CarAccelerationMultiplier;
            }

            return 1;
        }

        public float GetPlayerDamageMultiplier()
        {
            if (_currentDifficultyStep != null)
            {
                return _currentDifficultyStep.PlayerDamageMultiplier;
            }

            return 1;
        }
        public float GetPhysicsVelocityStopMultiplier()
        {
            if (_currentDifficultyStep != null)
            {
                return _currentDifficultyStep.PhysicsVelocityStopMultiplier;
            }

            return 1;
        }
    }
}