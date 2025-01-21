using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class SwimmingPool : MonoBehaviour
{
    [Header("Buoyancy Settings")]
    [SerializeField] private float _buoyancyForce = 10f; // Сила, поднимающая объект вверх
    [SerializeField] private float _dragInWater = 2f; // Сопротивление в воде

    [Header("Water Bounce Settings")]
    [SerializeField] private float _minBounceSpeed = 5f; // Минимальная скорость для отскока
    [SerializeField] private float _bounceForceMultiplier = 2f; // Множитель силы отскока

    private BoxCollider _boxCollider;
    private HashSet<Rigidbody> _objectsInPool = new HashSet<Rigidbody>();

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider>();
        _boxCollider.isTrigger = true;
    }

    private void FixedUpdate()
    {
        foreach (var rb in _objectsInPool)
        {
            if (rb == null) continue;

            // Применяем плавучесть только если объект не был отброшен
            if (!IsBouncing(rb))
            {
                ApplyBuoyancy(rb);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody != null)
        {
            Rigidbody rb = other.attachedRigidbody;

            // Проверяем возможность отскока
            if (ShouldBounceOffWater(rb))
            {
                ApplyWaterBounce(rb);
                MarkAsBouncing(rb); // Помечаем объект как отскакивающий
            }
            else if (!_objectsInPool.Contains(rb))
            {
                _objectsInPool.Add(rb);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody != null)
        {
            Rigidbody rb = other.attachedRigidbody;

            if (_objectsInPool.Contains(rb))
            {
                _objectsInPool.Remove(rb);
            }

            UnmarkAsBouncing(rb); // Убираем пометку отскока
        }
    }

    private void ApplyBuoyancy(Rigidbody rb)
    {
        Vector3 centerOfMass = rb.worldCenterOfMass;

        // Проверяем, находится ли объект под водой
        if (centerOfMass.y < transform.position.y + _boxCollider.center.y + _boxCollider.size.y / 2)
        {
            // Добавляем силу плавучести вверх
            rb.AddForce(Vector3.up * _buoyancyForce, ForceMode.Acceleration);

            // Применяем дополнительное сопротивление (drag)
            rb.velocity *= 1f - (_dragInWater * Time.fixedDeltaTime);
        }
    }

    private bool ShouldBounceOffWater(Rigidbody rb)
    {
        // Проверяем, достаточно ли высокая скорость
        if (rb.velocity.magnitude < _minBounceSpeed)
        {
            return false;
        }

        // Проверяем угол движения объекта относительно воды
        Vector3 velocityDirection = rb.velocity.normalized;
        Vector3 waterNormal = Vector3.up; // Нормаль поверхности воды

        // Проверяем, движется ли объект вниз к воде (скалярное произведение меньше 0)
        if (Vector3.Dot(velocityDirection, waterNormal) >= 0)
        {
            return false;
        }

        return true; // Если скорость и угол позволяют, возвращаем true
    }

    private void ApplyWaterBounce(Rigidbody rb)
    {
        Vector3 velocity = rb.velocity;
        Vector3 waterNormal = Vector3.up; // Нормаль воды всегда вверх

        // Рассчитываем направление отражения от воды
        Vector3 reflectedVelocity = Vector3.Reflect(velocity, waterNormal);

        // Применяем новую скорость с учетом силы отскока
        rb.velocity = reflectedVelocity * _bounceForceMultiplier;
    }

    // Пометка для предотвращения конфликта отскока и плавучести
    private HashSet<Rigidbody> _bouncingObjects = new HashSet<Rigidbody>();

    private void MarkAsBouncing(Rigidbody rb)
    {
        _bouncingObjects.Add(rb);
    }

    private void UnmarkAsBouncing(Rigidbody rb)
    {
        _bouncingObjects.Remove(rb);
    }

    private bool IsBouncing(Rigidbody rb)
    {
        return _bouncingObjects.Contains(rb);
    }
}
