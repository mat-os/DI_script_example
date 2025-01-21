using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.Widgets.Customization
{
    public class UIShopTabButton : MonoBehaviour
    {
        [SerializeField] private Image _background;
        [SerializeField] private Sprite _activeSprite;
        [SerializeField] private Sprite _inactiveSprite;
        [SerializeField] private GameObject _text;
        [SerializeField] private Vector3 _scaleOnActive;
        
        private int _tabIndex;
        private UIShopTabManager _uiShopTabManager;
        private Vector3 _origScale;

        public void SetIsActiveTab(bool isActive)
        {
            _background.sprite = isActive ? _activeSprite : _inactiveSprite;
            //_text.SetActive(isActive);
            var scale = isActive ? _scaleOnActive : _origScale;
            transform.localScale = scale;
            _background.SetNativeSize();
        }

        public void Init(int tabIndex, UIShopTabManager uiShopTabManager)
        {
            _origScale = transform.localScale;
            _uiShopTabManager = uiShopTabManager;
            _tabIndex = tabIndex;
            SetIsActiveTab(false);
        }

        public void G_SelectTab()
        {
            _uiShopTabManager.SelectTab(_tabIndex);
        }
    }
}