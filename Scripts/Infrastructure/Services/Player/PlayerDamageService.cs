using System;
using System.Collections.Generic;
using System.Linq;
using Configs;
using DG.Tweening;
using Game.Scripts.Constants;
using Game.Scripts.Infrastructure.Services.Level;
using Game.Scripts.Infrastructure.States;
using Game.Scripts.LevelElements;
using Game.Scripts.LevelElements.Collisions;
using Game.Scripts.LevelElements.Player;
using Game.Scripts.Utils.Debug;
using RootMotion.Dynamics;
using UnityEngine;

namespace Game.Scripts.Infrastructure.Services.Player
{

    public class PlayerDamageService : IDisposable
    {
        public Action<float> OnChangeTotalDamage;
        public Action<float> OnGetDamage;
        public Action<int> OnChangeBrokenBones;
        public Action<Muscle.Group> OnBrokeBone;

        private readonly VibrationService _vibrationService;
        private readonly VfxEffectsService _vfxEffectsService;
        private readonly PlayerService _playerService;
        private readonly PlayerConfig _playerConfig;
        private readonly PlayerFlightLaunchService _playerFlightLaunchService;
        private readonly PlayerFlightLandingService _playerFlightLandingService;
        private readonly DamageTextEffectService _damageTextEffectService;
        private readonly PlayerBoneDisplayService _boneDisplayService;
        private readonly PlayerAirTimeCounterService _playerAirTimeCounterService;
        private readonly PlayerCollisionPhysicsService _playerCollisionPhysicsService;
        
        private float _totalDamage;
        private int _brokenBonesCount;
        private bool _isInvincible = true;
        private int _maxDamage;

        // Храним суммарный урон по каждой части тела
        private readonly Dictionary<EExtendedMuscleGroup, float> _bodyPartDamage = new();
        private readonly Dictionary<EExtendedMuscleGroup, bool> _brokenBones = new();

        //Триггеры на мышце персонажа
        private List<MuscleCollisionTrigger> _muscleCollisionTriggers = new List<MuscleCollisionTrigger>();
        private DifficultyService _difficultyService;
        private float _difficultyDamageMultiplier;
        private GroundParticleSpawnService _groundParticleSpawnService;

        public int GetMaxPossibleDamage() => 
            _maxDamage;
        public PlayerDamageService(VibrationService vibrationService,
            VfxEffectsService vfxEffectsService,
            PlayerService playerService,
            GameConfig gameConfig,
            PlayerFlightLaunchService playerFlightLaunchService,
            PlayerFlightLandingService playerFlightLandingService,
            DamageTextEffectService damageTextEffectService,
            PlayerBoneDisplayService boneDisplayService,
            PlayerAirTimeCounterService playerAirTimeCounterService,
            PlayerCollisionPhysicsService playerCollisionPhysicsService,
            DifficultyService difficultyService,
            GroundParticleSpawnService groundParticleSpawnService)
        {
            _groundParticleSpawnService = groundParticleSpawnService;
            _difficultyService = difficultyService;
            _playerCollisionPhysicsService = playerCollisionPhysicsService;
            _playerAirTimeCounterService = playerAirTimeCounterService;
            _boneDisplayService = boneDisplayService;
            _damageTextEffectService = damageTextEffectService;
            _playerFlightLandingService = playerFlightLandingService;
            _playerFlightLaunchService = playerFlightLaunchService;
            _vfxEffectsService = vfxEffectsService;
            _vibrationService = vibrationService;
            _playerService = playerService;
            _playerConfig = gameConfig.PlayerConfig;

            _playerService.OnPlayerHumanoidCreated += PlayerCreatedHandler;
            _playerFlightLaunchService.OnPlayerFlyStart += PlayerFlyStartHandle;
            _playerFlightLandingService.OnPlayerFlyComplete += PlayerFlyCompleteHandle;
        }

        private void PlayerFlyCompleteHandle()
        {
            _isInvincible = true;
        }

        private void PlayerFlyStartHandle()
        {
            _isInvincible = false;
            //SetIsInvincible(0.5f);
        }

        private void PlayerCreatedHandler(PlayerHumanoid player)
        {
            _maxDamage = 0;
            var playerMuscles = player.PlayerView.MuscleGroups;
            foreach (var muscle in playerMuscles)
            {
                var group = muscle.ExtendedMuscleGroup;
                
                var trigger = muscle.MuscleJoint.gameObject.AddComponent<MuscleCollisionTrigger>();
                trigger.CollisionEnter += MuscleCollisionHandler;
                trigger.SetBodyPart(group, muscle.MuscleGroup);

                _muscleCollisionTriggers.Add(trigger);
                _bodyPartDamage.Add(group, 0);
                _brokenBones.Add(group, false);
                _maxDamage += _playerConfig.DamageConfig.DamageThresholds[muscle.MuscleGroup];
            }
            
            _difficultyDamageMultiplier = _difficultyService.GetPlayerDamageMultiplier();
        }

