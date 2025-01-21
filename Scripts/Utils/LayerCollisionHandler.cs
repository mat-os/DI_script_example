using UnityEngine;
using DG.Tweening;

public static class LayerCollisionHandler
{
    /// <summary>
    /// Временно отключает взаимодействие между двумя слоями.
    /// </summary>
    /// <param name="layer1">Первый слой.</param>
    /// <param name="layer2">Второй слой.</param>
    /// <param name="duration">Длительность отключения взаимодействия в секундах.</param>
    public static void TemporarilyIgnoreLayerCollision(int layer1, int layer2, float duration)
    {
        // Отключаем взаимодействие между слоями
        Physics.IgnoreLayerCollision(layer1, layer2, true);

        // Используем DoTween для возврата к исходному состоянию
        DOVirtual.DelayedCall(duration, () =>
        {
            // Включаем взаимодействие между слоями обратно
            Physics.IgnoreLayerCollision(layer1, layer2, false);
        }).SetUpdate(true); // Независимость от TimeScale
    }
}