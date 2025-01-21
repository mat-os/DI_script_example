using Configs;
using Cysharp.Threading.Tasks;
using Game.Scripts.Constants;
using Game.Scripts.Core.Update;
using Game.Scripts.Infrastructure.GameStateMachine;
using Game.Scripts.Infrastructure.LevelStateMachin;
using Game.Scripts.Infrastructure.Services;
using Game.Scripts.Infrastructure.Services.Analytics;
using Game.Scripts.UI.Screens.Base.Screens;
using Game.Scripts.UI.Screens.Serviсes;
using Game.Scripts.UI.Widgets;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.Screens.Popups
{
    public class SettingsPopup : Popup
    {
        [SerializeField] private Button _vibrationButton;
        [SerializeField] private ToggleSwitch _vibrationToggleSwitch;

        [SerializeField] private Button _musicButton;
        [SerializeField] private ToggleSwitch _musicToggleSwitch;

        [SerializeField] private Button _sfxButton;
        [SerializeField] private ToggleSwitch _sfxToggleSwitch;

        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _homeButton;
        [SerializeField] private Button _restartButton;
        
        /*[Header("Privacy")]
        [SerializeField] private Button _privacySettings;
        [SerializeField] private Button _termOfUse;
        [SerializeField] private Button _privacyPolicy;*/
        
        private VibrationService _vibrationService;
        private DataService _dataService;
        private PopupService _popupService;
       // private AudioService _audioService;
        private UpdateService _updateService;
        private GameStateMachine _gameStateMachine;
        private LevelStateMachine _levelStateMachine;
        private AnalyticService _analyticService;

        [Inject]
        public void Construct(VibrationService vibrationService,
            DataService dataService,
            PopupService popupService,
           // AudioService audioService,
            UpdateService updateService,
            GameStateMachine gameStateMachine,
            LevelStateMachine levelStateMachine,
            AnalyticService analyticService)
        {
            _analyticService = analyticService;
            _levelStateMachine = levelStateMachine;
            _gameStateMachine = gameStateMachine;
            _updateService = updateService;
           // _audioService = audioService;
            _popupService = popupService;
            _dataService = dataService;
            _vibrationService = vibrationService;
            
            _vibrationButton.onClick.AddListener(OnTapVibrationButtonHandle);
            _musicButton.onClick.AddListener(OnTapMusicButtonHandle);
            _sfxButton.onClick.AddListener(OnTapSFXButtonHandle);
            
            _closeButton.onClick.AddListener(OnTapCloseButtonHandle);
            if(_homeButton != null)
                _homeButton.onClick.AddListener(OnTapHomeButtonHandle);    
            if(_restartButton != null)
                _restartButton.onClick.AddListener(OnTapRestartButtonHandle);

            /*_termOfUse.onClick.AddListener(OnTapTermOfUseHandle);
            _privacyPolicy.onClick.AddListener(OnTapPrivacyPolicyHandle);
            if (FreeplayPrivacyPolicy.IsConsentFormAvailable)
            {
                _privacySettings.onClick.AddListener(OnTapPrivacySettingsHandle);
            }*/
        }

        private void OnTapHomeButtonHandle()
        {
            _analyticService.LogPlayerExitLevel(_dataService.Level.LevelIndex.Value, _dataService.Level.LevelPackIndex.Value, EExitLevelReason.SettingsMenu);
            _levelStateMachine.FireTrigger(ELevelState.Exit);
            _gameStateMachine.FireTrigger(EGameState.Lobby);
        }      
        private void OnTapRestartButtonHandle()
        {
            _analyticService.LogLevelRetry(_dataService.Level.LevelIndex.Value, _dataService.Level.LevelPackIndex.Value, ERetryReason.FromSettings);
            _levelStateMachine.FireTrigger(ELevelState.Exit);
            _gameStateMachine.FireTrigger(EGameState.LevelLoading);
        }

        public override UniTask OnOpenStart()
        {
            //_privacySettings.gameObject.SetActive(FreeplayPrivacyPolicy.IsConsentFormAvailable);

            _vibrationToggleSwitch.Init(_dataService.GameSettings.IsVibrationEnabled.Value);
            _musicToggleSwitch.Init(_dataService.GameSettings.IsMusicEnabled.Value);
            _sfxToggleSwitch.Init(_dataService.GameSettings.IsSFXEnabled.Value);
          //  _audioService.PlaySoundOneShot(SoundFxConstants.UI_OPTIONS_OPEN);

            _updateService.SetIsPause(true);
            
            return base.OnOpenStart();
        }

        public override UniTask OnCloseComplete()
        {
            _updateService.SetIsPause(false);
            return base.OnCloseComplete();
        }

        private void OnTapPrivacyPolicyHandle()
        {
            /*FreeplayPrivacyPolicy.ShowPrivacyPolicy();
            FreeplayAdvertising.LockAd(AdType.Banner, "settings");*/
        }

        private void OnTapTermOfUseHandle()
        {
            /*FreeplayPrivacyPolicy.ShowTermsOfUse();
            FreeplayAdvertising.LockAd(AdType.Banner, "settings");*/
        }

        private void OnTapPrivacySettingsHandle()
        {
            //FreeplayPrivacyPolicy.ShowUpdateConsentForm();
        }

        private void OnTapVibrationButtonHandle()
        {
            _dataService.GameSettings.IsVibrationEnabled.Value = !_dataService.GameSettings.IsVibrationEnabled.Value;
            if (_dataService.GameSettings.IsVibrationEnabled.Value)
            {
                _vibrationService.Vibrate(VibrationPlaceType.VibrationEnabled);
            //    _audioService.PlaySoundOneShot(SoundFxConstants.UI_OPTIONS_SLIDER_ON);
            }
            else
            {
            //    _audioService.PlaySoundOneShot(SoundFxConstants.UI_OPTIONS_SLIDER_OFF);
            }
        }
        private void OnTapMusicButtonHandle()
        {
            _dataService.GameSettings.IsMusicEnabled.Value = !_dataService.GameSettings.IsMusicEnabled.Value;
          //  _audioService.SetBusSettings(MusicSource.Music, _dataService.GameSettings.IsMusicEnabled.Value);
            
            /*_audioService.PlaySoundOneShot(_dataService.GameSettings.IsMusicEnabled.Value
                ? SoundFxConstants.UI_OPTIONS_SLIDER_ON
                : SoundFxConstants.UI_OPTIONS_SLIDER_OFF);*/
        }      
        private void OnTapSFXButtonHandle()
        {
            _dataService.GameSettings.IsSFXEnabled.Value = !_dataService.GameSettings.IsSFXEnabled.Value;
            /*_audioService.SetBusSettings(MusicSource.SFX, _dataService.GameSettings.IsSFXEnabled.Value);
            
            _audioService.PlaySoundOneShot(_dataService.GameSettings.IsSFXEnabled.Value
                ? SoundFxConstants.UI_OPTIONS_SLIDER_ON
                : SoundFxConstants.UI_OPTIONS_SLIDER_OFF);*/
        }

        private void OnTapCloseButtonHandle()
        {
            //FreeplayAdvertising.UnlockAd(AdType.Banner, "settings");
            _popupService.CloseScreen<SettingsPopup>();
           // _audioService.PlaySoundOneShot(SoundFxConstants.UI_OPTIONS_CLOSED);
        }

        protected override void OnDestroy()
        {
            _vibrationButton.onClick.RemoveListener(OnTapVibrationButtonHandle);
            _musicButton.onClick.RemoveListener(OnTapMusicButtonHandle);
            _sfxButton.onClick.RemoveListener(OnTapSFXButtonHandle);
            
            _closeButton.onClick.RemoveListener(OnTapCloseButtonHandle);
            /*_termOfUse.onClick.RemoveListener(OnTapTermOfUseHandle);
            _privacyPolicy.onClick.RemoveListener(OnTapPrivacyPolicyHandle);
            
            if (FreeplayPrivacyPolicy.IsConsentFormAvailable)
            {
                _privacySettings.onClick.RemoveListener(OnTapPrivacySettingsHandle);
            }*/

            base.OnDestroy();
        }
    }
}