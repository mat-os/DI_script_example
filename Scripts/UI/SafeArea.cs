using System.Collections;
using UnityEngine;

namespace M3.BridgeRace.UI
{
    public class SafeArea : MonoBehaviour
    {
        private RectTransform _panel;
        private Rect _safeAreaRect;

        private void Awake()
        {
            _panel = GetComponent<RectTransform>();

            if (_panel == null)
                Destroy(gameObject);
        }

        private IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();

            _safeAreaRect = Screen.safeArea;
            ApplySafeArea(_safeAreaRect);
        }

        private void ApplySafeArea(Rect r)
        {
            Vector2 anchorMin = r.position;
            Vector2 anchorMax = r.position + r.size;
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;
            _panel.anchorMin = anchorMin;
            _panel.anchorMax = anchorMax;
        }
    }
}