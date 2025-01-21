using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = nameof(EnvironmentConfig), menuName = "Configs/Level/" + nameof(EnvironmentConfig))]
    [InlineEditor]
    public class EnvironmentConfig : ScriptableObject
    {
        [Header("Skybox")]
        public Material SkyboxMaterial;
        public FogSettings FogSettings;

        [Header("Lighting")]
        public GradientLightingSettings GradientLightingSettings;
    }
}
[Serializable]
public class FogSettings
{
    [Header("Fog Settings")]
    public bool IsFogEnabled = true;

    public Color FogColor = Color.white;
    public float FogStart = 50;
    public float FogEnd = 81;
}
[Serializable]
public class GradientLightingSettings
{
    public Color SkyColor = Color.gray;
    public Color EquatorColor = Color.gray;
    public Color GroundColor = Color.black;
}