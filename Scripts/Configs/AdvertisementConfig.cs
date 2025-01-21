using System;
using UnityEngine;

namespace Game.Configs
{
    [CreateAssetMenu(fileName = nameof(AdvertisementConfig), menuName = "Configs/" + nameof(AdvertisementConfig))]
    public class AdvertisementConfig : ScriptableObject
    {
        [SerializeField] [Min(0f)] private float _interstitialDelay;
        [SerializeField] [Min(0f)] private int _activateInterstitialAfterLevel;
        //[SerializeField] [Min(0f)] private float _noTanksDelay;
        [SerializeField] [Min(0f)] private int _timerOfInterstitialDuration;
        [SerializeField] private bool _isRewardedAvailableInEditor;

        public float InterstitialDelay => _interstitialDelay;
        public int ActivateInterstitialAfterLevel => _activateInterstitialAfterLevel;
       // public float NoTanksDelay => _noTanksDelay;
        public bool IsRewardedAvailableInEditor => _isRewardedAvailableInEditor;
        public int TimerOfInterstitialDuration => _timerOfInterstitialDuration;
    }
}