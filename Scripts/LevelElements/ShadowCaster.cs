using UnityEngine;

public class ShadowCaster : MonoBehaviour
{
    [SerializeField]private GameObject _shadowSprite; 
    [SerializeField]private LayerMask _roadLayer; 
    [SerializeField]private float _shadowDistance = 2.0f; 

    private void Update()
    {
        CastShadow();
    }

    void CastShadow()
    {
        Vector3 rayStart = transform.position + Vector3.up * 1.0f; // Немного выше машины
        Vector3 rayDirection =  Vector3.down;

        // Проверка пересечения с дорогой
        if (Physics.Raycast(rayStart, rayDirection, out RaycastHit hit, _shadowDistance, _roadLayer))
        {
            _shadowSprite.transform.position = hit.point;
            _shadowSprite.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
        }
    }
}