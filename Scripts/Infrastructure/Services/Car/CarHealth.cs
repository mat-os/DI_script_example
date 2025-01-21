using System;
using Game.Scripts.LevelElements.Car;
using PG;
using UniRx;
using UnityEngine;

namespace Game.Scripts.Infrastructure.Services.Car
{
    public class CarHealth
    {
        public event Action<float> OnHealthChanged; // Вызывается при изменении здоровья
        public event Action OnCarDestroyed; // Вызывается, если машина разрушена
        
        private float _currentHealth;
        private readonly float _maxHealth;
        
        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        private readonly CarVfxView _carVfxView;
        private CarView _carView;

        public CarHealth(float maxHealth, CarView carView)
        {
            _carView = carView;
            _maxHealth = maxHealth;
            _currentHealth = maxHealth;
            _carVfxView = carView.CarVfxView;
        }
        public void ApplyDamage(float damageAmount, Collision collision)
        {
            /*if (damageAmount <= 0) 
                return;*/
            
            _currentHealth -= damageAmount;
            _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
            
            UpdateHealthEffects();
            ApplyDamageOnCar(damageAmount, collision);
            OnHealthChanged?.Invoke(_currentHealth);

            Debug.Log($"[CAR HEALTH] {_currentHealth}");
            if (_currentHealth <= 0)
            {
                HandleCarDestruction();
            }
        }
        
        public void DestroyCar()
        {
            //TODO:
            //_currentHealth = 0;
            //ApplyDamageOnCar(1000);
        }
        public void ApplyDamageOnCar(float damageAmount, Collision collision)
        {
            Debug.Log("[CAR] ApplyDamageOnCar");
            
            var damageData = new DamageData
            {
                DamagePoint = collision.contacts[0].point,
                DamageMultiplier = damageAmount,
                DamageForce = collision.relativeVelocity,
                MassFactor = collision.rigidbody.mass,
                SurfaceNormal = collision.contacts[0].normal
            };

            _carView.VehicleDamageController.AddDamageToQueue(damageData);
        }
        public void ApplyDamageTest()
        {
            Debug.Log("[CAR] ApplyDamageTest Called");
            // Создаем DamageData вручную
            DamageData damageData = new DamageData
            {
                DamagePoint = new Vector3(0, 1, 0),
                DamageForce = new Vector3(0, 0, -10),
                SurfaceNormal = Vector3.up,
                MassFactor = 1.5f,
                DamageMultiplier = 1f 
            };

            _carView.VehicleDamageController.AddDamageToQueue(damageData);
        }
        public void Heal(float healAmount)
        {
            if (healAmount <= 0) return;
            
            _currentHealth += healAmount;
            _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
            
            UpdateHealthEffects();
            OnHealthChanged?.Invoke(_currentHealth);
        }
        public float GetCurrentHealth()
        {
            return _currentHealth;
        }
        private void HandleCarDestruction()
        {
            OnCarDestroyed?.Invoke();
            Debug.Log("Car is destroyed!");
        }
        
        private void UpdateHealthEffects()
        {
            if (_carVfxView == null) return;

            float carHealthPercent = _currentHealth / _maxHealth;

            if (_carVfxView._engineHealth75Particles)
            {
                _carVfxView._engineHealth75Particles.gameObject.SetActive(carHealthPercent > 0.5f && carHealthPercent <= 0.75f);
            }
            if (_carVfxView._engineHealth50Particles)
            {
                _carVfxView._engineHealth50Particles.gameObject.SetActive(carHealthPercent <= 0.5f && carHealthPercent > 0.25f);
            }
            if (_carVfxView._engineHealth25Particles)
            {
                _carVfxView._engineHealth25Particles.gameObject.SetActive(carHealthPercent <= 0.25f);
            }
        }
        public void ResetHealth()
        {
            _currentHealth = _maxHealth;
            UpdateHealthEffects();
            OnHealthChanged?.Invoke(_currentHealth);
        }
        public void Dispose()
        {
            OnHealthChanged = null;
            OnCarDestroyed = null;
            _disposable?.Dispose();
        }


    }
}