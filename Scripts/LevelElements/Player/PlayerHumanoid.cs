using System;
using System.Collections;
using DG.Tweening;
using Game.Scripts.Infrastructure.Bootstrapper;
using Game.Scripts.Utils.Debug;
using RootMotion.Dynamics;
using UnityEngine;
using UnityEngine.Animations;

namespace Game.Scripts.LevelElements.Player
{
    public class PlayerHumanoid  : IDisposable
    {
        private readonly PlayerConfig _playerConfig;
        private Rigidbody[] _allPlayerRigidbodies;

        public Rigidbody[] AllPlayerRigidbodies => _allPlayerRigidbodies;

        public PlayerView PlayerView { get; private set; }
        
        public PlayerHumanoid(PlayerView playerView, PlayerConfig playerConfig, Transform playerConstraintTarget)
        {
            PlayerView = playerView;
            _playerConfig = playerConfig;
            
            PlayerView.PuppetMaster.humanoidConfig = _playerConfig.CarHumanoidConfig;

            PlayerView.RigidbodyRoot.useGravity = false;
            PlayerView.RigidbodyRoot.isKinematic = true;
            
            _allPlayerRigidbodies = PlayerView.RigidbodyRoot.GetComponentsInChildren<Rigidbody>();

            CreateConstraint(playerConstraintTarget);
        }

        private void CreateConstraint(Transform playerRoot)
        {
            ConstraintSource source = new ConstraintSource();
            source.sourceTransform = playerRoot.transform; 
            source.weight = 1f; 

            PlayerView.ParentConstraint.AddSource(source);

            PlayerView.ParentConstraint.translationAtRest = Vector3.zero;
            PlayerView.ParentConstraint.rotationAtRest = Vector3.zero;
            PlayerView.ParentConstraint.constraintActive = true;
        }

        public void AddLaunchForceImpulse(Vector3 force)
        {
            RemoveConstraint();
            SetupPuppetMasterDeadState(_playerConfig.AirHumanoidConfig);
            CoroutineRunner.Instance.StartCoroutine(FlyForwardFromCar(force));
        }

        private void RemoveConstraint()
        {
            PlayerView.ParentConstraint.RemoveSource(0);
            PlayerView.ParentConstraint.constraintActive = false;
        }

        private void SetupPuppetMasterDeadState(PuppetMasterHumanoidConfig humanoidConfig)
        {
            PlayerView.PuppetMaster.humanoidConfig = humanoidConfig;
            
            PlayerView.PuppetMaster.mappingWeight = humanoidConfig.mappingWeight;
            PlayerView.PuppetMaster.pinWeight = humanoidConfig.pinWeight;
            PlayerView.PuppetMaster.muscleWeight = humanoidConfig.muscleWeight;
            PlayerView.PuppetMaster.muscleSpring = humanoidConfig.muscleSpring;
            PlayerView.PuppetMaster.muscleDamper = humanoidConfig.muscleDamper;
            PlayerView.PuppetMaster.pinPow = humanoidConfig.pinPow;
            PlayerView.PuppetMaster.pinDistanceFalloff = humanoidConfig.pinDistanceFalloff;
            
            PlayerView.PuppetMaster.state = PuppetMaster.State.Dead;
            PlayerView.PuppetMaster.mode = PuppetMaster.Mode.Active;
            
            PlayerView.RigidbodyRoot.useGravity = true;
            PlayerView.RigidbodyRoot.isKinematic = false;
            
            foreach (var rb in _allPlayerRigidbodies)
            {
                rb.isKinematic = false;
            }
        }

        IEnumerator FlyForwardFromCar(Vector3 force)
        {
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame();

            RigidbodyClearVelocity();
            Debug.Log("[Player] Add Force = " + force.ToString());
            AddForceToAllPlayerRigidbodies(force, ForceMode.Impulse);
        }

        public void AddForceToAllPlayerRigidbodies(Vector3 appliedForce, ForceMode forceMode)
        {
            foreach (var rb in _allPlayerRigidbodies)
            {
                rb.AddForce(appliedForce, forceMode);
            }
        }
        public void AddTorqueToAllPlayerRigidbodies(Vector3 appliedForce, ForceMode forceMode)
        {
            foreach (var rb in _allPlayerRigidbodies)
            {
                rb.AddTorque(appliedForce, forceMode);
            }
        }
        
        public void ClampVerticalSpeed(float maxVerticalSpeed)
        {
            foreach (var rb in _allPlayerRigidbodies)
            {
                if (rb.velocity.y > maxVerticalSpeed)
                {
                    Debug.Log(rb + "Clear Speed Vertical! ");
                    rb.velocity = new Vector3(rb.velocity.x, maxVerticalSpeed, rb.velocity.z);
                }
            }
        }
        public void AddTorqueForFlip(Vector3 forwardDirection, float torqueAmount, ForceMode forceMode)
        {
            // Ось вращения - перпендикулярна направлению полета.
            // Например, если игрок летит вперед (по оси Z), вращение может быть вокруг X или Z.
            Vector3 torqueAxis = forwardDirection.normalized; // Используйте направление движения.
            Vector3 torqueVector = torqueAxis * torqueAmount;

            foreach (var rb in _allPlayerRigidbodies)
            {
                rb.AddTorque(torqueVector, forceMode);
            }
        }
        public void AddForceToChest(Vector3 force, ForceMode forceMode)
        {
            PlayerView.RigidbodyRoot.AddForce(force, forceMode);
        }

        public void RigidbodyClearVelocity()
        {
            foreach (var rb in _allPlayerRigidbodies)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        public void RigidbodyClearVelocityY(float multiplierVelocityLimitY = 1)
        {
            foreach (var rb in _allPlayerRigidbodies)
            {
                // Обнуляем только скорость по оси Y, оставляя X и Z без изменений
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y * multiplierVelocityLimitY, rb.velocity.z);
            }
        }

        public void RigidbodyClearVelocityX()
        {
            foreach (var rb in _allPlayerRigidbodies)
            {
                // Обнуляем только скорость по оси Y, оставляя X и Z без изменений
                rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);
            }
        }
        public void RigidbodyClearVelocityZ(float multiplierVelocityLimit = 1)
        {
            foreach (var rb in _allPlayerRigidbodies)
            {
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, rb.velocity.z * multiplierVelocityLimit);
            }
        }

        public void AddExplosionForce(Vector3 explodePosition, float explosionForce)
        {
            RigidbodyClearVelocityY();
            var direction = explodePosition - PlayerView.RigidbodyRoot.position;
            var force = direction.normalized * explosionForce;
            AddForceToChest(force, ForceMode.Impulse);
            //_playerConfig.BarrelExplosions
        }

        public void Dispose()
        {
            _allPlayerRigidbodies = null;
        }
    }
}