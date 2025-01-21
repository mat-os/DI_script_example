using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.Customization.ItemConfigs
{
    [CreateAssetMenu(fileName = nameof(ShopCarCarColorItemConfig), menuName = "Customization/" + nameof(ShopCarCarColorItemConfig))]
    public class ShopCarCarColorItemConfig : ShopCarItemConfig
    {
        //[field:SerializeField]public ECarColor CarColor{ get; private set; }
        [field:SerializeField]public Material CarMaterial { get; private set; }
    }
}