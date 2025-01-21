using Configs;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class PostProcessingService 
{
    private PostProcessorSettings _settings;
    private PostProcessor _postProcessor;

    private readonly PostProcessingConfig _config;

    private float _defaultBloom;

    private Tweener _bloomTween;

    [Inject]
    public PostProcessingService(EffectsConfig effectsConfig)
    {
        _config = effectsConfig.PostProcessingConfig;
    }

    public void SetPostProcessing(PostProcessor postProcessor)
    {
        _postProcessor = postProcessor;
        _settings = _postProcessor.Settings;
        _defaultBloom = _postProcessor.Settings.BloomIntensity;
    }

    public void StartJumpPostProcessing()
    {
        _bloomTween?.Kill();
        _bloomTween = DOVirtual.Float(_settings.BloomIntensity, _config.BloomIntensityOnJump,
            _config.ShowBloomAnimationParameter.Duration,
            x => { _settings.BloomIntensity = x; }).SetEase(_config.ShowBloomAnimationParameter.Ease).SetUpdate(true);
    }

    public void EndJumpPostProcessing()
    {
        _bloomTween?.Kill();
        DOVirtual.Float(_settings.BloomIntensity, 0, _config.ShowBloomAnimationParameter.Duration,
            x => { _settings.BloomIntensity = x; }).SetEase(_config.ShowBloomAnimationParameter.Ease).SetUpdate(true);
    }
}