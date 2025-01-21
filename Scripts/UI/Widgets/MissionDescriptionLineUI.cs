using System.Text.RegularExpressions;
using Configs;
using Game.Scripts.Configs.Level;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.Widgets
{
    public class MissionDescriptionLineUI : MonoBehaviour
    {
        [SerializeField]private Image _starImage;
        [SerializeField]private Sprite _activeSprite;
        [SerializeField]private Sprite _inactiveSprite;
        [SerializeField]private TMP_Text _text;
        [SerializeField]private Color _activeTextColor;
        [SerializeField]private Color _inactiveTextColor;

        public void SetupLine(MissionConfig missionConfig, LevelMission levelMission, bool isCompleted)
        {
            int targetValue = levelMission.TargetValue;

            // Заменяем {x} в шаблоне на конкретное значение
            string updatedDescription = missionConfig.TextForMissionDescriptionLineUI.Replace("{x}", targetValue.ToString());

            if (isCompleted)
            {
                // Удаляем любые теги <color> и <link> с помощью регулярных выражений
                updatedDescription = Regex.Replace(updatedDescription, @"<color.*?>|</color>|<link.*?>|</link>", "");
            }
            _text.text = updatedDescription;

            _starImage.sprite = isCompleted ? _activeSprite : _inactiveSprite;
            _text.color = isCompleted ? _inactiveTextColor : _activeTextColor;
        }
    }
}