using System;
using Game.Scripts.Configs;
using Game.Scripts.Infrastructure;
using Game.Scripts.Utils.Debug;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.LevelElements
{
    public class LevelPhysicGameObject : MonoBehaviour
    {
        public UnityEvent OnPhysicsActivated;
        
        [Header("Score")] 
        public EScoreType ScoreType;
        
        [Header("Is Need Activate Nearby Object?")]
        public bool IsActivateNearbyObject = true;

        [Header("Mission")] 
        public bool IsCountForMission;
        
        [Header("Components")]
        [SerializeField]private Rigidbody _rigidbody;
        [SerializeField]private float _activationRadiusAdd;

        [field: Header("Player Human Velocity Change On Damage")]
        [field:InfoBox(" If = 0 than no change")]
        [field: Range(0, 1)]
        [field: SerializeField] public float PlayersVelocityChangeOnDamage { get; private set; } = 0;

        //TODO:
        private LayerMask _affectedLayers; 
        
        [SerializeField][ReadOnly]private bool _isActivated = false;
        
        private float _activationRadius;

        private void Awake()
        {
            if(_rigidbody == null)
                _rigidbody = GetComponent<Rigidbody>();
            if (_rigidbody != null)
            {
                _rigidbody.isKinematic = false;
                _rigidbody.Sleep();
            }

            _isActivated = false;
            _activationRadius = CalculateActivationRadius();
            _affectedLayers = LayerMask.NameToLayer("Default");
        }

        public void ActivatePhysics(Vector3 impactForce, bool isFirstImpact = true)
        {
            if (_isActivated || _rigidbody == null)
                return;
            
            CustomDebugLog.Log("Activating Physics " + gameObject.name + " "+ impactForce.ToString());
            _isActivated = true;
            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = true;

            if (isFirstImpact)
            {
                //_rigidbody.AddForce(impactForce, ForceMode.VelocityChange);
                if(IsActivateNearbyObject)
                    ActivateNearbyObjects();
            }
            
            if (IsCountForMission && isFirstImpact)
            {
                GlobalEventSystem.Broker.Publish(new MissionObjectDestroyEvent { HitPosition = transform.position });
            }
            
            OnPhysicsActivated?.Invoke();
            
            GlobalEventSystem.Broker.Publish(new DestroyObjectEvent() { ScoreType = ScoreType, DestroyPosition = transform.position});
        }
        // Метод для активации физики объектов в зоне взрыва
        private void ActivateNearbyObjects()
        {
            // Используем радиус, который вычисляется на основе размеров объекта
            Collider[] colliders = Physics.OverlapSphere(transform.position, _activationRadius, _affectedLayers);

            foreach (var collider in colliders)
            {
                LevelPhysicGameObject nearbyLevelPhysic = collider.GetComponent<LevelPhysicGameObject>();
                if (nearbyLevelPhysic != null)
                {
                    nearbyLevelPhysic.ActivatePhysics(Vector3.zero, false);
                }
            }
        }

        // Вычисляем радиус активации на основе размеров меша или коллайдера
        private float CalculateActivationRadius()
        {
            float radius = 0f;

            // Если у объекта есть Collider, используем его размер
            Collider collider = GetComponent<Collider>();
            if (collider != null)
            {
                radius = collider.bounds.extents.magnitude + _activationRadiusAdd; // Максимальное расстояние от центра до края коллайдера
                return radius;
            }

            // Если у объекта есть MeshRenderer, используем его размеры
            /*MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                radius = meshRenderer.bounds.extents.magnitude + _activationRadiusAdd; // Максимальное расстояние от центра до края меша
                return radius;
            }*/

            // Если нет ни Collider, ни MeshRenderer, возвращаем стандартное значение радиуса
            return 1.0f;
        }

        private void OnDrawGizmosSelected()
        {
            float radius = 0f;
            Collider collider = GetComponent<Collider>();
            if (collider != null)
            {
                radius = collider.bounds.extents.magnitude + _activationRadiusAdd; // Максимальное расстояние от центра до края коллайдера
                Gizmos.DrawWireSphere(transform.position, radius);
            }
        }

        private void OnDestroy()
        {
            OnPhysicsActivated = null;
        }
    }
}