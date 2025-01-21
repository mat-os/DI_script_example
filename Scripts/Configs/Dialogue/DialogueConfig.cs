using UnityEngine;

namespace Game.Scripts.Configs.Dialogue
{
    [CreateAssetMenu(fileName = nameof(DialogueConfig), menuName = "Dialogue/DialogueConfig")]
    public class DialogueConfig : ScriptableObject
    {
        public string DialogueName; // Имя блока в Flowchart
        public bool IsForceShowDialogue; // Имя блока в Flowchart
    }
}