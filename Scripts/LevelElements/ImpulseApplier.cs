using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public class ImpulseApplier : MonoBehaviour
{
    [Header("Impulse Settings")]
    [SerializeField] private Vector3 impulseDirection = Vector3.forward; // Направление импульса
    [SerializeField] private float impulseStrength = 10f; // Базовая сила импульса
    [SerializeField] private bool scaleWithMass = true; // Учитывать ли массу при расчёте импульса

    [Header("Random Rotation Settings")]
    [SerializeField] private bool applyRandomRotation = true; // Применять ли случайное вращение
    [SerializeField] private float randomRotationStrength = 10f; // Максимальная сила вращения

    [Header("Gizmo Settings")]
    [SerializeField] private float gizmoLineLength = 2f; // Длина линии Gizmos
    [SerializeField] private Color gizmoLineColor = Color.red; // Цвет линии Gizmos

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        ApplyImpulse();
    }

    /// <summary>
    /// Применяет импульс и случайное вращение к объекту.
    /// </summary>
    [Button]
    public void ApplyImpulse()
    {
        if (_rigidbody == null)
        {
            Debug.LogWarning("Rigidbody is missing.");
            return;
        }

        // Учитываем массу при расчёте импульса
        float massFactor = scaleWithMass ? _rigidbody.mass : 1f;
        Vector3 impulse = impulseDirection.normalized * impulseStrength * massFactor;
        _rigidbody.AddForce(impulse, ForceMode.Impulse);

        // Применяем случайное вращение
        if (applyRandomRotation)
        {
            Vector3 randomTorque = Random.onUnitSphere * randomRotationStrength;
            _rigidbody.AddTorque(randomTorque, ForceMode.Impulse);
        }
    }

    private void OnDrawGizmos()
    {
        // Рисуем линию Gizmos от центра объекта в направлении импульса
        Gizmos.color = gizmoLineColor;
        Vector3 startPoint = transform.position;
        Vector3 endPoint = startPoint + impulseDirection.normalized * gizmoLineLength;
        Gizmos.DrawLine(startPoint, endPoint);

        // Рисуем маленькую сферу на конце линии для визуализации направления
        Gizmos.DrawSphere(endPoint, 0.1f);
    }
}
