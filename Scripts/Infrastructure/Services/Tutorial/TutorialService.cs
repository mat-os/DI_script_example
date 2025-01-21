using System;
using Configs;
using DG.Tweening;
using Game.Scripts.Configs.Level;
using Game.Scripts.Configs.Vfx;
using Game.Scripts.Infrastructure.Services.Player;
using Game.Scripts.UI.Screens.Messages;
using Game.Scripts.UI.Screens.Serviсes;
using Game.Scripts.Utils.Debug;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Infrastructure.Services.Tutorial
{
    public class TutorialService : IDisposable
    {
        private readonly MessageBoxService _messageBoxService;
        private readonly TutorialConfig _tutorialConfig;

        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        private readonly PlayerFlightLaunchService _playerFlightLaunchService;
        private SlowMotionService _slowMotionService;
        private DataService _dataService;
        private PlayerFlightControlService _playerFlightControlService;
        private LevelDataService _levelDataService;

        [Inject]
        public TutorialService(MessageBoxService messageBoxService, GameConfig gameConfig, 
            SlowMotionService slowMotionService, 
            PlayerFlightLaunchService playerFlightLaunchService, 
            DataService dataService,
            PlayerFlightControlService playerFlightControlService,
            LevelDataService levelDataService)
        {
            _levelDataService = levelDataService;
            _playerFlightControlService = playerFlightControlService;
            _dataService = dataService;
            _playerFlightLaunchService = playerFlightLaunchService;
            _slowMotionService = slowMotionService;
            _messageBoxService = messageBoxService;
            _tutorialConfig = gameConfig.TutorialConfig;
            
            GlobalEventSystem.Broker.Receive<StartPlayLevelEvent>()
                .Subscribe(StartPlayLevelEventHandler)
                .AddTo(_disposable);
            
            GlobalEventSystem.Broker.Receive<TriggerTutorialEvent>()
                .Subscribe(TriggerTutorialEventHandle)
                .AddTo(_disposable);
            
            GlobalEventSystem.Broker.Receive<PlayerEnterLevelWithJumpTutorialTrigger>()
                .Subscribe(PlayerEnterLevelWithJumpTutorialTriggerHandle)
                .AddTo(_disposable);
        }

        private void PlayerEnterLevelWithJumpTutorialTriggerHandle(PlayerEnterLevelWithJumpTutorialTrigger playerEnterLevelWithJumpTutorialTrigger)
        {
            if (_dataService.Tutorial.IsSwipeToJumpTutorialShow.Value == false)
            {
                _playerFlightControlService.SetIsPlayerCanJump(false);
            }
        }
        private void StartPlayLevelEventHandler(StartPlayLevelEvent startPlayLevelEvent)
        {
            if (_dataService.Level.TotalLevelComplete.Value <= _tutorialConfig.LevelsToShowHoldToRideTutorial && _levelDataService.IsStartFromLevelZero == true)
            {
                _messageBoxService.ShowScreen<HoldToRideTutorialMessage>();
                _playerFlightLaunchService.OnPlayerHitWall += PlayerHitWallHandler;
            }
        }
        private void TriggerTutorialEventHandle(TriggerTutorialEvent triggerTutorialEvent)
        {
            CustomDebugLog.Log("[TUTORIAL] TutorialEventHandle triggered " + triggerTutorialEvent.TutorialStep, DebugColor.Green);
            switch (triggerTutorialEvent.TutorialStep)
            {
                case ETutorialStep.SwipeToJump:
                    if (_dataService.Tutorial.IsSwipeToJumpTutorialShow.Value == false)
                    {
                        _messageBoxService.ShowScreen<JumpTutorialMessage>();
                    }
                    break;           
                case ETutorialStep.SwipeToJumpWithEnergy:
                    if (_dataService.Tutorial.IsSwipeToJumpTutorialShow.Value == false)
                    {
                        _messageBoxService.ShowScreen<JumpWithEnergyTutorialMessage>();
                        _dataService.Tutorial.IsSwipeToJumpTutorialShow.Value = true;
                    }
                    break;
                case ETutorialStep.SimpleSwipeToJump:
                    _slowMotionService.StartSlowMo(ESlowMotionType.SimpleJumpTutorial);
                    _messageBoxService.ShowScreen<SimpleJumpTutorialMessage>();
                    break;            
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void PlayerHitWallHandler()
        {
            _messageBoxService.CloseScreen<HoldToRideTutorialMessage>();
            _playerFlightLaunchService.OnPlayerHitWall -= PlayerHitWallHandler;
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
    public enum ETutorialStep
    {
        HoldToRide,
        SwipeToJump,
        SimpleSwipeToJump,
        SwipeToJumpWithEnergy
    }
}