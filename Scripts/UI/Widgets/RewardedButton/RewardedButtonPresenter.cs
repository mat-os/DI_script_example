using System;
using Game.Configs;
using Game.RateUs.Scripts.UI.Common;
using Game.Scripts.Infrastructure.Services.Advertisement;
using UniRx;
using UnityEngine;

public class RewardedButtonPresenter : MonoBehaviour
{
    public Action<bool> OnRewardedStateChanged;
    public ReactiveProperty<bool> IsInternetAvailable = new ReactiveProperty<bool>();

    [SerializeField] private StateButtonView _view;
    private IAdvertisementService _advertisementService;
    private AdvertisementConfig _settings;
    
    public RewardedButtonStateType CurrentState { get; private set; }

    public void Construct(IAdvertisementService advertisementService, AdvertisementConfig settings)
    {
        _advertisementService = advertisementService;
        _settings = settings;

        // Проверка интернет-соединения каждые 1 секунду
        Observable.Interval(TimeSpan.FromSeconds(1))
            .Subscribe(_ =>
            {
                bool isConnected = Application.internetReachability != NetworkReachability.NotReachable;
                IsInternetAvailable.Value = isConnected;
            })
            .AddTo(this); // Автоматическая отписка при уничтожении

        // Реактивное обновление состояния на основе доступности интернета
        IsInternetAvailable
            .Subscribe(isAvailable =>
            {
                SetState(isAvailable ? RewardedButtonStateType.Active : RewardedButtonStateType.Inactive);
                OnRewardedStateChanged?.Invoke(isAvailable);
            })
            .AddTo(this);
    }

    public void UpdateState(string placement)
    {
        SetState(_advertisementService.CanShowRewarded(placement)
            ? RewardedButtonStateType.Active
            : RewardedButtonStateType.Inactive);

#if UNITY_EDITOR
        if (_settings.IsRewardedAvailableInEditor)
        {
            SetState(RewardedButtonStateType.Active);
        }
#endif
    }

    public void ResetState()
    {
        SetState(RewardedButtonStateType.Inactive);
        _view.ResetState();
    }

    private void SetState(RewardedButtonStateType type)
    {
        _view.SetState(type);
        CurrentState = type;
    }
}
