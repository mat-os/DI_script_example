using System;
using DG.Tweening;
using Game.Scripts.Infrastructure.Services.Player;
using TMPro;
using UnityEngine;
using Zenject;

namespace Game.Scripts.UI.Widgets
{
    public class AirTimeTextUI : MonoBehaviour , IDisposable
    {
        [SerializeField] private Transform _root;
        [SerializeField] private TextMeshProUGUI _airTimeText;
        [SerializeField] private ShowMoveUIAnimation _showMoveUIAnimation;
        
        private PlayerAirTimeCounterService _airTimeCounterService;
        private PlayerFlightLaunchService _playerFlightLaunchService;

        [Inject]
        public void Construct(PlayerAirTimeCounterService airTimeCounterService, PlayerFlightLaunchService playerFlightLaunchService)
        {
            _playerFlightLaunchService = playerFlightLaunchService;
            _airTimeCounterService = airTimeCounterService;
            
            _airTimeCounterService.OnPlayerAirTimeChange += PlayerAirTimeChangeHandler;
            _airTimeCounterService.OnPlayerTouchGround += PlayerTouchGroundHandler;
            _playerFlightLaunchService.OnPlayerFlyStart += PlayerFlyStartHandler;
        }

        public void OnOpenStart()
        {
            _root.gameObject.SetActive(false);
        }

        private void PlayerTouchGroundHandler(float airTime)
        {
            DOVirtual.DelayedCall(1.5f, () => { _showMoveUIAnimation.Hide(_root); });
        }

        private void PlayerFlyStartHandler()
        {
            _showMoveUIAnimation.Show(_root);
        }

        private void PlayerAirTimeChangeHandler(float airTime)
        {
            _airTimeText.text = airTime.ToString("F1");
        }

        public void Dispose()
        {
            _airTimeCounterService.OnPlayerAirTimeChange -= PlayerAirTimeChangeHandler;
            _airTimeCounterService.OnPlayerTouchGround -= PlayerTouchGroundHandler;
            _playerFlightLaunchService.OnPlayerFlyStart -= PlayerFlyStartHandler;
        }
    }
}