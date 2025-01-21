using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.Widgets.Customization
{
    public class UIScrollViewPage : MonoBehaviour
    {
        [SerializeField] private Transform _viewport;
        [SerializeField] private ScrollRect _scrollRect;

        public Transform Viewport => _viewport;
    
        public void Show()
        {
            if (_scrollRect != null)
            {
                _scrollRect.velocity = Vector2.zero;
                _scrollRect.verticalNormalizedPosition = 1.0f;
            }
        }
    }
}
