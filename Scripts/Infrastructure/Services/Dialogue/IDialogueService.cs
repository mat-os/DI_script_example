using System;
using Game.Scripts.Configs.Dialogue;

namespace Game.Scripts.Infrastructure.Services.Dialogue
{
    public interface IDialogueService
    {
        void StartDialogue(DialogueConfig dialogueConfig, Action onDialogueCompleted); // Принимает объект SO с параметрами диалога
        void EndDialogue();
    }
}