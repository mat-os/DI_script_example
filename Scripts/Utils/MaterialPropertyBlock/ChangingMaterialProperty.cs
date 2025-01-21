using DG.Tweening;
using Game.Scripts.Constants;
using UnityEngine;

namespace Game.Scripts.Utils.MaterialPropertyBlock
{
    public class ChangingMaterialProperty : MonoBehaviour
    {
        [field: SerializeField] public Renderer Renderer { get; private set; }

        private UnityEngine.MaterialPropertyBlock _materialPropertyBlock;
        private Sequence _changingColorSequence;

        private void Awake()
        {
            _materialPropertyBlock = new UnityEngine.MaterialPropertyBlock();
            if (Renderer == null)
                Renderer = GetComponent<Renderer>();
        }

        public void SetMaterialData(int materialIndex, MaterialData materialData)
        {
            Renderer.GetPropertyBlock(_materialPropertyBlock, materialIndex);
            _materialPropertyBlock.SetColor(ShaderProperty.Color, materialData.MainColor);
            _materialPropertyBlock.SetColor(ShaderProperty.ShadingColor, materialData.ShadingColor);
            _materialPropertyBlock.SetColor(ShaderProperty.ShadowColor, materialData.ShadowColor);
            _materialPropertyBlock.SetFloat(ShaderProperty.Emission, materialData.Emission);
            _materialPropertyBlock.SetFloat(ShaderProperty.RampThreshold, materialData.RampThreshold);
            _materialPropertyBlock.SetFloat(ShaderProperty.RampSmooth, materialData.RampSmooth);
            _materialPropertyBlock.SetFloat(ShaderProperty.MatcapIntensity, materialData.MatcapIntensity);

            if (materialData.MainTexture != null)
                _materialPropertyBlock.SetTexture(ShaderProperty.MainTex, materialData.MainTexture);
            if (materialData.MatcapTexture != null)
                _materialPropertyBlock.SetTexture(ShaderProperty.MatcapTex, materialData.MatcapTexture);

            Renderer.SetPropertyBlock(_materialPropertyBlock, materialIndex);
        }

        public void SetAnimationColor(MaterialData materialData, AnimationParameterConfig config)
        {
            _changingColorSequence?.Kill();
            _changingColorSequence = DOTween.Sequence();

            Renderer.GetPropertyBlock(_materialPropertyBlock);

            /*if (materialData.MainTexture != null)
                SetTexture(materialData.MainTexture, ShaderProperty.MainTex);*/
            
            if (materialData.MainTexture != null)
                SetTexture(materialData.MainTexture, ShaderProperty.MainTex);
            if (materialData.MatcapTexture != null)
                SetTexture(materialData.MatcapTexture, ShaderProperty.MatcapTex);
            
            int mainColorId = ShaderProperty.Color;
            int shadingColorId = ShaderProperty.ShadingColor;
            int shadowColorId = ShaderProperty.ShadowColor;
            int emissionId = ShaderProperty.Emission;
            int rampThresholdId = ShaderProperty.RampThreshold;
            int rampSmoothId = ShaderProperty.RampSmooth;
            int matcapIntensityId = ShaderProperty.MatcapIntensity;

            Color startMainColor = _materialPropertyBlock.GetColor(mainColorId);
            Color startShadingColor = _materialPropertyBlock.GetColor(shadingColorId);
            Color startShadowColor = _materialPropertyBlock.GetColor(shadowColorId);
            float startEmission = _materialPropertyBlock.GetFloat(emissionId);
            float startRampThreshold = _materialPropertyBlock.GetFloat(rampThresholdId);
            float startRampSmoothId = _materialPropertyBlock.GetFloat(rampSmoothId);
            float startMatcapIntensity = _materialPropertyBlock.GetFloat(matcapIntensityId);

            float duration = config.Duration;
            Ease ease = config.Ease;

            _changingColorSequence
                .Join(DOVirtual.Color(startMainColor, materialData.MainColor, duration,
                    color => SetColor(color, mainColorId)).SetEase(ease))
                .Join(DOVirtual.Color(startShadingColor, materialData.ShadingColor, duration,
                    color => SetColor(color, shadingColorId)).SetEase(ease))
                .Join(DOVirtual.Color(startShadowColor, materialData.ShadowColor, duration,
                    color => SetColor(color, shadowColorId)).SetEase(ease))
                .Join(DOVirtual.Float(startEmission, materialData.Emission, duration,
                    value => SetFloat(value, emissionId)).SetEase(ease))
                .Join(DOVirtual.Float(startRampThreshold, materialData.RampThreshold, duration,
                    value => SetFloat(value, rampThresholdId)).SetEase(ease))
                .Join(DOVirtual.Float(startRampSmoothId, materialData.RampSmooth, duration,
                    value => SetFloat(value, rampSmoothId)).SetEase(ease))
                .Join(DOVirtual.Float(startMatcapIntensity, materialData.MatcapIntensity, duration,
                    value => SetFloat(value, matcapIntensityId)).SetEase(ease));
        }

        private void SetColor(Color color, int propertyId)
        {
            Renderer.GetPropertyBlock(_materialPropertyBlock);
            _materialPropertyBlock.SetColor(propertyId, color);
            Renderer.SetPropertyBlock(_materialPropertyBlock);
        }

        private void SetFloat(float value, int propertyId)
        {
            Renderer.GetPropertyBlock(_materialPropertyBlock);
            _materialPropertyBlock.SetFloat(propertyId, value);
            Renderer.SetPropertyBlock(_materialPropertyBlock);
        }

        private void SetTexture(Texture texture, int propertyId)
        {
            Renderer.GetPropertyBlock(_materialPropertyBlock);
            _materialPropertyBlock.SetTexture(propertyId, texture);
            Renderer.SetPropertyBlock(_materialPropertyBlock);
        }

        private void OnDestroy()
        {
            _changingColorSequence?.Kill();
        }
    }
}