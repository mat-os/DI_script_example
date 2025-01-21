using UnityEngine;

namespace Game.Scripts.Utils
{
    public class AutoScale : MonoBehaviour
    {
        [Header("Настройки масштабирования")]
        [SerializeField] 
        private Vector3 direction = Vector3.one; // В каких направлениях изменять масштаб (по X, Y, Z)

        [SerializeField] 
        private float amplitude = 0.5f; // Максимальное изменение масштаба

        [SerializeField] 
        [Range(0.1f, 120.0f)] 
        private float interval = 2.0f; // Время на полный цикл увеличения/уменьшения масштаба

        private Vector3 startScale; // Начальный масштаб объекта
        private float time = 0f; // Счётчик времени

        void Start()
        {
            startScale = transform.localScale; // Сохраняем начальный масштаб объекта
        }

        void Update()
        {
            time += Time.deltaTime;
            float ang = (time % interval) / interval * Mathf.PI * 2.0f;

            // Вычисляем, как должен измениться масштаб по X, Y, Z
            Vector3 scaleOffset = Vector3.Scale(direction, new Vector3(Mathf.Sin(ang), Mathf.Sin(ang), Mathf.Sin(ang))) * amplitude;

            // Устанавливаем новый масштаб объекта
            transform.localScale = startScale + scaleOffset;
        }
    }
}