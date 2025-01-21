using UnityEngine;

public class RandomActivator : MonoBehaviour
{
    [Header("Настройки вероятности")]
    [Range(0f, 1f)] 
    [Tooltip("Вероятность того, что объект будет включен при старте (от 0 до 1)")]
    public float activationProbability = 0.5f;

    [Header("Объект для управления (если оставить пустым, будет управляться текущий объект)")]
    public GameObject targetObject;

    private void Start()
    {
        // Если объект не задан, используем сам объект на котором висит скрипт
        if (targetObject == null)
        {
            targetObject = this.gameObject;
        }

        // Генерируем случайное число от 0 до 1
        float randomValue = Random.value;

        // Сравниваем значение с вероятностью
        bool shouldActivate = randomValue <= activationProbability;

        // Включаем или выключаем объект
        targetObject.SetActive(shouldActivate);
    }
}