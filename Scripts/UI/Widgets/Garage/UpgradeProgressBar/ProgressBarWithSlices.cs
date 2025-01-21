using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.Widgets.Garage.UpgradeProgressBar
{
    public class ProgressBarWithSlices : ProgressBar
    {
        [SerializeField] private Transform _parent;
        [SerializeField] private Image _progressPart;
        
        [SerializeField] private Sprite _upgradedSprite;
        [SerializeField] private Sprite _notUpgradedSprite;
        
        private List<Image> _parts = new List<Image>();

        /*public void Initialize(int maxParts)
        {
            _parent.Clear();
        }*/
        private void Awake()
        {
            _parent.Clear();
        }

        public override void SetProgress(float currentLevel, float maxLevel)
        {
            if (_parts.Count > 0)
            {
                _parent.Clear();
                _parts.Clear();
            }
            if (_parts.Count == 0)
            {
                for (int i = 0; i < maxLevel; i++)
                {
                    var part = Instantiate(_progressPart, _parent);
                    part.sprite = _notUpgradedSprite;
                    _parts.Add(part);
                }
            }

            for (int i = 0; i < currentLevel; i++)
            {
                _parts[i].sprite = _upgradedSprite;
            }
        }
    }
}