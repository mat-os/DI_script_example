using UnityEngine;
using UnityEngine.UI;

public class SmoothImageOffset : MonoBehaviour
{
    [SerializeField] private Image targetImage; // UI Image, на которой изменяем оффсет
    [SerializeField] private float speedX = 0.1f; // Скорость смещения по оси X
    [SerializeField] private float speedY = 0.1f; // Скорость смещения по оси Y

    private Material _material; // Материал, связанный с изображением
    private Vector2 _currentOffset; // Текущий оффсет (X, Y)

    private void Awake()
    {
        // Создаем экземпляр материала, чтобы изменения не затрагивали другие изображения
        if (targetImage != null)
        {
            _material = targetImage.material;
        }
        else
        {
            Debug.LogError("Не назначен targetImage для SmoothImageOffset");
        }
    }

    private void Update()
    {
        if (_material == null) return;

        // Увеличиваем смещение по X и Y на основе времени
        _currentOffset.x += speedX * Time.deltaTime;
        _currentOffset.y += speedY * Time.deltaTime;

        // Ограничиваем смещение в пределах [0, 1] для повторяющегося эффекта
        _currentOffset.x = Mathf.Repeat(_currentOffset.x, 1f);
        _currentOffset.y = Mathf.Repeat(_currentOffset.y, 1f);

        // Устанавливаем новый оффсет для текстуры
        _material.SetTextureOffset("_MainTex", _currentOffset);
    }
}