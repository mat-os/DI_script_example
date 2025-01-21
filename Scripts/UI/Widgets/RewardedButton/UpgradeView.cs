using System;
using AssetKits.ParticleImage;
using DG.Tweening;
using Game.RateUs.Scripts.UI.Common;
using Game.Scripts.UI.Widgets;
using Game.Scripts.UI.Widgets.Garage.UpgradeProgressBar;
using Game.Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeView : MonoBehaviour, IUpgradeView
{
    [SerializeField] private TextMeshProUGUI _lvlInfo;
    [SerializeField] private TextMeshProUGUI _upgradePrice;
    [SerializeField] private GameObject _lockView;
    [SerializeField] private GameObject _activeView;
    [SerializeField] private ProgressBarWithSlices _progressBar;
    [SerializeField] private GameObject _notification;
    [SerializeField] private Transform _root;
    
    [Header("Buttons")] 
    [SerializeField] private Button _coinsButton;
    [SerializeField] private Button _rewardedButton;
    
    [Header("Fx")] 
    [SerializeField] private ParticleImage _particleImage;
    [SerializeField] private AnimationParameterConfig _buyAnimationParameter;
    [SerializeField] private float _scaleOnBuy;
    [SerializeField] private AutoScale _autoScale;
    

    public event Action OnCoinUpgradeClick;
    public event Action OnAdUpgradeClick;

    private Sequence _scaleSequence;

    private void Start()
    {
        _coinsButton.onClick.AddListener(() => OnCoinUpgradeClick?.Invoke());
        //_rewardedButton.onClick.AddListener(() => OnAdUpgradeClick?.Invoke());
    }
    
    public void DisableButton()
    {
        gameObject.SetActive(false);
    }
    public void SetCoinButtonActive(bool isActive)
    {
        _coinsButton.gameObject.SetActive(isActive);
    }
    public void SetAdButtonActive(bool isActive)
    {
        _rewardedButton.gameObject.SetActive(isActive);
    }
    public void PlayEffectOnBuy()
    {
        _particleImage.gameObject.SetActive(true);
        _particleImage.Play();
        
        KillScaleEffect();

        _autoScale.enabled = false;
        _scaleSequence = DOTween.Sequence()
            .Append(_root.DOScale(_scaleOnBuy, _buyAnimationParameter.Duration).SetEase(_buyAnimationParameter.Ease)) 
            .Append(_root.DOScale(1f, _buyAnimationParameter.Duration).SetEase(_buyAnimationParameter.Ease)).OnComplete(
                () =>
                {
                    _autoScale.enabled = true;
                });
    }
    public void UpdateLevelText(int level)
    {
        _lvlInfo.text = $"{level + 1}";
    }

    public void ActivateLockedView(bool isLockViewActive)
    {
        _lockView.SetActive(isLockViewActive);
        _activeView.SetActive(!isLockViewActive);
        _autoScale.enabled = !isLockViewActive;
    }

    public void SetNotificationActive(bool isActive)
    {
        _notification.SetActive(isActive);
    }

    public void UpdateCostText(int cost)
    {
        _upgradePrice.text = $"{cost}";
    }

    public void SetupProgressBar(int currentLevel, int maxLevel)
    {
        _progressBar.SetProgress(currentLevel, maxLevel);
    }
    private void OnDestroy()
    {
        _coinsButton.onClick.RemoveAllListeners();
        //_rewardedButton.onClick.RemoveAllListeners();
    }

    private void OnDisable()
    {
        KillScaleEffect();
    }

    private void KillScaleEffect()
    {
        if (_scaleSequence != null)
        {
            _scaleSequence.Kill();
            _scaleSequence = null;
            _root.transform.localScale = Vector3.one;
            _autoScale.enabled = true;
        }
    }
}