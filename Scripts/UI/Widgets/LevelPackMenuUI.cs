using System.Collections.Generic;
using Configs;
using DanielLochner.Assets.SimpleScrollSnap;
using Game.Scripts.Configs.Level;
using Game.Scripts.Infrastructure.Services;
using Game.Scripts.Infrastructure.Services.Level;
using Game.Scripts.UI.Screens.Popups;
using Game.Scripts.UI.Screens.Serviсes;
using Game.Scripts.Utils.Debug;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.Widgets
{
    public class LevelPackMenuUI : MonoBehaviour
    {
        [SerializeField] private Button _nextLevelPack;
        [SerializeField] private Button _previousLevelPack;
        [SerializeField] private Transform _levelPackHolder;
        [SerializeField] private SimpleScrollSnap _simpleScrollSnap;

        private LevelPackService _levelPackService;
        private LevelsRepository _levelsRepository;
        private UIConfig _uiConfig;

        [Inject] private DiContainer _container;
        private PopupService _popupService;
        private List<LevelPackUI> _levelPacks = new List<LevelPackUI>();
        private DataService _dataService;
        private LevelsProgressService _levelsProgressService;

        [Inject]
        public void Construct(LevelPackService levelPackService,
            LevelsRepository levelsRepository,
            UIConfig uiConfig,
            PopupService popupService, 
            DataService dataService,
            LevelsProgressService levelsProgressService)
        {
            _levelsProgressService = levelsProgressService;
            _dataService = dataService;
            _popupService = popupService;
            _uiConfig = uiConfig;
            _levelsRepository = levelsRepository;
            _levelPackService = levelPackService;
            
            _simpleScrollSnap.StartingPanel = _dataService.Level.LevelPackIndex.Value;
        }

        public void GenerateLevelPacks()
        {
            for (var i = 0; i < _levelsRepository.LevelPackConfigs.Count; i++)
            {
                //var levelPackUI = Instantiate(_uiConfig.LevelPackUI, _levelPackHolder);
                _simpleScrollSnap.Add(_uiConfig.LevelPackUI.gameObject, i);
            }

            var levelPackUI = _simpleScrollSnap.GetComponentsInChildren<LevelPackUI>();
            for (var i = 0; i < levelPackUI.Length; i++)
            {
                var levelPack = levelPackUI[i];
                var levelPackConfig = _levelsRepository.LevelPackConfigs[i];
                _container.Inject(levelPack);
                levelPack.Initialize(i, levelPackConfig, OnClickHandler);
                _levelPacks.Add(levelPack);
            }

            for (var i = 0; i < _levelPacks.Count; i++)
            {
                _levelPacks[i].SetIsInteractable(i == 0);
            }

            _simpleScrollSnap.OnPanelCentered.AddListener(OnPanelCentered);
        }


        private void OnPanelCentered(int newPanel, int previousPanel)
        {
            //Debug.Log($"OnPanelCentered {newPanel}, previous panel: {previousPanel}");
            _levelPacks[newPanel].SetIsInteractable(true);
            _levelPacks[previousPanel].SetIsInteractable(false);
            
            _levelPackService.SetLevelPackByIndex(newPanel);
            UpdateButtonStates(newPanel);
        }

        private void OnClickHandler()
        {
            _popupService.ShowScreen<LevelPackSelectPopup>();
        }

        public void OnOpenStart()
        {
            for (int i = 0; i < _levelPacks.Count; i++)
            {
                var packConfig = _levelPacks[i].GetPackConfig();
                var isPackLocked = GetIsPackLocked(packConfig);
                _levelPacks[i].UpdateData(isPackLocked);   
            }
        }
        private bool GetIsPackLocked(LevelPackConfig levelPackConfig)
        {
            if (Debug.isDebugBuild || Application.isEditor)
            {
                if (_levelPackService.IsAllLevelsUnlocked)
                    return false;
            }

            var packId = levelPackConfig.GetPackId();
            if (packId == 0)
                return false;

            var previousPack =  _levelPacks[packId - 1].GetPackConfig();
            var previousPackId = previousPack.GetPackId();
            // Получаем прогресс предыдущего пака уровней
            var levelsCount = previousPack.LevelDataConfigs.Count - 1;
            var previousLevel = _levelsProgressService.GetPreviousLevel(previousPackId, levelsCount);
            return previousLevel.IsCompleted == false;
        }
        public void GoToCurrentPanel()
        {
            var panelNumber = _dataService.Level.LevelPackIndex.Value;
            _simpleScrollSnap.GoToPanel(panelNumber);
            UpdateButtonStates(panelNumber);
        }
        private void UpdateButtonStates(int currentPanelIndex)
        {
            // Проверяем, активна ли кнопка "предыдущая"
            _previousLevelPack.interactable = currentPanelIndex > 0;

            // Проверяем, активна ли кнопка "следующая"
            _nextLevelPack.interactable = currentPanelIndex < _simpleScrollSnap.NumberOfPanels - 1;
        }
    }
}