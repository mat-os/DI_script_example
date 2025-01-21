using UnityEngine;

public class ShadowCasterPlayer : MonoBehaviour
{
    [SerializeField] private GameObject _shadowSprite;
    [SerializeField] private LayerMask _roadLayer;
    [SerializeField] private float _shadowDistance = 2.0f;
    [SerializeField] private ParticleSystem _particleSystem;

    [Header("Particle Color Settings")]
    [SerializeField] private Color _minHeightColor = Color.blue;  // Цвет при минимальной высоте
    [SerializeField] private Color _maxHeightColor = Color.red;   // Цвет при максимальной высоте
    [SerializeField] private float _minHeight = 0.5f;             // Минимальная высота над землей
    [SerializeField] private float _maxHeight = 2.0f;             // Максимальная высота над землей

    private float _currentHeight = 0f;

    private void Update()
    {
        CastShadowAndMeasureHeight();
        UpdateParticleColor();
    }

    void CastShadowAndMeasureHeight()
    {
        Vector3 rayStart = transform.position + Vector3.up * 1.0f; // Немного выше объекта
        Vector3 rayDirection = Vector3.down;

        // Один рейкаст для определения положения тени и высоты
        if (Physics.Raycast(rayStart, rayDirection, out RaycastHit hit, _shadowDistance, _roadLayer))
        {
            // Обновляем положение и вращение тени
            _shadowSprite.transform.position = hit.point;
            _shadowSprite.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

            // Запоминаем текущую высоту над землей
            _currentHeight = Vector3.Distance(transform.position, hit.point);
            _shadowSprite.gameObject.SetActive(true);
        }
        else
        {
            // Если рейкаст не попал, высота равна максимальной дистанции
            _shadowSprite.gameObject.SetActive(false);
            _currentHeight = _shadowDistance;
        }
    }

    void UpdateParticleColor()
    {
        // Нормализация высоты в диапазон от 0 до 1
        float t = Mathf.InverseLerp(_minHeight, _maxHeight, _currentHeight);

        // Интерполяция цвета
        Color particleColor = Color.Lerp(_minHeightColor, _maxHeightColor, t);

        // Применение цвета к ParticleSystem
        var mainModule = _particleSystem.main;
        mainModule.startColor = particleColor;
    }
}
