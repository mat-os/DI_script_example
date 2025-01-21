using System;
using Configs;
using Game.Scripts.Configs;
using Game.Scripts.Utils.Debug;
using RootMotion.Dynamics;
using UniRx;
using UnityEngine;

namespace Game.Scripts.Infrastructure.Services.Player
{
    public class PlayerScoreService : IDisposable
    {
        public Action<int> OnScoreChanged;

        private readonly ScoreConfig _scoreConfig;
        private readonly PlayerComboService _playerComboService;
        private readonly ScoreTextEffectService _scoreTextEffectService;
        private readonly RewardConfig _rewardConfig;
        private readonly PlayerDamageService _playerDamageService;
        private readonly UpgradeService _upgradeService;

        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        private int _objectDestroyScore;
        private float _playerDamageScore;
        private int _praiseScore;
        private int _airTimeScore;
        private PlayerAirTimeCounterService _airTimeCounterService;

        public PlayerScoreService(GameConfig gameConfig, 
        PlayerComboService playerComboService, 
        PlayerDamageService playerDamageService, 
        ScoreTextEffectService scoreTextEffectService,
        UpgradeService upgradeService,
        PlayerAirTimeCounterService airTimeCounterService)
    {
        _airTimeCounterService = airTimeCounterService;
        _upgradeService = upgradeService;
        _scoreTextEffectService = scoreTextEffectService;
        _playerDamageService = playerDamageService;
        _playerComboService = playerComboService;
        _scoreConfig = gameConfig.ScoreConfig;
        _rewardConfig = gameConfig.RewardConfig;

        SubscribeToGlobalEvents();
        SubscribeToPlayerDamageEvents();
        
        _airTimeCounterService.OnPlayerScoredDuringAirTime += PlayerScoredDuringAirTimeHandler;
    }

        private void PlayerScoredDuringAirTimeHandler()
        {
            AddAirTimeScore(_scoreConfig.AirTimeScoreToAdd);
        }

        private void SubscribeToGlobalEvents()
    {
        GlobalEventSystem.Broker.Receive<DestroyObjectEvent>()
            .Subscribe(e => HandleObjectEvent(e.ScoreType, e.DestroyPosition))
            .AddTo(_disposable);

        GlobalEventSystem.Broker.Receive<HitPeopleEvent>()
            .Subscribe(e => HandleObjectEvent(e.ScoreType, e.DestroyPosition))
            .AddTo(_disposable);
    }

    private void SubscribeToPlayerDamageEvents()
    {
        _playerDamageService.OnGetDamage += HandleDamageEvent;
        _playerDamageService.OnBrokeBone += HandleBoneBreakEvent;
    }

    private void HandleBoneBreakEvent(Muscle.Group muscleGroup)
    {
        AddDamageScore(_scoreConfig.ScoreForBreakingBones[muscleGroup]);
    }

    private void HandleDamageEvent(float damage)
    {
        AddDamageScore((int)(damage * _scoreConfig.PlayerDamageScoreMultiplier));
    }

    private void HandleObjectEvent(EScoreType scoreType, Vector3 position)
    {
        if(scoreType == EScoreType.None)
            return;
        
        var baseScore = _scoreConfig.ScoreByType[scoreType];
        var score = baseScore * _playerComboService.CurrentComboLevel * _scoreConfig.ObjectDestroyScoreMultiplier;

        _objectDestroyScore += score;
        _scoreTextEffectService.CreateScoreTextEffect(position, baseScore);

        NotifyScoreChange();
    }

    public void AddScoreForPraise(int score)
    {
        _praiseScore += score;
        NotifyScoreChange();
    }

    private void AddDamageScore(int score)
    {
        _playerDamageScore += score;
        NotifyScoreChange();
    }    
    private void AddAirTimeScore(int airTimeScore)
    {
        _airTimeScore += airTimeScore;
        NotifyScoreChange();
    }

    private void NotifyScoreChange()
    {
        var totalScore = GetTotalScore();
        OnScoreChanged?.Invoke(totalScore);
        _playerComboService.UpdateComboLevel(totalScore);
    }

    public int GetTotalScore()
    {
        return Mathf.RoundToInt(_objectDestroyScore + _playerDamageScore + _praiseScore + _airTimeScore);
    }

    public int GetRewardForScore(bool isLevelComplete)
    {
        var multiplier = isLevelComplete ? _rewardConfig.RewardForScoreMultiplier : _rewardConfig.RewardForScoreOnFailMultiplier;
        var upgrade = _upgradeService.GetCurrentUpgradeStep<IncomeUpgradeStep>(EUpgradeType.Income).EarningMultiplier;
        return Mathf.RoundToInt(GetTotalScore() * multiplier * upgrade);
    }

    public void Clear()
    {
        _objectDestroyScore = 0;
        _playerDamageScore = 0;
        _praiseScore = 0;
        _airTimeScore = 0;
        NotifyScoreChange();
    }

    public void Dispose()
    {
        _playerDamageService.OnGetDamage -= HandleDamageEvent;
        _playerDamageService.OnBrokeBone -= HandleBoneBreakEvent;
        _airTimeCounterService.OnPlayerScoredDuringAirTime -= PlayerScoredDuringAirTimeHandler;
        _disposable.Dispose();
    }
    }
}