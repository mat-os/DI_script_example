using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Core.CurrencyService;
using Game.Scripts.Customization.ItemConfigs;
using Game.Scripts.Infrastructure.Services;
using Game.Scripts.Infrastructure.Services.Analytics;
using Game.Scripts.Infrastructure.Services.Car;
using Game.Scripts.Utils.Debug;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Scripts.Customization
{
    public class CustomizationShopService 
    {
        public event Action<string> OnBuyItem;

        public Action<ShopCarCarColorItemConfig> OnChangeCarColor;
        public Action<ShopCarCarModelItemConfig> OnChangeCarModel;
        public Action<ShopCarCarDecalItemConfig> OnChangeCarDecal;
        
        private readonly DataService _dataService;
        private readonly CustomizationShopConfig _customizationShopConfig;
        private readonly CurrencyService _currencyService;
        private AnalyticService _analyticService;

        public List<ShopCarItemConfig> _purchsedItems = new List<ShopCarItemConfig>();
        public Dictionary<string, ShopCarItemConfig> _map = new Dictionary<string, ShopCarItemConfig>();
        public PurchaseData _internalPurchaseData;

        private ShopView _shopView;
        //TODO:
        private const ECarColor SHOP_CAT_COLOR = ECarColor.Red;

        //TODO:
        public ShopCarCarColorItemConfig PickedHatItem { get; set; }

        public ShopView ShopView
        {
            get
            {
                if (_shopView != null)
                    return _shopView;

                _shopView = Object.FindObjectOfType<ShopView>();

                if (_shopView == null)
                {
                    CustomDebugLog.LogError($"No level view");
                    return null;
                }
                else
                {
                    //TODO;
                    //_shopView.DummyCatController.Init(this);
                }

                return _shopView;
            }
        }

        public CustomizationShopService(DataService dataService, 
            CustomizationShopConfig customizationShopConfig, 
            CurrencyService currencyService,
            AnalyticService analyticService)
        {
            _analyticService = analyticService;
            _currencyService = currencyService;
            _customizationShopConfig = customizationShopConfig;
            _dataService = dataService;
        }

        public void Initialize()
        {
            PopulateItemsMap();
            _internalPurchaseData = new PurchaseData();

            _purchsedItems.Clear();
            if (string.IsNullOrEmpty(_dataService.CustomizationSettings.PurchasesJSON.Value))
            {
                //Initialize empty or with defaults 
                
                //_dataService.CustomizationSettings.SelectedDecalName.Value = _customizationShopConfig.DefaultCarCarDecal.name;
                //Purchase(_customizationShopConfig.DefaultCarCarDecal);
                
                _dataService.CustomizationSettings.SelectedCarColorName.Value = _customizationShopConfig.DefaultCarColor.name;
                Purchase(_customizationShopConfig.DefaultCarColor, false);
                
                _dataService.CustomizationSettings.SelectedCarModelName.Value = _customizationShopConfig.DefaultCarModel.name;
                Purchase(_customizationShopConfig.DefaultCarModel, false);
                
                Debug.Log("Purchase Items on Init");
                _dataService.GiftSettings.GiftID.Value = 0;
                _dataService.GiftSettings.GiftProgress.Value = 0;
            }
            else
            {
                //Load
                //UnityEngine.Debug.Log(" ==> LOADING SHOP <==");
                //CustomDebugLog.Log(_dataService.CatSettings.PurchasesJSON.Value);
                _internalPurchaseData = JsonUtility.FromJson<PurchaseData>(_dataService.CustomizationSettings.PurchasesJSON.Value);
                foreach (var dItem in _internalPurchaseData.Items)
                {
                    //UnityEngine.Debug.Log($" LOADED : {dItem}");
                    _purchsedItems.Add(_map[dItem]);
                }
            }
        }
        private void AddToMap(ShopCarItemConfig carItem)
        {
            if (!_map.ContainsKey(carItem.name))
                _map.Add(carItem.name, carItem);
        }

        private void PopulateItemsMap()
        {
            _map.Clear();
        
            foreach (var item in _customizationShopConfig.CarDecals) 
                AddToMap(item);

            foreach (var item in _customizationShopConfig.CarColors) 
                AddToMap(item);
            
            foreach (var item in _customizationShopConfig.CarModels) 
                AddToMap(item);
            
            AddToMap(_customizationShopConfig.DefaultCarColor);
            //AddToMap(_customizationShopConfig.DefaultCarCarDecal);
            AddToMap(_customizationShopConfig.DefaultCarModel);
        }
        
        public void Purchase(ShopCarItemConfig carItem, bool isSendAnalyticEvent = true)
        {
            if (carItem == null)
                return;

            _purchsedItems.Add(carItem);

            var names = _purchsedItems.Select(t => t.name);
            if (_internalPurchaseData == null)
            {
                _internalPurchaseData = new PurchaseData();
            }

            _internalPurchaseData.Items = names.ToArray();
            _dataService.CustomizationSettings.PurchasesJSON.Value = JsonUtility.ToJson(_internalPurchaseData);
        
            if(isSendAnalyticEvent)
                _analyticService.LogPlayerBuyCustomizationItem(carItem.Name);
            
            OnBuyItem?.Invoke(carItem.Name);
        }
        
        public bool IsPurchased(ShopCarItemConfig carItem) => 
            _purchsedItems.Contains(carItem);

        public bool IsCanBuy(ShopCarItemConfig config)
        {
            switch (config.CurrencyType)
            {
                case ECurrencyType.Coins:
                    return _currencyService.IsEnoughCurrency(ECurrencyType.Coins, config.Price);
                case ECurrencyType.Diamonds:
                    return _currencyService.IsEnoughCurrency(ECurrencyType.Diamonds, config.Price);
            }
            return false;
        }

        public bool IsAnySkinAvailableForPurchase()
        {
            foreach (var item in _map.Values)
            {
                if(IsPurchased(item))
                    continue;
                if (IsCanBuy(item))
                    return true;
            }
            return false;
        }

        public bool IsItemSelected(string configName)
        {
            return configName == _dataService.CustomizationSettings.SelectedDecalName.Value || 
                configName == _dataService.CustomizationSettings.SelectedCarColorName.Value ||
                configName == _dataService.CustomizationSettings.SelectedCarModelName.Value;
        }

        public void ChangeCarElement(ShopCarItemConfig config)
        {
            switch (config)
            {
                /*case ShopCarCarDecalItemConfig decalItemConfig:
                    _dataService.CustomizationSettings.SelectedDecalName.Value = decalItemConfig.name;
                    OnChangeCarDecal?.Invoke(decalItemConfig);
                    break;*/
                case ShopCarCarModelItemConfig carModelItemConfig:
                    _dataService.CustomizationSettings.SelectedCarModelName.Value = carModelItemConfig.name;
                    OnChangeCarModel?.Invoke(carModelItemConfig);
                    break;
                case ShopCarCarColorItemConfig carColorItemConfig:
                    _dataService.CustomizationSettings.SelectedCarColorName.Value = carColorItemConfig.name;
                    OnChangeCarColor?.Invoke(carColorItemConfig);
                    break;
            }
        }
        public ShopCarCarColorItemConfig GetCurrentCarColorConfig()
        {
            var name = _dataService.CustomizationSettings.SelectedCarColorName.Value;
            return _map[name] as ShopCarCarColorItemConfig;
        }

        public ShopCarCarModelItemConfig GetCurrentCarModelConfig()
        {
            var name = _dataService.CustomizationSettings.SelectedCarModelName.Value;
            return _map[name] as ShopCarCarModelItemConfig;
        }
        
        /*public void ChangeSkinElementToDefault(ShopCarItemConfig config)
{
    switch (config)
    {
        case ShopCarCarDecalItemConfig decalItemConfig:
            _dataService.CustomizationSettings.SelectedDecalName.Value = _customizationShopConfig.DefaultCarCarDecal.name;
           // _player.SnakeController.CatSkinController.ChangeBaseSkin(_shopConfig.DefaultCarDecal, _player.SnakeController.CurrentCatColor);
           // ShopView.DummyCatController.PreviewCatSkinController.ChangeBaseSkin(_shopConfig.DefaultCarDecal, SHOP_CAT_COLOR);
            break;
        case ShopCarCarModelItemConfig carModelItemConfig:
            _dataService.CustomizationSettings.SelectedCarModelName.Value = _customizationShopConfig.DefaultCarCarModel.name;
            // _player.SnakeController.CatSkinController.ChangeBaseSkin(decalItemConfig, _player.SnakeController.CurrentCatColor);
            //ShopView.DummyCatController.PreviewCatSkinController.ChangeBaseSkin(decalItemConfig, SHOP_CAT_COLOR);
            break;
        case ShopCarCarColorItemConfig carColorItemConfig:
            _dataService.CustomizationSettings.SelectedCarColorName.Value = _customizationShopConfig.DefaultCarCarColor.name;
            //_carService.ChangeCarColor(_customizationShopConfig.DefaultCarCarColor);
            _uiGarageService.ChangeCarColor(_customizationShopConfig.DefaultCarCarColor);
            break;
    }
}*/
        public void Clear()
        {
        }
    }
}