using Game.Scripts.Infrastructure;
using UnityEngine;
using UnityEngine.Events;

public class PinfallDetector : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float fallAngleThreshold = 30f; // Угол, при котором кегля считается упавшей
    [SerializeField] private float checkInterval = 0.2f; // Как часто проверять угол (для оптимизации)

    [Header("Events")]
    public UnityEvent OnPinFall; // Событие, когда кегля упала

    private bool _hasFallen = false; // Флаг, чтобы убедиться, что событие вызывается только один раз
    private Quaternion _initialRotation; // Начальная ориентация кегли

    private void Start()
    {
        _initialRotation = transform.rotation;

        if (gameObject.TryGetComponent(out Rigidbody rb))
        {
            rb.Sleep();
        }

        InvokeRepeating(nameof(CheckIfFallen), checkInterval, checkInterval);
    }

    private void CheckIfFallen()
    {
        if (_hasFallen) 
            return;

        float angle = Quaternion.Angle(_initialRotation, transform.rotation);
        
        if (angle > fallAngleThreshold)
        {
            _hasFallen = true; 
            
            OnPinFall?.Invoke(); 
            GlobalEventSystem.Broker.Publish(new BowlingPinFallEvent {  });

            //Debug.Log($"Keg {gameObject.name} has fallen! Angle: {angle}°");
            
            CancelInvoke(nameof(CheckIfFallen));
        }
    }

    private void OnDestroy()
    {
        CleanUp();
    }
    private void CleanUp()
    {
        //Debug.Log($"Cleaning up PinfallDetector on {gameObject.name}");
        
        // Отменяем все Invoke, которые могли остаться
        CancelInvoke(nameof(CheckIfFallen));
        
        // Очищаем подписки UnityEvent (если подписки были добавлены через код)
        OnPinFall.RemoveAllListeners();
    }
}