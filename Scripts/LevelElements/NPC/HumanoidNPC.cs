using System;
using System.Collections.Generic;
using Game.Scripts.Configs;
using Game.Scripts.Infrastructure;
using Game.Scripts.LevelElements.Collisions;
using Game.Scripts.Utils.Debug;
using RootMotion.Dynamics;
using SWS;
using UnityEngine;

namespace Game.Scripts.LevelElements.NPC
{
    public class HumanoidNPC : MonoBehaviour
    {
        [Header("Physics")]
        [SerializeField] private PuppetMaster _puppetMaster;  
        [SerializeField] private LayerMask _carLayer;  // Слой для машин
        [SerializeField] private LayerMask _playerLayer;  // Слой для игрока
        [SerializeField] private PuppetMasterHumanoidConfig _deadPuppetConfig;  // Конфигурация для состояния Dead
        
        [SerializeField]private List<MuscleCollisionTrigger> _muscleCollisionTriggers = new List<MuscleCollisionTrigger>();

        [Header("Mission")]
        [SerializeField] private bool _isCountForMission;
        
        [Header("Score")]
        [SerializeField] private EScoreType _scoreType;
        
        [Header("Move")]
        [SerializeField]private splineMove _splineMove;
        
        [Header("Fx")]
        [SerializeField]private Transform _vfxTransform;

        private bool _isDead = false;
        private Rigidbody[] _rigidbodies;

        private void Start()
        {
            _rigidbodies = GetComponentsInChildren<Rigidbody>();

            foreach (var muscle in _muscleCollisionTriggers)
            {
                muscle.CollisionEnter += MuscleCollisionEnterHandle;
            }
        }

        private void MuscleCollisionEnterHandle(CollisionComponent arg1, Collision collision)
        {
            // Проверяем, принадлежит ли объект слою машин или слою игрока
            if (IsInDamageLayer(collision.gameObject.layer))
            {
                //Debug.Log("Hit by object in damage layer");
                if (!_isDead)
                {
                    KillNPC();
                }
            }
        }

        // Метод для проверки, принадлежит ли объект слою машин ИЛИ слою игрока
        private bool IsInDamageLayer(int objectLayer)
        {
            // Проверка: объект должен принадлежать либо _carLayer, либо _playerLayer
            bool isCarLayer = (_carLayer.value & (1 << objectLayer)) != 0;
            bool isPlayerLayer = (_playerLayer.value & (1 << objectLayer)) != 0;

            //Debug.Log($"Is in car layer: {isCarLayer}, is in player layer: {isPlayerLayer}");
            //Debug.Log($"Checking object layer: {LayerMask.LayerToName(objectLayer)} against car layer and player layer.");

            return isCarLayer || isPlayerLayer;
        }

        private void KillNPC()
        {
            _isDead = true;

            _puppetMaster.humanoidConfig = _deadPuppetConfig;
            
            _puppetMaster.mappingWeight = _deadPuppetConfig.mappingWeight;
            _puppetMaster.pinWeight = _deadPuppetConfig.pinWeight;
            _puppetMaster.muscleWeight = _deadPuppetConfig.muscleWeight;
            _puppetMaster.muscleSpring = _deadPuppetConfig.muscleSpring;
            _puppetMaster.muscleDamper = _deadPuppetConfig.muscleDamper;
            _puppetMaster.pinPow = _deadPuppetConfig.pinPow;
            _puppetMaster.pinDistanceFalloff = _deadPuppetConfig.pinDistanceFalloff;
            
            _puppetMaster.state = PuppetMaster.State.Dead;
            _puppetMaster.mode = PuppetMaster.Mode.Active;
            
            foreach (var rb in _rigidbodies)
            {
                rb.isKinematic = false;
            }

            GlobalEventSystem.Broker.Publish(new HitPeopleEvent { ScoreType = _scoreType, DestroyPosition = transform.position, VfxPosition =  _vfxTransform.position});
            
            if (_isCountForMission)
            {
                GlobalEventSystem.Broker.Publish(new MissionObjectDestroyEvent { HitPosition = transform.position });
            }

            if (transform.TryGetComponent(out splineMove splineMove))
            {
                splineMove.Stop();
            }

            if (_splineMove != null)
            {
                _splineMove.Stop();
                _splineMove.enabled = false;
            }
            CustomDebugLog.Log("NPC is now Dead.");
        }

        private void OnDisable()
        {
            if (_splineMove != null)
            {
                _splineMove.Stop();
                _splineMove.enabled = false;
            }
        }
    }
}
