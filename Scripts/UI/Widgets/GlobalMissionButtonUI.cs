using System;
using Coffee.UIEffects;
using Configs;
using DG.Tweening;
using Game.Scripts.Infrastructure.Services;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.Widgets
{
    public class GlobalMissionButtonUI : MonoBehaviour
    {
        public Action<GlobalMissionButtonUI> OnGetReward;
        
        [SerializeField] private MissionProgressBarUI _missionProgressBarUI;
        [SerializeField] private Button _acceptButton;
        [SerializeField] private UIEffect _buttonEffectUI;

        [Header("Debug")]
        [SerializeField] private Button _debugDoneButton;

        public LevelMission MissionConfig { get; private set; }
        public MissionProgressBarUI MissionProgressBarUI => _missionProgressBarUI;
        public Transform GetButtonTransform() => 
            _acceptButton.transform;
        
        public void Initialize(MissionProgress missionProgress)
        {
            MissionConfig = missionProgress.Mission;
            _acceptButton.onClick.AddListener(AcceptButtonClickedHandle);
            SetupDebugButton();
        }

        [Button]
        private void AcceptButtonClickedHandle()
        {
            OnGetReward?.Invoke(this);
            _acceptButton.interactable = false;
        }

        public void OnOpenStart(MissionProgress missionProgress)
        {
            _missionProgressBarUI.OnOpenStart(MissionConfig);
            
            var isCompleted = missionProgress.IsCompleted;
            _buttonEffectUI.enabled = !isCompleted;
            _acceptButton.interactable = isCompleted;
        }

        public void ShowFinalResult(MissionProgress missionProgress, LevelMission missionConfig)
        {
            _missionProgressBarUI.ShowFinalResult(missionProgress, missionConfig);
        }
        private void SetupDebugButton()
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            _debugDoneButton.gameObject.SetActive(true);
            _debugDoneButton.onClick.AddListener(AcceptButtonClickedHandle);
#else
        _debugDoneButton.gameObject.SetActive(false);
#endif
        }


        private void OnDestroy()
        {
            OnGetReward = null;
            _acceptButton.onClick.RemoveAllListeners();
            
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            _debugDoneButton.onClick.RemoveAllListeners();
#endif
        }

        public void Remove(Action onRemoveComplete)
        {
            transform.DOScaleY(0, 0.5f).OnComplete(() =>
            {
                onRemoveComplete?.Invoke();
                Destroy(gameObject);
            });
        }
    }
}