using System;
using Coffee.UIEffects;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelectButtonUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _levelNameText;
    [SerializeField] private Image[] _starImages;
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private Image _trophyIcon;
    [SerializeField] private Image _levelImg;
    [SerializeField] private UIShiny _trophyIconShiny;
    [SerializeField] private UIShiny _bgIconShiny;
    [SerializeField] private Button _button;
    [SerializeField] private GameObject _lockedLevel;

    [SerializeField] private Sprite _activeStarSprite;
    [SerializeField] private Sprite _inactiveStarSprite;
    
    [SerializeField] private Color _activeStarColor;
    [SerializeField] private Color _inactiveStarColor;
    
    [SerializeField] private Sprite _activeBgSprite;
    [SerializeField] private Sprite _inactiveBgSprite;
    
    [SerializeField] private Sprite _activeTrophySprite;
    [SerializeField] private Sprite _inactiveTrophySprite;
    
    public void Setup(int levelNumber, int starsEarned, bool isLocked, bool isHasTrophy, string levelName, Sprite levelImage)
    {
        if (isLocked)
        {
            _levelNameText.text = $"{levelNumber.ToString()}. ? ? ?";
        }
        else
        {
            _levelNameText.text = $"{levelNumber.ToString()}. {levelName}";
        }

        // Показываем звезды в зависимости от прогресса
        for (int i = 0; i < _starImages.Length; i++)
        {
            var isActive = i < starsEarned;
            _starImages[i].sprite = isActive ? _activeStarSprite : _inactiveStarSprite;
            _starImages[i].color = isActive ? _activeStarColor : _inactiveStarColor;
            _starImages[i].gameObject.SetActive(!isLocked); 
        }

        _backgroundImage.sprite = isLocked ? _inactiveBgSprite : _activeBgSprite;
        _button.interactable = !isLocked;

        _trophyIcon.sprite = isHasTrophy ? _activeTrophySprite : _inactiveTrophySprite;
        _trophyIcon.color = isHasTrophy ? _activeStarColor : _inactiveStarColor;
        _trophyIcon.gameObject.SetActive(!isLocked);
        _trophyIconShiny.enabled = (isHasTrophy);
        _bgIconShiny.enabled = (!isLocked);
        _levelNameText.gameObject.SetActive(!isLocked);
        
        _lockedLevel.SetActive(isLocked);
        if(levelImage != null)
            _levelImg.sprite = levelImage;
    }

    public void OnClick(Action callback)
    {
        _button.onClick.AddListener(() => callback());
    }
}