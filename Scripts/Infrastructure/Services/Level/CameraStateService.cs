using System;
using Game.Scripts.Core.StateMachine;
using Game.Scripts.Infrastructure.GameStateMachine;
using Game.Scripts.Infrastructure.GameStateMachine.States;
using Game.Scripts.Infrastructure.LevelStateMachin;
using Game.Scripts.Infrastructure.LevelStateMachin.States;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Infrastructure.Services
{
    public class CameraStateService : IDisposable, IInitializable
    {
        private readonly GameStateMachine.GameStateMachine _gameStateMachine;
        private readonly LevelStateMachine _levelStateMachine;
        private readonly CameraService _cameraService;
        
        [Inject]
        public CameraStateService
        (
            GameStateMachine.GameStateMachine gameStateMachine,
            LevelStateMachine levelStateMachine,
            CameraService cameraService
        )
        {
            _cameraService = cameraService;
            _gameStateMachine = gameStateMachine;
            _levelStateMachine = levelStateMachine;
        }
        public void Initialize()
        {
            _gameStateMachine.StateChanged += OnGameStateEnter;
            _levelStateMachine.StateChanged += OnLevelStateEnter;
        }
        private void OnGameStateEnter(IState<EGameState> state)
        {
            switch (state)
            {
                case LobbyState:
                    _cameraService.SetActiveCamera(EGameCameraType.Menu); 
                    break;
                case LevelState:
                    _cameraService.SetActiveCamera(EGameCameraType.Gameplay); 
                    break;
            }
        }

        private void OnLevelStateEnter(IState<ELevelState> state)
        {
            switch (state)
            {
                case PlayLevelState:
                    _cameraService.SetActiveCamera(EGameCameraType.Gameplay); 
                    break;
                case LevelMissionState:
                    _cameraService.SetActiveCamera(EGameCameraType.Menu); 
                    break;
                /*case CompleteLevelState:
                    _cameraService.SetActiveCamera(EGameCameraType.Cinematic); 
                    break;
                case LevelFailState:
                    _cameraService.SetActiveCamera(EGameCameraType.Menu); 
                    break;*/
            }
        }

        public void Dispose()
        {
            _levelStateMachine.StateChanged -= OnLevelStateEnter;
            _gameStateMachine.StateChanged -= OnGameStateEnter;
        }
    }
}
