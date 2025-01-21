using System;
using Game.Scripts.Core.CurrencyService;
using Game.Scripts.Customization;
using Game.Scripts.Infrastructure.States;
using Game.Scripts.Utils.Debug;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.Widgets.Customization
{
    public class UIShopItemView : MonoBehaviour
    {
        public static event Action<ShopCarItemConfig> OnPicked;
    
        [SerializeField] private Image _itemIcon;
        [SerializeField] private TMP_Text _nameText;

        [Header("Panel")]
        [SerializeField] private GameObject _activePanel;
        [SerializeField] private GameObject _purchasedPanel;
        [SerializeField] private GameObject _lockedPanel;
        
        [Header("Buttons")]
        [SerializeField] private Button _buyButton;
        [SerializeField] private TMP_Text _costText;
    
        private ShopCarItemConfig _config;
        private PlayerService _playerService;
        private CustomizationShopService _customizationShopService;
        private CustomizationShopConfig _customizationShopConfig;
        private CurrencyService _currencyService;
        //private AudioService _audioService;

        public void Init(ShopCarItemConfig config,
            PlayerService playerService,
            CustomizationShopService customizationShopService,
            CustomizationShopConfig customizationShopConfig,
            CurrencyService currencyService)
        {
            //_audioService = audioService;
            _currencyService = currencyService;
            _customizationShopConfig = customizationShopConfig;
            _customizationShopService = customizationShopService;
            _playerService = playerService;
            _config = config;
        
            OnPicked += OnItemPicked;
            _buyButton.onClick.AddListener(G_Pick);
            RefreshView();
        }
        private void OnItemPicked(ShopCarItemConfig obj)
        {
            RefreshView();
        }

        public bool IsPurchased() =>
            _customizationShopService.IsPurchased(_config);

        public void RefreshView()
        {
            if (_customizationShopService != null)
            {
                var isPurchased = _customizationShopService.IsPurchased(_config);
                var isSelected = _customizationShopService.IsItemSelected(_config.name);;
                var isCanBuy = _customizationShopService.IsCanBuy(_config);

                _itemIcon.sprite = _config.Icon;
                _itemIcon.rectTransform.sizeDelta = _config.IconSize;

                _buyButton.interactable = isCanBuy || isPurchased;
                _costText.text = _config.Price.ToString();
                _nameText.text = _config.Name;
            
                _activePanel.SetActive(isPurchased && isSelected);
                _purchasedPanel.SetActive(isPurchased && !isSelected);
                _lockedPanel.SetActive(isPurchased == false);
            }
        }

        public void G_Pick()
        {
            var isPurchased = _customizationShopService.IsPurchased(_config);
            if (isPurchased)
            {
                ChangeSkinElement();
                //_audioService.PlaySoundOneShot(SoundFxConstants.UI_SHOP_WEAR);
            }
            else
            {
                var currencyToSpent = _config.CurrencyType == ECurrencyType.Coins
                    ? ECurrencyType.Coins
                    : ECurrencyType.Diamonds;
            
                if (_currencyService.TrySpendCurrency(currencyToSpent, _config.Price))
                {
                    _customizationShopService.Purchase(_config);
                    ChangeSkinElement();
                    //_audioService.PlaySoundOneShot(SoundFxConstants.UI_SHOP_BUY);
                }
            }
            OnPicked?.Invoke(_config);
        }
        private void ChangeSkinElement()
        {
            _customizationShopService.ChangeCarElement(_config);
        }

        public void Purchase()
        {
            CustomDebugLog.Log("Purchase");
            _customizationShopService.Purchase(_config);
            G_Pick();
        }


        private void OnDestroy()
        {
            _buyButton.onClick.RemoveAllListeners();
            OnPicked -= OnItemPicked;
        }
        /*public void BlinkLockIcon()
    {
        LockIcon.DOColor(HighLight, 0.1f).OnComplete(() =>
        {
            LockIcon.DOColor(Normal, 0.1f);
        });
    }*/
    }
}
