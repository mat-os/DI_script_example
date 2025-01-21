using DanielLochner.Assets.SimpleScrollSnap;
using Game.Scripts.Utils.Debug;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.Widgets
{
    public class LobbySelectPageUI : MonoBehaviour
    {
        [SerializeField] private SimpleScrollSnap scrollSnap;  
        [SerializeField] private LobbySelectPageButton[] buttons;  
        [SerializeField] private int _startingPageIndex;  

        private LobbySelectPageButton _currentActiveButton;

        [Inject] private DiContainer _container;
        
        
        public void OnCreate()
        {
            foreach (var button in buttons)
            {
                _container.Inject(button);
                button.Initialize(scrollSnap, OnButtonClicked);
            }
            
            //TODO: Нерпавильный выбор панелек 
            _currentActiveButton = buttons[_startingPageIndex];
            scrollSnap.OnPanelSelected.AddListener(OnPanelChanged);
            UpdateButtonStyles();
        }

        private void OnPanelChanged(int pageIndex)
        {
            CustomDebugLog.Log("[LOBBY UI] OnPanelChanged " + pageIndex);
            UpdateButtonStyles();
        }
        private void OnButtonClicked(int pageIndex)
        {
            Debug.Log($"[LOBBY UI] onButtonClicked {pageIndex}");
            _currentActiveButton = buttons[pageIndex];
            UpdateButtonStyles();
        }

        private void UpdateButtonStyles()
        {
            foreach (var button in buttons)
            {
                bool isSelected = button.PageIndex == _currentActiveButton.PageIndex;
                button.UpdateButtonStyle(isSelected);
            }
        }


        public void OnOpenStart()
        {
            foreach (var button in buttons)
            {
                button.OnOpenStart();
            }
        }
    }
}