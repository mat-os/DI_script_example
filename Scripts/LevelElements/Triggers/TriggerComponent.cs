using System;
using UnityEngine;

namespace Game.Scripts.LevelElements.Triggers
{
    public abstract class TriggerComponent : MonoBehaviour
    {
        public event Action<TriggerComponent, Collider> TriggerEnter;
        public event Action<TriggerComponent, Collider> TriggerExit;
        public event Action<TriggerComponent, Collider> TriggerStay;

        private void OnTriggerEnter(Collider other)
        {
            TriggerEnter?.Invoke(this, other);
        }

        private void OnTriggerStay(Collider other)
        {
            TriggerStay?.Invoke(this, other);
        }

        private void OnTriggerExit(Collider other)
        {
            TriggerExit?.Invoke(this, other);
        }

        private void OnDestroy()
        {
            TriggerEnter = null;
            TriggerStay = null;
            TriggerExit = null;
        }
    }
}