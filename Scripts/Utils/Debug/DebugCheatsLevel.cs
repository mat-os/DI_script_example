using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Configs;
using Game.Scripts.Core.CurrencyService;
using Game.Scripts.Infrastructure.GameStateMachine;
using Game.Scripts.Infrastructure.LevelStateMachin;
using Game.Scripts.Infrastructure.Services;
using Game.Scripts.Infrastructure.Services.Car;
using Game.Scripts.Infrastructure.Services.Player;
using Game.Scripts.Utils;
using SRDebugger;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Debugging
{
    public class DebugCheatsLevel  : INotifyPropertyChanged, ITickable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private GameStateMachine _gameStateMachine;
        private LevelStateMachine _levelStateMachine;
        private CurrencyService _currencyService;
        private LevelsRepository _levelsRepository;
        private LevelDataService _levelDataService;
        private DataService _dataService;
        private PlayerFlightLaunchService _playerFlightLaunchService;
        private CarService _carService;

        [Inject]
        public void Construct
        (
            GameStateMachine gameStateMachine,
            LevelStateMachine levelStateMachine,
            CurrencyService currencyService,
            LevelsRepository levelsRepository,
            LevelDataService levelDataService,
            DataService dataService,
            PlayerFlightLaunchService playerFlightLaunchService,
            CarService carService
        )
        {
            _carService = carService;
            _playerFlightLaunchService = playerFlightLaunchService;
            _dataService = dataService;
            _levelDataService = levelDataService;
            _levelsRepository = levelsRepository;
            _currencyService = currencyService;
            _gameStateMachine = gameStateMachine;
            _levelStateMachine = levelStateMachine;
        }
        public void Tick()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartLevel();
            }
            if (Input.GetKeyDown(KeyCode.N))
            {
                CompleteLevel();
            }        
            if (Input.GetKeyDown(KeyCode.F))
            {
                _playerFlightLaunchService.LaunchPlayerFromCar();
            }
            /*if (Input.GetKeyDown(KeyCode.Alpha9) && creativeCamera != null)
            {
                creativeCamera.gameObject.SetActive(!creativeCamera.isActiveAndEnabled);
            }*/
        }
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);

            return true;
        }

        [Category("Player")]
        public void DealCarDamage()
        {
            _carService.DebugDealCarDamage();
        }
        [Category("Currency")]
        [Increment(50)]
        public int Money
        {
            get { return _currencyService.GetCurrencyValue(ECurrencyType.Coins); }
            set
            {
                _currencyService.AddCurrency(ECurrencyType.Coins, value);
                OnPropertyChanged();
            }
        }    
        [Category("Currency")]
        public int Diamond
        {
            get { return _currencyService.GetCurrencyValue(ECurrencyType.Diamonds);; }
            set
            {
                _currencyService.AddCurrency(ECurrencyType.Diamonds, value);
                OnPropertyChanged();
            }
        }
        /*[Category("Level")] 
        [DisplayName("Level index")]
        public int LevelIndex
        {
            get => _dataService.Level.LevelIndex.Value;
            set
            {
                _dataService.Level.LevelIndex.Value = value;
                LevelName = _levelDataService.GetCurrentLevelData().Level.name;

                OnPropertyChanged();
            }
        }*/
        [Category("Level")]
        [DisplayName("Level name")]
        public string LevelName { get; private set; }
 
        [Category("Level")] 
        public void NextLevel()
        {
            _levelStateMachine.FireTrigger(ELevelState.Complete);
           // _gameStateMachine.FireTrigger(EGameState.LevelComplete);
        }
        [Category("Level")] 
        public void RestartLevel()
        {
            _levelStateMachine.FireTrigger(ELevelState.Exit);
            _gameStateMachine.FireTrigger(EGameState.LevelLoading);
        }
        [Category("Level")] 
        public void CompleteLevel()
        {
            _levelStateMachine.FireTrigger(ELevelState.Complete);
            // _gameStateMachine.FireTrigger(EGameState.LevelComplete);
        }       
        [Category("FPS")]
        public bool ShowFPS
        {
            get => FPSMeter.Show;
            set
            {
                FPSMeter.Show = value;
                OnPropertyChanged();
            }
        }


    }
}