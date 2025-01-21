using System;
using System.Collections.Generic;
using Game.Scripts.Core.CurrencyService;
using Game.Scripts.Customization;
using Game.Scripts.Infrastructure.States;
using UnityEngine;

namespace Game.Scripts.UI.Widgets.Customization
{
    public class UISwipeGalleryPage : MonoBehaviour
    {
        public Action<Transform> OnGridChange;

        public RectTransform GridsContainer;
        //public RectTransform Root;

        public bool IsInitialized { get; private set; } = false;
        public RectTransform CurrentGridDebug { get; set; }
    
        private int _currentItemViewID = 0;
        //private UIShopItemView _picked;
        private bool _isBusy = false;
        private UIScrollViewPage _page;
        private List<UIShopItemView> _shopItems = new List<UIShopItemView>();
    
        private PlayerService _playerService;
        private CustomizationShopConfig _customizationShopConfig;
        private CustomizationShopService _customizationShopService;
        private CurrencyService _currencyService;

        public void Construct(PlayerService playerService, CustomizationShopConfig customizationShopConfig,
            CustomizationShopService customizationShopService, CurrencyService currencyService)
        {
            _currencyService = currencyService;
            _customizationShopService = customizationShopService;
            _customizationShopConfig = customizationShopConfig;
            _playerService = playerService;
        }
        /*private void OnDisable()
        {
            if (_picked != null)
            {
                _picked.Purchase();
                _picked = null;
            }
        }*/

        public void Show()
        {
            _page.Show();
            RefreshAllItems();
        }

        public void RefreshAllItems()
        {
            foreach (var item in _shopItems)
            {
                item.RefreshView();
            }
        }

        public void PopulateItems(UIScrollViewPage gridPrefab, 
            UIShopItemView shopItemPrefab, 
            ShopCarItemConfig[] items)
        {
            if (!IsInitialized)
            {
                _shopItems.Clear();
                GridsContainer.Clear();
            
                _page = Instantiate(gridPrefab, GridsContainer);
                _page.Viewport.transform.Clear();

                foreach (var item in items)
                {
                    var uiShopItemView = Instantiate(shopItemPrefab, _page.Viewport.transform);
                    uiShopItemView.Init(item, _playerService, _customizationShopService, _customizationShopConfig, _currencyService);
                
                    if(_shopItems.Contains(uiShopItemView) == false)
                        _shopItems.Add(uiShopItemView);
                }

                CurrentGridDebug = GridsContainer.GetChild(0) as RectTransform;
                OnGridChange?.Invoke(CurrentGridDebug);

                IsInitialized = true;
            }
        }



    }
}
