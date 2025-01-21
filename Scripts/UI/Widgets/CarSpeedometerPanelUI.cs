using System;
using Configs;
using Game.Scripts.Infrastructure.Services.Car;
using Game.Scripts.LevelElements.Car;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.Widgets
{
    public class CarSpeedometerPanelUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _speed;
        //[SerializeField] private Gradient speedColorGradient; 
        [SerializeField] private Image _speedFillImage;     

        [SerializeField] private float minFill = 0.1f; // Минимальное значение fill
        [SerializeField] private float maxFill = 1.0f; // Максимальное значение fill
        
        private CarService _carService;
        private float _maxSpeedKmh;

        [Inject]
        public void Construct(CarService carService)
        {
            _carService = carService;
        }
        public void Initialize()
        {
            _maxSpeedKmh = _carService.GetCarMaxSpeedKmh();
        }
        private void Update()
        {
            var speed = _carService.GetCarSpeedKmh();
            _speed.text = speed.ToString("F0");
            
            float normalizedSpeed = Mathf.Clamp01(speed / _maxSpeedKmh);
            //_speed.color = speedColorGradient.Evaluate(normalizedSpeed);
            
            //_speedFillImage.fillAmount = normalizedSpeed;
            _speedFillImage.fillAmount = Map(normalizedSpeed, 0f, 1f, minFill, maxFill);
        }
        private float Map(float value, float inMin, float inMax, float outMin, float outMax)
        {
            return outMin + (outMax - outMin) * ((value - inMin) / (inMax - inMin));
        }
        public void Clear()
        {
            _maxSpeedKmh = 0;
            _speed.text = "0";
        }
    }
}