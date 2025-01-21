using System;
using DG.Tweening;
using Game.Scripts.Configs;
using Game.Scripts.Configs.Vfx;
using Game.Scripts.Constants;
using Game.Scripts.Infrastructure;
using Game.Scripts.Infrastructure.Services;
using Game.Scripts.LevelElements.NPC.Cars;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

public class CarNPC : MonoBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField] private Transform _explosionSpawnPoint; // Точка спавна эффекта взрыва
    [SerializeField] private float _upwardForce = 2f; // Дополнительная сила вверх
    [SerializeField] private Vector3 _rotation; // Сила вращения
    [SerializeField] private float _repulsionForce = 5f; // Сила отталкивания

    [Header("Score")] 
    public EScoreType ScoreType;
    
    private SimpleCarMover _carMover;
    private Rigidbody _rigidbody;
    private bool _isDestroyed;
    
    private float _colliderHeight;
    private Collider _collider;
    public Transform ExplosionSpawnPoint => _explosionSpawnPoint;

    private void Awake()
    {
        _carMover = GetComponent<SimpleCarMover>();
        _rigidbody = GetComponent<Rigidbody>();
        if (_rigidbody == null)
        {
            Debug.LogError($"{gameObject.name} missing Rigidbody!");
        }
        
        _collider = GetComponent<Collider>();
        _colliderHeight = _collider.bounds.size.y;
    }

    
    private void OnCollisionEnter(Collision collision)
    {
        if (_isDestroyed) 
            return;
        if(collision.gameObject.layer != LayerMask.NameToLayer(LayersConstants.PLAYER_CAR))
            return;
        
        HandleCarDestruction(collision.contacts[0].point, collision.contacts[0].normal);
        _collider.enabled = false;
    }

    [Button]
    private void HandleCarDestruction(Vector3 collisionPoint, Vector3 collisionNormal)
    {
        _isDestroyed = true;

        // Останавливаем движение
        if (_carMover != null)
        {
            _carMover.enabled = false;
        }
        
        // Анимация отлёта
         AnimateCarDestruction(collisionPoint, collisionNormal);
        
        GlobalEventSystem.Broker.Publish(new CarNPCDestroyedEvent() {CarNPC  = this});
        GlobalEventSystem.Broker.Publish(new DestroyObjectEvent() { ScoreType = ScoreType, DestroyPosition = transform.position});
    }
    
    private void AnimateCarDestruction(Vector3 collisionPoint, Vector3 collisionNormal)
    {
        if (_rigidbody == null) return;

        _rigidbody.isKinematic = true;
        
        Vector3 repulsionDirection = (collisionPoint - _rigidbody.position).normalized;
        
        float randomYRotation = UnityEngine.Random.Range(-30f, 30f);
        Vector3 randomRotation = new Vector3(0f, randomYRotation, 0f);
        Vector3 landPosition = _rigidbody.position - Vector3.up * (_upwardForce - _colliderHeight);

        // Apply the flip rotation using DOTween
        _rigidbody.DORotate(_rotation + randomRotation, 1f, RotateMode.WorldAxisAdd).SetEase(Ease.OutBack);

        // Apply an upward movement using DOTween
        Vector3 liftPosition = _rigidbody.position + Vector3.up * _upwardForce + -repulsionDirection * _repulsionForce; 
        _rigidbody.DOMove(liftPosition, 0.5f).SetEase(Ease.OutSine).OnComplete(() =>
        {
            // Apply a downward movement using DOTween to simulate landing
            _rigidbody.DOMove(landPosition, 0.5f).SetEase(Ease.InSine);
        });
    }
}
