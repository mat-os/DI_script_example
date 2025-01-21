using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.LevelElements.Triggers
{
    public class TriggerUnityEvent : TriggerComponent
    {
        [Header("Unity Events")]
        public UnityEvent OnTriggerEnterEvent; // Вызывается при OnTriggerEnter
        public UnityEvent OnTriggerExitEvent;  // Вызывается при OnTriggerExit
        public UnityEvent OnTriggerStayEvent;  // Вызывается при OnTriggerStay

        public LayerMask ActivationMask;
        
        private void Awake()
        {
            // Убедимся, что у объекта есть коллайдер, и он настроен как триггер
            Collider collider = GetComponent<Collider>();
            if (collider != null && !collider.isTrigger)
            {
                collider.isTrigger = true;
                Debug.LogWarning($"Collider on {gameObject.name} was not set to Trigger. Automatically fixed.", gameObject);
            }
        }
        private void Start()
        {
            // Подписываем Unity Events на базовые события
            TriggerEnter += HandleTriggerEnter;
            TriggerExit += HandleTriggerExit;
            TriggerStay += HandleTriggerStay;
        }

        private void OnDestroy()
        {
            // Отписываемся от событий
            TriggerEnter -= HandleTriggerEnter;
            TriggerExit -= HandleTriggerExit;
            TriggerStay -= HandleTriggerStay;
        }

        // Вызываем UnityEvent, когда срабатывают события
        private void HandleTriggerEnter(TriggerComponent trigger, Collider other)
        {
            if (((1 << other.gameObject.layer) & ActivationMask.value) != 0)
                OnTriggerEnterEvent?.Invoke();
        }

        private void HandleTriggerExit(TriggerComponent trigger, Collider other)
        {
            if (((1 << other.gameObject.layer) & ActivationMask.value) != 0)
                OnTriggerExitEvent?.Invoke();
        }

        private void HandleTriggerStay(TriggerComponent trigger, Collider other)
        {
            if (((1 << other.gameObject.layer) & ActivationMask.value) != 0)
                OnTriggerStayEvent?.Invoke();
        }
    }
}