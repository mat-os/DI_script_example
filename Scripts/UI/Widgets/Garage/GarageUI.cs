using System;
using Configs;
using Game.Scripts.Core.CurrencyService;
using Game.Scripts.Infrastructure.Services;
using Game.Scripts.UI.Screens.Pages;
using Game.Scripts.UI.Widgets.CurrencyCounter;
using Game.Scripts.UI.Widgets.Customization;
using UnityEngine;
using Zenject;

namespace Game.Scripts.UI.Widgets
{
    public class GarageUI : MonoBehaviour
    {
        [SerializeField] private UpgradeProgressBarUI _upgradeProgressBarUI;
        [SerializeField] private UpgradeProgressBarUI _buttonsProgressBarUI;
        
        [Header("Customization")]
        [SerializeField] private CustomizationUI _customizationUI;

        [Header("Upgrade Buttons")]
        [SerializeField] private UpgradeView _carUpgradeView;
        [SerializeField] private UpgradeView _stuntUpgradeView;
        [SerializeField] private UpgradeView _earningsUpgradeView;
        
        [Header("Currency View")]
        [SerializeField] private CurrencyView _coinCurrencyView;
        private CurrencyViewPresenter _coinCurrencyViewPresenter;
        
        [Inject] private DiContainer _container;
        
        private UpgradePresenter _carUpgradePresenter;
        private UpgradePresenter _stuntUpgradePresenter;
        private UpgradePresenter _earningsUpgradePresenter;
        
        private DataService _dataService;
        private UIGarageService _uiGarageService;

        [Inject]
        public void Construct(DataService dataService, UIGarageService uiGarageService, CurrencyService currencyService, VibrationService vibrationService)
        {
            _uiGarageService = uiGarageService;
            _dataService = dataService;
            
            _container.Inject(_upgradeProgressBarUI);
            _container.Inject(_customizationUI);
            
            _coinCurrencyViewPresenter = new CurrencyViewPresenter(currencyService, _coinCurrencyView.CurrencyType, vibrationService, _coinCurrencyView.AnimationParameterConfig);
            _coinCurrencyView.Construct(_coinCurrencyViewPresenter);     
        
            _coinCurrencyView.Initialize();
            _coinCurrencyViewPresenter.Initialize();
        }

        public void Initialize()
        {
            // Апгрейд машины (Car)
            var carUpgradeModel = new UpgradeModel();
            _container.Inject(carUpgradeModel);
            _carUpgradePresenter = new UpgradePresenter(_carUpgradeView, carUpgradeModel, EUpgradeType.Car);

            // Апгрейд трюков (Stunt)
            var stuntUpgradeModel = new UpgradeModel();
            _container.Inject(stuntUpgradeModel);
            _stuntUpgradePresenter = new UpgradePresenter(_stuntUpgradeView, stuntUpgradeModel, EUpgradeType.Stunt);

            // Апгрейд заработка (Earnings)
            var earningsUpgradeModel = new UpgradeModel();
            _container.Inject(earningsUpgradeModel);
            _earningsUpgradePresenter = new UpgradePresenter(_earningsUpgradeView, earningsUpgradeModel, EUpgradeType.Income);
            
            _upgradeProgressBarUI.Initialize();
            _buttonsProgressBarUI.Initialize();
            
            _customizationUI.OnCreate();
            
            _customizationUI.OnChangeShopTab += OnChangeShopTabHandle;

            var rotationTarget = _uiGarageService.GetRotationTarget();
            _customizationUI.SetRotationTarget(rotationTarget);
        }

        private void OnChangeShopTabHandle(int index)
        {
            _uiGarageService.ChangeCameraByIndex(index);
        }

        public void ShowGarageView()
        {
            _uiGarageService.SetViewActive(true);
            _customizationUI.OnOpenStart();
        }

        public void HideGarageView()
        {
            _uiGarageService.SetViewActive(false);
            _customizationUI.OnCloseComplete();
        }

        private void OnDestroy()
        {
            _customizationUI.OnChangeShopTab -= OnChangeShopTabHandle;
        }
    }
}