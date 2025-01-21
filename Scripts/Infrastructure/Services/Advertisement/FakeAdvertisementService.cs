using System;

namespace Game.Scripts.Infrastructure.Services.Advertisement
{
    public class FakeAdvertisementService : IAdvertisementService
    {
        public bool IsActive { get; set; }
        
        public event Action<bool> OnBannerStateChanged;
        public event Action<bool> OnInterstitialStateChanged;
        public event Action<bool> OnRewardedStateChanged;
        
        public void Initialize()
        {
            
        }
        public bool CanShowInterstitial(bool isTimerIgnored, string placement)
        {
            return false;
        }
        public bool CanShowRewarded(string placement)
        {
            return false;
        }
        public void ShowInterstitial(string placement, Action closeCallback)
        {
            
        }

        public void ShowRewarded(string placement, Action successCallback, Action closeCallback = null)
        {
            
        }
    }
}