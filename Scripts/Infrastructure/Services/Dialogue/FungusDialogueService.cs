using System;
using System.Collections.Generic;
using Configs;
using Fungus;
using Game.Scripts.Configs;
using Game.Scripts.Configs.Dialogue;
using Game.Scripts.Infrastructure.Services;
using Game.Scripts.Infrastructure.Services.Analytics;
using Game.Scripts.Infrastructure.Services.Car;
using Game.Scripts.Infrastructure.Services.Dialogue;
using Game.Scripts.Infrastructure.Services.Level;
using Game.Scripts.UI.Screens.Popups;
using Game.Scripts.UI.Screens.Serviсes;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class FungusDialogueService : MonoBehaviour, IDialogueService
{
    public Action OnStartDialogue;
    public Action OnEndDialogue;
    
    [SerializeField]private Flowchart _flowchart;
    [SerializeField]private SayDialog _sayDialog;
    
    private PopupService _popupService;
    private FungusConfig _fungusConfig;
    private CarInputService _carInputService;
    
    private List<string> _playedDialogues = new List<string>();
    private LevelsProgressService _levelsProgressService;
    private LevelPackService _levelPackService;
    private LevelDataService _levelDataService;
    private DialogueConfig _currentDialogueConfig;
    private Action _dialogueCompleteAction;
    private AnalyticService _analyticService;

    [Inject]
    public void Construct(PopupService popupService, GameConfig gameConfig, CarInputService carInputService,
        LevelPackService levelPackService, LevelDataService levelDataService, LevelsProgressService levelsProgressService,
        AnalyticService analyticService)
    {
        _analyticService = analyticService;
        _levelsProgressService = levelsProgressService;
        _levelDataService = levelDataService;
        _levelPackService = levelPackService;
        _carInputService = carInputService;
        _fungusConfig = gameConfig.FungusConfig;
        _popupService = popupService;

        _sayDialog.OnSkipDialogueButtonClick += SkipDialogueButtonClickHandle;
    }

    private void SkipDialogueButtonClickHandle()
    {
        _analyticService.LogUIClick("skip_dialogue");
        EndDialogue();
    }

    public void StartDialogue(DialogueConfig dialogueConfig, Action onDialogueCompleted)
    {
        if (dialogueConfig == null)
        {
            EndDialogue();
            return;
        }
        if (!IsDialogueBlockAvailable(dialogueConfig.DialogueName))
        {
            Debug.LogWarning($"Block {dialogueConfig.DialogueName} not found in Flowchart.");
            return;
        }
        
        _dialogueCompleteAction = onDialogueCompleted;
        if (!ShouldShowDialogue(dialogueConfig))
        {
            EndDialogue();
            return;
        }

        PlayDialogue(dialogueConfig);
    }
    private bool ShouldShowDialogue(DialogueConfig dialogueConfig)
    {
        if (dialogueConfig.IsForceShowDialogue)
            return true;
        
        //TODO: Не работает на 4 уровне
        var currentPackId = _levelPackService.GetCurrentLevelPack().GetPackId();
        var currentLevelId = _levelDataService.LevelId;
        var isLevelCompleted = _levelsProgressService.GetLevelProgress(currentPackId, currentLevelId).Stars > 0;
        
        return _fungusConfig.IsShowDialogues &&
               dialogueConfig != null &&
               !_playedDialogues.Contains(dialogueConfig.DialogueName) &&
                isLevelCompleted == false;
    }
    private bool IsDialogueBlockAvailable(string dialogueName)
    {
        return _flowchart.HasBlock(dialogueName);
    }
    private void PlayDialogue(DialogueConfig dialogueConfig)
    {
        if (_currentDialogueConfig != null)
        {
            Debug.LogWarning("A dialogue is already active. Cannot start a new one.");
            return;
        }
        Debug.Log("Play Dialogue");

        _currentDialogueConfig = dialogueConfig;

        //_sayDialog.Clear();
        _sayDialog.gameObject.SetActive(true);
        _flowchart.ExecuteBlock(_currentDialogueConfig.DialogueName);

        _playedDialogues.Add(_currentDialogueConfig.DialogueName);
        _carInputService.SetIsReadInput(false);
        
        OnStartDialogue?.Invoke();
    }
    
    public void EndDialogue()
    {
        if (_currentDialogueConfig != null)
        {
            Debug.Log("End Dialogue");
            _sayDialog.gameObject.SetActive(false);
            _flowchart.StopBlock(_currentDialogueConfig.DialogueName);
            _sayDialog.Clear();
            _currentDialogueConfig = null;
        } 
        
        OnEndDialogue?.Invoke();
        _dialogueCompleteAction?.Invoke();
        _dialogueCompleteAction = null;
    }
}