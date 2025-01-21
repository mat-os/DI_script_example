using UnityEngine;

public static class TransformUtil
{
    public static void Clear(this Transform parent)
    {
        if (parent == null)
        {
            Debug.LogWarning("Parent transform is null.");
            return;
        }

        int childCount = parent.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            Transform child = parent.GetChild(i);
            if (Application.isEditor)
            {
                Object.DestroyImmediate(child.gameObject);
            }
            else
            {
                Object.Destroy(child.gameObject);
            }
        }
    }
}