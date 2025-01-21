using System;
using System.Collections.Generic;
using System.Linq;
using AssetKits.ParticleImage;
using Game.Scripts.Core.CurrencyService;
using UnityEngine;

namespace Game.Configs.PrefabsCollection.PrefabsSettings
{
    [Serializable]
    public class UIFXSettings
    {
        [SerializeField] private List<UICurrencyFXSettings> _uiCurrencyFX;
        
        public ParticleImage GetCurrencyEffect(ECurrencyType type) =>
            _uiCurrencyFX.FirstOrDefault(effect => effect.CurrencyType == type)?.Effect;
    }
    
    [Serializable]
    public class UICurrencyFXSettings
    {
        [field:SerializeField] public ECurrencyType CurrencyType;
        [field:SerializeField] public ParticleImage Effect;
    }
}