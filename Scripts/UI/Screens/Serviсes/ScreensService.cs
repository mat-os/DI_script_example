using System;
using Configs;
using DG.Tweening;
using Game.Scripts.Core.StateMachine;
using Game.Scripts.Infrastructure.GameStateMachine;
using Game.Scripts.Infrastructure.GameStateMachine.States;
using Game.Scripts.Infrastructure.LevelStateMachin;
using Game.Scripts.Infrastructure.LevelStateMachin.States;
using Game.Scripts.UI.Screens.Pages;
using Game.Scripts.UI.Screens.Popups;

namespace Game.Scripts.UI.Screens.Serviсes
{
    public class ScreensService : IDisposable
    {
        private readonly GameStateMachine _gameStateMachine;
        private readonly LevelStateMachine _levelStateMachine;

        private readonly PageService _pageService;
        private readonly PopupService _popupService;
        private readonly MessageBoxService _messageBoxService;
        private UIConfig _uiConfig;

        public ScreensService
        (
            GameStateMachine gameStateMachine,
            LevelStateMachine levelStateMachine,
            PageService pageService,
            PopupService popupService,
            MessageBoxService messageBoxService,
            UIConfig uiConfig
        )
        {
            _uiConfig = uiConfig;
            _gameStateMachine = gameStateMachine;
            _gameStateMachine.StateChanged += OnGameStateEnter;

            _levelStateMachine = levelStateMachine;
            _levelStateMachine.StateChanged += OnLevelStateEnter;

            _pageService = pageService;
            _popupService = popupService;
            _messageBoxService = messageBoxService;
        }

        public void OnGameStateEnter(IState<EGameState> state)
        {
            switch (state)
            {
                case GameLoadingState:
                    CloseAll();
                    _pageService.ShowScreen<GameLoadingPage>();
                    break;
                case LoadingLevelState:
                    CloseAll();
                    _pageService.ShowScreen<LoadingLevelPage>();
                    break;
                case LobbyState:
                    CloseAll();
                    _pageService.ShowScreen<LobbyPage>();
                    break;
                case LevelState:
                    CloseAll();
                    //_popupService.ShowScreen<MissionStartInfoPopup>();
                    break;
            }
        }

        private void OnLevelStateEnter(IState<ELevelState> state)
        {
            switch (state)
            {
                case PlayLevelState:
                    CloseAll();
                    _pageService.ShowScreen<GameplayPage>();
                    break;
                case CompleteLevelState:
                    CloseAll();
                    DOVirtual.DelayedCall(_uiConfig.DelayBeforeLevelWinScreen, () =>
                    {
                        _pageService.ShowScreen<CompleteLevelPage>();
                    });
                    break;                         
                case FailLevelState:
                    CloseAll();
                    DOVirtual.DelayedCall(_uiConfig.DelayBeforeLevelFailedScreen, () =>
                    {
                        _pageService.ShowScreen<FailedLevelPage>();
                    });
                    break;           
                case FinalDialogueLevelState:
                    CloseAll();
                    break;
            }
        }

        private void CloseAll()
        {
            _pageService.CloseAll();
            _popupService.CloseAll();
            _messageBoxService.CloseAll();
        }

        public void Dispose()
        {
            _levelStateMachine.StateChanged -= OnLevelStateEnter;
            _gameStateMachine.StateChanged -= OnGameStateEnter;
        }
    }
}