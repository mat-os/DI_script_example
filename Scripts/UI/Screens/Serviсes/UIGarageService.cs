using System;
using Configs;
using Game.Scripts.Customization;
using Game.Scripts.Customization.ItemConfigs;
using Game.Scripts.UI.Widgets;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Infrastructure.Services
{
    public class UIGarageService : MonoBehaviour
    {
        [SerializeField]private Transform garageContainer;
        
        private UIConfig _uiConfig;
        private GarageView _garageView;
        
        private bool _isInitialized;
        private CustomizationShopService _customizationShopService;

        [Inject]
        public void Construct(UIConfig uiConfig, CustomizationShopService customizationShopService)
        {
            _customizationShopService = customizationShopService;
            _uiConfig = uiConfig;
        }

        private void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (!_isInitialized)
            {
                _garageView = Instantiate(_uiConfig.GarageView, garageContainer.transform);
                SetViewActive(false);
                _isInitialized = true;
                
                SetupStartCarColor();

                SubscribeEvent();
            }
        }

        private void SetupStartCarColor()
        {
            var currentColor = _customizationShopService.GetCurrentCarColorConfig();
            ChangeCarColor(currentColor);
        }

        private void SubscribeEvent()
        {
            _customizationShopService.OnChangeCarColor += ChangeCarColor;
            _customizationShopService.OnBuyItem += OnBuyItemHandler;
        }

        private void OnBuyItemHandler(string obj)
        {
            _garageView.IncomeUpgrade.Play();
        }

        public void PlayUpgradeFx(EUpgradeType upgradeType)
        {
            switch (upgradeType)
            {
                case EUpgradeType.Car:
                    _garageView.CarUpgrade.Play();
                    break;
                case EUpgradeType.Stunt:
                    _garageView.StuntUpgrade.Play();
                    break;
                case EUpgradeType.Income:
                    _garageView.IncomeUpgrade.Play();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(upgradeType), upgradeType, null);
            }
        }

        public void SetViewActive(bool isActive)
        {
            garageContainer.gameObject.SetActive(isActive);
        }

        public void ChangeCarColor(ShopCarCarColorItemConfig carColorItemConfig)
        {
            _garageView.CarCustomizationController.SetCarMaterial(carColorItemConfig.CarMaterial);
            _garageView.CarUpgrade.Play();
        }

        private void OnDestroy()
        {
            _customizationShopService.OnChangeCarColor -= ChangeCarColor;
            _customizationShopService.OnBuyItem -= OnBuyItemHandler;
        }

        public void ChangeCameraByIndex(int activeCamIndex)
        {
            for (var i = 0; i < _garageView.GarageCameras.Length; i++)
            {
                var camera = _garageView.GarageCameras[i];
                camera.SetActive(activeCamIndex == i);
            }
        }

        public Transform GetRotationTarget()
        {
            return _garageView.CarCustomizationController.transform;
        }
    }
}