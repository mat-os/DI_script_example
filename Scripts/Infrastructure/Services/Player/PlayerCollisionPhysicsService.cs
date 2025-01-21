using Configs;
using Game.Scripts.Infrastructure.Services.Level;
using Game.Scripts.LevelElements;
using Game.Scripts.LevelElements.Collisions;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Infrastructure.Services.Player
{
    public class PlayerCollisionPhysicsService 
    {
        private DifficultyService _difficultyService;
        private float _physicsVelocityStopMultiplier = 1;

        [Inject]
        private PlayerCollisionPhysicsService(DifficultyService difficultyService)
        {
            _difficultyService = difficultyService;
        }

        public void SetupDifficulties()
        {
            _physicsVelocityStopMultiplier = _difficultyService.GetPhysicsVelocityStopMultiplier();
        }
        public void TryEnableObjectPhysics(Collision collision, MuscleCollisionTrigger playerMuscles)
        {
            var levelPhysicGameObject = collision.gameObject.GetComponent<LevelPhysicGameObject>();
            if (levelPhysicGameObject != null)
            {
                if (collision.rigidbody == null)
                {
                    Debug.LogWarning($"Collision object {collision} does not have a Rigidbody.");
                    return;
                }
                
                Vector3 impactForce = collision.relativeVelocity * collision.rigidbody.mass;
                levelPhysicGameObject.ActivatePhysics(impactForce);
                
                Vector3 currentVelocity = playerMuscles.Rigidbody.velocity;
                var stopMultiplier = 1 - levelPhysicGameObject.PlayersVelocityChangeOnDamage;
                stopMultiplier *= _physicsVelocityStopMultiplier;
                playerMuscles.Rigidbody.velocity = currentVelocity * stopMultiplier;
            }
        }
    }
}