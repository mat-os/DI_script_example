using System;
using Zenject;

namespace Game.Scripts.Infrastructure.Services.Advertisement
{
    public interface IAdvertisementService : IInitializable
    {
        public bool IsActive { get; set; }
        public bool CanShowInterstitial(bool isTimerIgnored, string placement);
        public bool CanShowRewarded(string placement);
        
        public event Action<bool> OnBannerStateChanged;
        public event Action<bool> OnInterstitialStateChanged;
        public event Action<bool> OnRewardedStateChanged;
        public void ShowInterstitial(string placement, Action closeCallback);
        public void ShowRewarded(string placement, Action successCallback, Action closeCallback = null);
    }
}