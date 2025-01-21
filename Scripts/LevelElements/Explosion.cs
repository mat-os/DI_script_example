using Game.Scripts.Infrastructure;
using Game.Scripts.LevelElements.Player;
using UnityEngine;

namespace Game.Scripts.LevelElements
{
    public class Explosion : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Rigidbody _rigidbody; 
        [SerializeField] private Transform _explosionPosition; 

        [Header("Explosion Settings")] 
        [SerializeField] private float _explosionRadius = 5f; 
        [SerializeField] private float _explosionForce = 500f; 
        [SerializeField] private float _upwardsModifier = 1f; 
        [SerializeField] private LayerMask _affectedLayers;

        [Header("Visual & Audio Effects")]
        //[SerializeField] private AudioClip _explosionSound; // Звук взрыва
        [SerializeField]
        private float _destroyDelay = 2f; // Задержка перед уничтожением бочки после взрыва

        [SerializeField] private float _barrelForce = 2f; // Задержка перед уничтожением бочки после взрыва

        private bool _hasExploded; 

        public void Explode()
        {
            if (_hasExploded)
                return;
            
            _hasExploded = true;
            
            AddForceToNearbyObjects();
            JumpObject();
            
            GlobalEventSystem.Broker.Publish(new PlayerExplodeEvent() { ExplodePosition = transform.position, ExplosionForce = _explosionForce});

            // Уничтожаем бочку с задержкой
            //Destroy(gameObject, _destroyDelay);
            
            /*// 🔊 Воспроизводим звук взрыва
            if (_explosionSound != null)
            {
                AudioSource.PlayClipAtPoint(_explosionSound, transform.position);
            }*/
        }

        private void AddForceToNearbyObjects()
        {
            var colliders = Physics.OverlapSphere(transform.position, _explosionRadius, _affectedLayers);
            foreach (var nearbyObject in colliders)
            {
                var rb = nearbyObject.attachedRigidbody;

                if (rb != null)
                {
                    rb.AddExplosionForce(_explosionForce, _explosionPosition.position, _explosionRadius, _upwardsModifier, ForceMode.Impulse);
                }
            }
        }

        private void JumpObject()
        {
            _rigidbody.AddForce(_barrelForce * transform.up, ForceMode.Impulse);
        }

        private void OnDrawGizmosSelected()
        {
            // 🎯 Визуализируем радиус взрыва в редакторе
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _explosionRadius);
        }
    }
}