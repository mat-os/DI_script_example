using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.Utils
{
    [CreateAssetMenu(fileName = nameof(AnimationParameterConfig), menuName = "Configs/Settings/Tween/" + nameof(AnimationParameterConfig))]
    [InlineEditor]
    public class AnimationParameterConfig : ScriptableObject
    {
        [field: SerializeField] public float Duration { get; private set; }
        [field: SerializeField] public Ease Ease { get; private set; }
    }
}