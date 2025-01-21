using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.Utils.MaterialPropertyBlock
{
    [Serializable]
    public struct MaterialData
    {
        [field: SerializeField] public Color MainColor { get; private set; }
        [field: SerializeField] public Color ShadingColor { get; private set; }
        [field: SerializeField] public Color ShadowColor { get; private set; }
        [field: PreviewField]
        [field: SerializeField] public Texture MainTexture { get; private set; }
        [field: SerializeField, Range(0.0f, 1.0f)] public float Emission { get; private set; }
        [field: SerializeField, Range(0.0f, 1.0f)] public float RampThreshold { get; private set; }
        [field: SerializeField, Range(0.0f, 1.0f)] public float RampSmooth { get; private set; }
        [field: PreviewField]
        [field: SerializeField] public Texture MatcapTexture { get; private set; }
        [field: SerializeField, Range(0.0f, 1.0f)] public float MatcapIntensity { get; private set; }
    }
}