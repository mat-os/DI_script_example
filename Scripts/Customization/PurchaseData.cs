using System;

namespace Game.Scripts.Customization
{
    [Serializable]
    public class PurchaseData
    {
        public string[] Items;

        public PurchaseData()
        {
            Items = Array.Empty<string>();
        }
    }
}