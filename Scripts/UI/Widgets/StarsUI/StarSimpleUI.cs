using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class StarSimpleUI : MonoBehaviour, IStarUI
{
    [SerializeField] private Image _starImage;       // Ссылка на изображение звезды
    [SerializeField] private Sprite _activeSprite;  // Спрайт при активации
    [SerializeField] private Sprite _inactiveSprite; // Спрайт при деактивации
    
    [Header("Animation")]
    [SerializeField] private float _activeStarScaleEffect = 1.2f;
    [SerializeField] private Vector3 _activeStarScale;
    [SerializeField] private float _starActivateAnimationDuration = 0.4f;
    
    public RectTransform StarRectTransform { get; private set; }

    private void Awake()
    {
        StarRectTransform = GetComponent<RectTransform>();
    }

    public void PlayActivateAnimation()
    {
        _starImage.sprite = _activeSprite;
        _starImage.transform.DOScale(Vector3.one * _activeStarScaleEffect, _starActivateAnimationDuration)
            .SetEase(Ease.OutBack)
            .OnComplete(() => _starImage.transform.DOScale(_activeStarScale, 0.2f).SetEase(Ease.InOutQuad));

        _starImage.transform.DORotate(new Vector3(0, 0, 360), _starActivateAnimationDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.OutCubic);

    }

    public void Deactivate()
    {
        _starImage.sprite = _inactiveSprite;
        transform.localScale = Vector3.one;
    }

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}