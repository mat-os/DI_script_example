using UnityEngine;

namespace Game.Scripts.Constants
{
    public static class ShaderProperty
    {
        public static readonly int Color = Shader.PropertyToID("_Color");
        public static readonly int ShadowColor = Shader.PropertyToID("_ShadowColor");
        public static readonly int ShadingColor = Shader.PropertyToID("_ShadingColor");
        public static readonly int MainTex = Shader.PropertyToID("_MainTex");
        public static readonly int Emission = Shader.PropertyToID("_Emission");
        public static readonly int RampThreshold = Shader.PropertyToID("_RampThreshold");
        public static readonly int RampSmooth = Shader.PropertyToID("_RampSmooth");
        public static readonly int MatcapIntensity = Shader.PropertyToID("_Matcap_Intensity");
        public static readonly int MatcapTex = Shader.PropertyToID("_Matcap");
    }
}