using Game.Scripts.Customization.ItemConfigs;
using UnityEngine;

namespace Game.Scripts.Customization
{
    [CreateAssetMenu(fileName = nameof(CustomizationShopConfig), menuName = "Customization/" + nameof(CustomizationShopConfig))]
    public class CustomizationShopConfig : ScriptableObject
    {
        public ShopCarCarModelItemConfig DefaultCarModel;
        //public ShopCarCarDecalItemConfig DefaultCarCarDecal;
        public ShopCarCarColorItemConfig DefaultCarColor;

        [Header("BaseSkins")]
        public ShopCarCarModelItemConfig[] CarModels;
        //public ShopCarCarDecalItemConfig[] CarDecals;
        public ShopCarCarColorItemConfig[] CarDecals;
        public ShopCarCarColorItemConfig[] CarColors;
    }
}
