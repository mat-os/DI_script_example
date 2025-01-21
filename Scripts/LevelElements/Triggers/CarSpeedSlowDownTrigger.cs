using System;
using Game.Scripts.Constants;
using Game.Scripts.Infrastructure;
using UnityEngine;

namespace Game.Scripts.LevelElements.Triggers
{
    public class CarSpeedSlowDownTrigger : MonoBehaviour
    {
        [Range(0, 100)]
        [SerializeField] private float _speedBrakePercent;
        [SerializeField] private float _carDamage;
        [SerializeField] private float _pushForce = 1000f; // Сила отталкивания

        private bool _isActivated = false;

        private Rigidbody _rigidbody;
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            if (_rigidbody == null)
            {
                Debug.LogError("Rigidbody is required on this object for physical interactions.");
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if(_isActivated)
                return;
            
            if (collision.gameObject.layer == LayerMask.NameToLayer(LayersConstants.PLAYER_CAR))
            {
                _isActivated = true;
                
                GlobalEventSystem.Broker.Publish(new CarEnterSpeedSlowDownTrigger() 
                { 
                    SpeedBrakePercent = _speedBrakePercent, 
                    CarDamage = _carDamage,
                    Collision = collision
                });
                
                ApplyPushForce(collision);
            }
        }
        private void ApplyPushForce(Collision collision)
        {
            if (_rigidbody == null)
                return;

            // Получаем направление от машины к объекту
            Vector3 direction = (transform.position - collision.contacts[0].point).normalized;

            // Применяем силу к Rigidbody в направлении от удара
            _rigidbody.AddForce(direction * _pushForce, ForceMode.Impulse);
        }
    }
}