using System;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using Game.Scripts.LevelElements;
using Game.Scripts.UI.Screens.Messages;
using Game.Scripts.UI.Screens.Serviсes;
using Game.Scripts.Utils.Debug;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Infrastructure.Services.Level
{
    public class CutsceneService : MonoBehaviour, IDisposable
    {
        private FungusDialogueService _fungusDialogueService;
        
        private CutsceneView _cutsceneView;

        //private int _currentCameraIndex = -1;
        
        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        
        private CinemachineVirtualCamera[] _allCameras;
        private MessageBoxService _messageBoxService;

        [Inject]
        public void Construct(FungusDialogueService fungusDialogueService, MessageBoxService messageBoxService)
        {
            _messageBoxService = messageBoxService;
            
            _fungusDialogueService = fungusDialogueService;
            _fungusDialogueService.OnStartDialogue += StartDialogueHandler;
            _fungusDialogueService.OnEndDialogue += EndDialogueHandler;
            
            GlobalEventSystem.Broker.Receive<ChangeCameraOnDialogueEvent>()
                .Subscribe(ChangeCameraOnDialogueHandler)
                .AddTo(_disposable);
        }


        public void SetCutsceneCameras(LevelView levelView)
        {
            _cutsceneView = levelView.CutsceneView;
            _allCameras = _cutsceneView.GetComponentsInChildren<CinemachineVirtualCamera>(true);
        }

        public void PlayEndLevelZeroEndingAnimation()
        {
            EndLevelZeroMessage levelZeroMessage = _messageBoxService.ShowScreen<EndLevelZeroMessage>();
            DOVirtual.DelayedCall(levelZeroMessage.DelayBeforeHide, () =>
            {
                _messageBoxService.CloseScreen<EndLevelZeroMessage>();
            });
        }
        public void PlayStartLevelOneAnimation()
        {
            StartLevelOneMessage startLevelOneMessage = _messageBoxService.ShowScreen<StartLevelOneMessage>();
            DOVirtual.DelayedCall(startLevelOneMessage.DelayBeforeHide, () =>
            {
                _messageBoxService.CloseScreen<StartLevelOneMessage>();
            });
        }
        private void StartDialogueHandler()
        {
            //ActivateCameraByIndex(0);
        }
        private void ChangeCameraOnDialogueHandler(ChangeCameraOnDialogueEvent changeCamera)
        {
            foreach (var pair in _cutsceneView.CutsceneCameras)
            {
                pair.Value.Camera.gameObject.SetActive(pair.Key == changeCamera.CameraType);
            }
        }
        private void EndDialogueHandler()
        {
            DeactivateAllCameras();
        }
        /*private void ActivateCameraByIndex(int index)
        {
            if (_cutsceneView == null || index < 0 || index >= _cutsceneView.CutsceneCameras.Count)
            {
                Debug.LogWarning($"Invalid camera index: {index}");
                return;
            }
            
            DeactivateAllCameras();
            _cutsceneView.CutsceneCameras.Values[_currentCameraIndex].Camera.gameObject.SetActive(true);
        }*/

        private void DeactivateAllCameras()
        {
            CustomDebugLog.Log("[Cutscene] DeactivateAllCameras");
            foreach (var camera in _allCameras)
            {
                camera.gameObject.SetActive(false);
            }
        }

        public void Clear()
        {
            _allCameras = null;
            //_currentCameraIndex = -1;
            _cutsceneView = null;
        }

        public void Dispose()
        {
            _fungusDialogueService.OnStartDialogue -= StartDialogueHandler;
            _fungusDialogueService.OnEndDialogue -= EndDialogueHandler;
            _disposable?.Dispose();
        }
    }
}
