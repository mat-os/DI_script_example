using System;
using UnityEngine;

namespace Game.Scripts.LevelElements.Collisions
{
    public abstract class CollisionComponent : MonoBehaviour
    {
        public event Action<CollisionComponent, Collision> CollisionEnter;
        public event Action<CollisionComponent, Collision> CollisionExit;
        public event Action<CollisionComponent, Collision> CollisionStay;

        private void OnCollisionEnter(Collision other)
        {
            CollisionEnter?.Invoke(this, other);
        }

        private void OnCollisionStay(Collision other)
        {
            CollisionStay?.Invoke(this, other);
        }

        private void OnCollisionExit(Collision other)
        {
            CollisionExit?.Invoke(this, other);
        }

        private void OnDestroy()
        {
            CollisionEnter = null;
            CollisionStay = null;
            CollisionExit = null;
        }
    }
}