using System;
using DG.Tweening;
using Game.Scripts.Configs.Level;
using Game.Scripts.UI.Screens.Messages;
using Game.Scripts.UI.Screens.Serviсes;
using UniRx;

namespace Game.Scripts.Infrastructure.Services.Level
{
    public class LevelTrophyService : IDisposable
    {
        public Action OnTrophyCollect;
        
        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        
        private DataService _dataService;
        private LevelsProgressService _levelsProgressService;
        private MessageBoxService _messageBoxService;

        public bool IsTrophyCollected { get; private set; }

        public LevelTrophyService(DataService dataService, LevelsProgressService levelsProgressService, MessageBoxService messageBoxService)
        {
            _messageBoxService = messageBoxService;
            _levelsProgressService = levelsProgressService;
            _dataService = dataService;

            GlobalEventSystem.Broker.Receive<PlayerCollectTrophyEvent>()
                .Subscribe(CollectTrophyHandler)
                .AddTo(_disposable);
        }

        private void CollectTrophyHandler(PlayerCollectTrophyEvent playerCollectTrophyEvent)
        {
            IsTrophyCollected = true;
            OnTrophyCollect?.Invoke();
            _messageBoxService.ShowScreen<TrophyCollectMessage>();
        }
        public void TryActivateTrophyOnLevel()
        {
            var isEarnTrophyOnLevel = IsEarnTrophyOnLevel(_dataService.Level.LevelPackIndex.Value, _dataService.Level.LevelIndex.Value);
            if (isEarnTrophyOnLevel)
            {
                GlobalEventSystem.Broker.Publish(new DisableTrophyOnLevelEvent() {  });
            }
        }
        public int GetCountOfCollectedTrophyInPack(LevelPackConfig levelPackConfig)
        {
            int totalTrophy = 0;
            var packId = levelPackConfig.GetPackId();
            for (int i = 0; i < levelPackConfig.LevelDataConfigs.Count; i++)
            {
                totalTrophy += IsEarnTrophyOnLevel(packId, i) ? 1 : 0;
            }

            return totalTrophy;
        }

        public bool IsEarnTrophyOnLevel(int packId, int levelId)
        {
            var progress = _levelsProgressService.GetLevelProgress(packId, levelId);
            return progress.IsTrophyCollected;
        }
        
        public void Clear()
        {
            IsTrophyCollected = false;
        }
        public void Dispose()
        {
            _disposable.Dispose();
        }


    }
}