        private void MuscleCollisionHandler(CollisionComponent collisionComponent, Collision collision)
        {
            if (_isInvincible)
                return;

            if(collisionComponent.gameObject.layer == collision.gameObject.layer)
                return;
            
            var playerMuscles = collisionComponent as MuscleCollisionTrigger;
            if (playerMuscles == null)
                return;

            if (collision.gameObject.layer == LayerMask.NameToLayer(LayersConstants.GROUND))
            {
                _playerAirTimeCounterService.PlayerTouchGround();

                if (playerMuscles.PropsGroup is Muscle.Group.Spine or Muscle.Group.Head or Muscle.Group.Hips)
                {
                    var position = collisionComponent.gameObject.transform.position;
                    float impactForce = collision.relativeVelocity.magnitude;
                    _groundParticleSpawnService.TrySpawnParticle(position, impactForce, collision);
                }
            }

            var extendedMuscleGroup = playerMuscles.ExtendedMusculeGroup;

            //Если кость уже сломана
            if (_brokenBones[extendedMuscleGroup])
            {
                //Debug.Log("Bone already broken, damage is ignored!");
                return;
            }

            if (_bodyPartDamage.ContainsKey(extendedMuscleGroup) == false)
            {
                return;
            }
                
            float collisionForce = collision.relativeVelocity.magnitude;
            float damage = collisionForce * _playerConfig.DamageConfig.DamageMultiplier; // * _rigidbody.mass
            damage *= _difficultyDamageMultiplier;
            
            var totalBodyPartDamage = _bodyPartDamage[extendedMuscleGroup];
            int breakThreshold = _playerConfig.DamageConfig.DamageThresholds[playerMuscles.PropsGroup];
            
            if(totalBodyPartDamage + damage >= breakThreshold)
                damage = Mathf.Clamp(damage, 0, breakThreshold - totalBodyPartDamage);
            
            // Добавляем урон для конкретной части тела
            _bodyPartDamage[extendedMuscleGroup] += damage;
            _totalDamage += damage;
            OnChangeTotalDamage?.Invoke(_totalDamage);
            OnGetDamage?.Invoke(damage);

            CheckForBoneBreak(playerMuscles.PropsGroup, extendedMuscleGroup, totalBodyPartDamage, breakThreshold);
            
            var percentage = totalBodyPartDamage / breakThreshold;
            _boneDisplayService.UpdateBoneDisplay(extendedMuscleGroup, percentage);
                
            GlobalEventSystem.Broker.Publish(new PlayerBodyPartTakeDamageEvent { 
                MuscleGroup = playerMuscles.PropsGroup,
                Damage = damage,
                EExtendedMuscleGroup = extendedMuscleGroup,
                TotalDamageOnBodyPart = _bodyPartDamage[extendedMuscleGroup]
            });
                
            //CustomDebugLog.Log($"[DMG] Add Damage: {damage} on {extendedMuscleGroup}, {collision.gameObject.name}, Total Damage: {_totalDamage}");
            
            _playerCollisionPhysicsService.TryEnableObjectPhysics(collision, playerMuscles);
        }
        
        private void CheckForBoneBreak(Muscle.Group bodyPart, EExtendedMuscleGroup extendedMuscleGroup, float totalDamage, int breakThreshold)
        {
            if (totalDamage >= breakThreshold && _brokenBones[extendedMuscleGroup] == false)
            {
                _brokenBonesCount++;
                OnChangeBrokenBones?.Invoke(_brokenBonesCount);
                OnBrokeBone?.Invoke(bodyPart);
                CustomDebugLog.Log($"[BONE] {bodyPart} broken! Total broken bones: {_brokenBonesCount}");
                _brokenBones[extendedMuscleGroup] = true;
                
                // Дополнительно можно вызвать визуальные эффекты, вибрацию и т.д.
                //_vfxEffectsService.ShowBrokenBoneEffect(bodyPart);
                _vibrationService.Vibrate(VibrationPlaceType.BreakBone);
            }
        }
        public void Clear()
        {
            _totalDamage = 0;
            _brokenBonesCount = 0;
            _maxDamage = 0;
            _bodyPartDamage.Clear();
            _brokenBones.Clear();
            _isInvincible = true;
        }

        public void Dispose()
        {
            _playerService.OnPlayerHumanoidCreated -= PlayerCreatedHandler;
            _playerFlightLaunchService.OnPlayerFlyStart -= PlayerFlyStartHandle;
            _playerFlightLandingService.OnPlayerFlyComplete -= PlayerFlyCompleteHandle;
        }

        public void SetIsInvincible(float duration)
        {
            _isInvincible = true;
            DOVirtual.DelayedCall(duration, () => _isInvincible = false);
        }
    }
}