using UnityEngine;

namespace Game.Scripts.Customization.ItemConfigs
{
    [CreateAssetMenu(fileName = nameof(ShopCarCarDecalItemConfig), menuName = "Customization/" + nameof(ShopCarCarDecalItemConfig))]
    public class ShopCarCarDecalItemConfig : ShopCarItemConfig
    {
        [field:SerializeField]public Material CarMaterial { get; private set; }

    }
}