using System;
using Game.Scripts.Core.CurrencyService;
using Game.Scripts.Customization;
using Game.Scripts.Infrastructure.Services;
using Game.Scripts.Infrastructure.States;
using Game.Scripts.UI.Screens.Serviсes;
using UnityEngine;
using Zenject;

namespace Game.Scripts.UI.Widgets.Customization
{
    public class CustomizationUI : MonoBehaviour
    {
        public Action<int> OnChangeShopTab;
        
        [SerializeField] private UIScrollViewPage GridPrefab;
        [SerializeField] private UIShopItemView ItemPrefab;
    
        [SerializeField] private UIShopTabManager _uiShopTabManager;
        [SerializeField] private PreviewRotator _previewRotator;
        [SerializeField] private UISwipeGalleryPage[] Pages;
        [SerializeField] private UISwipeGalleryPage CurrentPage = null;
        
        [Header("Currency View")]
        //[SerializeField] private CurrencyView _coinCurrencyView;
        //private CurrencyViewPresenter _coinCurrencyViewPresenter;
        /*[SerializeField] private CurrencyView _diamondCurrencyView;
        private CurrencyViewPresenter _diamondCurrencyViewPresenter;*/
        
        private PlayerService _playerService;
        private CustomizationShopService _customizationShopService;
        private DataService _dataService;
        private CustomizationShopConfig _customizationShopConfig;
        private CurrencyService _currencyService;
        private PopupService _popupService;
        
        private bool _isInitialized;
        //private AudioService _audioService;

        [Inject]
        public void Construct(CustomizationShopService customizationShopService, 
             DataService dataService,
             CustomizationShopConfig customizationShopConfig, 
             CurrencyService currencyService, 
             PlayerService playerService, 
             PopupService popupService,
             VibrationService vibrationService)
        {
            //_audioService = audioService;
            _popupService = popupService;
            _currencyService = currencyService;
            _customizationShopConfig = customizationShopConfig;
            _dataService = dataService;
            _customizationShopService = customizationShopService;
            _playerService = playerService;

            foreach (var page in Pages)
            {
                page.Construct(_playerService, _customizationShopConfig, _customizationShopService,_currencyService);
            }
            /*_coinCurrencyViewPresenter = new CurrencyViewPresenter(currencyService, _coinCurrencyView.CurrencyType, vibrationService, _coinCurrencyView.AnimationParameterConfig);
            _coinCurrencyView.Construct(_coinCurrencyViewPresenter);     */
            /*_diamondCurrencyViewPresenter = new CurrencyViewPresenter(currencyService, _diamondCurrencyView.CurrencyType, vibrationService, _diamondCurrencyView.AnimationParameterConfig);
            _diamondCurrencyView.Construct(_diamondCurrencyViewPresenter);*/
        }
        public void OnCreate()
        {
            //_catPreviewRotator.Init(_shopService.ShopView.DummyCatController.transform);
            
            var carColors = _customizationShopConfig.CarColors;
            var carDecals = _customizationShopConfig.CarDecals;
            var carModels = _customizationShopConfig.CarModels;

            Pages[0].PopulateItems(GridPrefab, ItemPrefab, carModels);
            Pages[1].PopulateItems(GridPrefab, ItemPrefab, carColors);
            Pages[2].PopulateItems(GridPrefab, ItemPrefab, carDecals );
  
            if (CurrentPage == null) 
                CurrentPage = Pages[0];

            _uiShopTabManager.Init(Pages, _customizationShopService);

            _currencyService.OnCurrencyChanged += CurrencyChangedHandle;
            _uiShopTabManager.OnChangeShopTab += ChangeShopTabHandle;

            /*_coinCurrencyView.Initialize();
            _coinCurrencyViewPresenter.Initialize();*/
            /*_diamondCurrencyView.Initialize();
            _diamondCurrencyViewPresenter.Initialize();*/

        }

        private void ChangeShopTabHandle(int tabIndex)
        {
            OnChangeShopTab?.Invoke(tabIndex);
        }

        public void OnOpenStart()
        {
            _previewRotator.SetDefaultRotation();

            //TODO:
            //var startCatColor = CatColor.Yellow;
            //_shopService.ShopView.DummyCatController.PreviewCatSkinController.ApplyCurrentSkin(_shopService, _dataService.CatSettings, startCatColor);
            //_shopService.ShopView.DummyCatController.SetActiveCamera(true);
            
            _uiShopTabManager.Show();
            RefreshPurchaseButtonsState();
            
            //_audioService.SetGameMusicParameter(EMusicState.ShopUI);
        }

        private void CurrencyChangedHandle(ECurrencyType arg1, int arg2) => 
            RefreshPurchaseButtonsState();
        
        public void OnCloseComplete()
        {
            //TODO:
            //_shopService.ShopView.DummyCatController.SetActiveCamera(false);
            
            //_audioService.SetGameMusicParameter(EMusicState.Menu);
        }

        private void OnGridChanged(Transform obj)
        {
            RefreshPurchaseButtonsState();
        }

        public void RefreshPurchaseButtonsState()
        {
            if (_isInitialized)
            {
                _uiShopTabManager.Refresh();
            }
        }

        public void SetRotationTarget(Transform rotationTarget)
        {
            _previewRotator.SetTarget(rotationTarget);
        }
    }
}