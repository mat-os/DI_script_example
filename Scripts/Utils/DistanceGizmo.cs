using UnityEngine;

public class DistanceGizmo : MonoBehaviour
{
#if UNITY_EDITOR
    // Задаём три расстояния (публичные переменные видны в инспекторе)
    [SerializeField] private float distance1 = 1f;
    [SerializeField] private float distance2 = 2f;
    [SerializeField] private float distance3 = 3f;

    // Цвет линии
    [SerializeField] private Color lineColor1 = Color.red;
    [SerializeField] private Color lineColor2 = Color.yellow;
    [SerializeField] private Color lineColor3 = Color.green;

    // Смещение по X для каждой линии
    [SerializeField] private float xOffset1 = -0.5f; // Первая линия сдвинута влево
    [SerializeField] private float xOffset2 = 0f;    // Вторая линия посередине
    [SerializeField] private float xOffset3 = 0.5f;  // Третья линия сдвинута вправо

    // Метод для отрисовки Gizmos
    private void OnDrawGizmos()
    {
        // Устанавливаем цвет и рисуем первую линию
        Gizmos.color = lineColor1;
        Vector3 startPoint1 = transform.position + Vector3.right * xOffset1; // Смещение по X
        Vector3 point1 = startPoint1 + Vector3.back * distance1; // Смещение по Z
        Gizmos.DrawLine(startPoint1, point1);
        
        // Устанавливаем цвет и рисуем вторую линию
        Gizmos.color = lineColor2;
        Vector3 startPoint2 = transform.position + Vector3.right * xOffset2; // Смещение по X
        Vector3 point2 = startPoint2 + Vector3.back * distance2; // Смещение по Z
        Gizmos.DrawLine(startPoint2, point2);
        
        // Устанавливаем цвет и рисуем третью линию
        Gizmos.color = lineColor3;
        Vector3 startPoint3 = transform.position + Vector3.right * xOffset3; // Смещение по X
        Vector3 point3 = startPoint3 + Vector3.back * distance3; // Смещение по Z
        Gizmos.DrawLine(startPoint3, point3);
    }
#endif

}