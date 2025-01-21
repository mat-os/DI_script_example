using System;
using Game.Scripts.Utils;
using UnityEngine;

namespace Game.Scripts.UI.Widgets.Base
{
    [Serializable]
    public class WidgetSettings
    {
        [field: SerializeField] public CanvasGroup AnimatedRoot { get; private set; }
        [field: SerializeField] public AnimationParameterConfig ShowAnimationParameter { get; private set; }
        [field: SerializeField] public AnimationParameterConfig HideAnimationParameter { get; private set; }
    }
}