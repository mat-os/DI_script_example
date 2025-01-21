using System;
using System.Collections.Generic;
using Game.Scripts.Customization;
using UnityEngine;

namespace Game.Scripts.UI.Widgets.Customization
{
    public class UIShopTabManager : MonoBehaviour
    {
        public Action<int> OnChangeShopTab;
        
        [SerializeField] private List<UIShopTabButton> _tabButtons;
        [SerializeField] private List<GameObject> _shopSections;
    
        private int _activeTabIndex = 0;
        private UISwipeGalleryPage[] _uiSwipeGalleryPages;
        
        private CustomizationShopService _customizationShopService;

        public void Init(UISwipeGalleryPage[] uiSwipeGalleryPages, CustomizationShopService customizationShopService)
        {
            _customizationShopService = customizationShopService;
            _uiSwipeGalleryPages = uiSwipeGalleryPages;
            
            if (_tabButtons.Count != _shopSections.Count)
            {
                Debug.LogError("The number of tab buttons and shop sections should match.");
                return;
            }
            for (int i = 0; i < _tabButtons.Count; i++)
            {
                _tabButtons[i].Init(i, this);
            }
            foreach (var shopSection in _shopSections)
            {
                shopSection.SetActive(false);
            }
        }
        public void Show()
        {
            SelectTab(_activeTabIndex);
            _uiSwipeGalleryPages[_activeTabIndex].Show();
        }

        public void SelectTab(int tabIndex)
        {
            _shopSections[_activeTabIndex].SetActive(false);
            _shopSections[tabIndex].SetActive(true);

            _tabButtons[_activeTabIndex].SetIsActiveTab(false);
            _tabButtons[tabIndex].SetIsActiveTab(true);

            _activeTabIndex = tabIndex;
            OnChangeShopTab?.Invoke(tabIndex);
        }

        public void Refresh()
        {
            foreach (var page in _uiSwipeGalleryPages)
            {
                page.RefreshAllItems();
            }
        }
    }
}