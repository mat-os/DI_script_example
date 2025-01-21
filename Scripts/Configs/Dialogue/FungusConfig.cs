using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.Configs.Dialogue
{
    [InlineEditor]
    [CreateAssetMenu(fileName = nameof(FungusConfig), menuName = "Configs/FungusConfig")]
    public class FungusConfig: ScriptableObject
    {
        [field:SerializeField]public bool IsShowDialogues { get; private set; }
    }
}