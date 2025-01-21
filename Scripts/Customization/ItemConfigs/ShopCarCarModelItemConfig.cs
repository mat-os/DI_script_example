using Configs;
using Game.Scripts.LevelElements.Car;
using UnityEngine;

namespace Game.Scripts.Customization.ItemConfigs
{
    [CreateAssetMenu(fileName = nameof(ShopCarCarModelItemConfig), menuName = "Customization/" + nameof(ShopCarCarModelItemConfig))]
    public class ShopCarCarModelItemConfig : ShopCarItemConfig
    {
        public ECarType CarType;
    }
}