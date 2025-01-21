using UnityEngine;

namespace Game.Scripts.LevelElements.Car
{
    public class CarDamageTrigger : MonoBehaviour
    {
        [field: SerializeField] public LayerMask PhysicObjectLayer  { get;private set; }
        [field: SerializeField] public float ForceMultiplier  { get;private set; }
        private void OnCollisionEnter(Collision collision)
        {
            if ((PhysicObjectLayer.value & (1 << collision.gameObject.layer)) != 0)
            {
                if (collision.gameObject.TryGetComponent(out LevelPhysicGameObject levelPhysicGameObject))
                {
                    Vector3 impactForce = collision.relativeVelocity * collision.rigidbody.mass * ForceMultiplier;
                    Debug.Log($"CarDamageTrigger OnCollisionEnter impactForce = {impactForce} with {collision.gameObject}");
                    levelPhysicGameObject.ActivatePhysics(impactForce, false);
                }
            }
        }
    }
}