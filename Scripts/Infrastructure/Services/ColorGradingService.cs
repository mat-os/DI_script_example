using Configs;
using DG.Tweening;
using Zenject;

namespace Game.Scripts.Infrastructure.Services
{
    public class ColorGradingService
    {
        private AutoMobileColorGrading _colorGrading;
        
        private readonly ColorGradingConfig _config;
        
        private float _defaultContrast;
        
        private Tweener _vignetteTween;
        private Tweener _contrastTween;

        [Inject]
        public ColorGradingService(EffectsConfig effectsConfig)
        {
            _config = effectsConfig.ColorGradingConfig;
        }
        public void SetColorGrading(AutoMobileColorGrading colorGrading)
        {
            _colorGrading = colorGrading;
            _defaultContrast = _colorGrading.Contrast;
        }

        public void StartColorGrading()
        {
            _vignetteTween?.Kill();
            _vignetteTween = DOVirtual.Float(_colorGrading.vignetteIntensity, _config.VignetteIntensityOnJump, _config.ShowVignetteAnimationParameter.Duration,
                (x) =>
                {
                    _colorGrading.vignetteIntensity = x;
                }).SetEase(_config.ShowVignetteAnimationParameter.Ease).SetUpdate(true);

            _contrastTween?.Kill();
            _contrastTween = DOVirtual.Float(_colorGrading.Contrast, _config.ContrastOnJump, _config.ShowVignetteAnimationParameter.Duration,
                (x) =>
                {
                    _colorGrading.Contrast = x;
                }).SetEase(_config.ShowVignetteAnimationParameter.Ease).SetUpdate(true);
        }
        public void EndColorGrading()
        {
            _vignetteTween?.Kill();
            DOVirtual.Float(_colorGrading.vignetteIntensity, 0, _config.HideVignetteAnimationParameter.Duration,
                (x) =>
                {
                    _colorGrading.vignetteIntensity = x;
                }).SetEase(_config.HideVignetteAnimationParameter.Ease).SetUpdate(true);
            
            _contrastTween?.Kill();
            DOVirtual.Float(_colorGrading.Contrast, _defaultContrast, _config.HideVignetteAnimationParameter.Duration,
                (x) =>
                {
                    _colorGrading.Contrast = x;
                }).SetEase(_config.HideVignetteAnimationParameter.Ease).SetUpdate(true);
        }
    }
}