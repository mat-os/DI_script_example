using Game.Scripts.Core.CurrencyService;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.Customization
{
    [CreateAssetMenu(fileName = nameof(ShopCarItemConfig), menuName = "Cat Shop/" + nameof(ShopCarItemConfig), order = 0)]
    public class ShopCarItemConfig : ScriptableObject
    {
        [Header("Icon")]
        [PreviewField]
        [SerializeField] private Sprite _icon;
        [SerializeField] private Vector2 _iconSize;

        //[PreviewField]
        //[SerializeField] private Sprite _inactiveIcon;
    

        [Header("Price")]
        [SerializeField] private ECurrencyType _currencyType;
        [SerializeField] private int _price;

        [Header("Description")]
        [SerializeField] private string _name;
        [TextArea]
        [SerializeField] private string _description;

    
        public string Description => _description;
        public string Name => _name;
        public int Price => _price;
        public Sprite Icon => _icon;

        public Vector2 IconSize => _iconSize;

        //public Sprite InactiveIcon => _inactiveIcon;
        public ECurrencyType CurrencyType => _currencyType;
    }
}