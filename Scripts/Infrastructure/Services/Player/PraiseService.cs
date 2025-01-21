using System;
using System.Collections.Generic;
using System.Linq;
using Configs;
using Game.Scripts.Configs.Player;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Infrastructure.Services.Player
{
    public class PraiseService: IDisposable
    {
        private readonly List<PraisePhraseConfig> _phraseConfigs;
        private Dictionary<PraisePhraseConfig, bool> _isInvokedByPraise;
        
        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        private PlayerScoreService _scoreService;
        private int _destroyedObjectsCount;
        private int _bumpedPeopleCount;
        private int _destroyedNpcCarsCount;
        private PlayerComboService _playerComboService;

        [Inject]
        public PraiseService(GameConfig gameConfig, PlayerScoreService scoreService, PlayerComboService playerComboService)
        {
            _playerComboService = playerComboService;
            _scoreService = scoreService;
            _phraseConfigs = gameConfig.RewardConfig.PraisePhraseConfigs;
            InitDictionaries(_phraseConfigs);
            SubscribeEvents();
        }

        private void InitDictionaries(List<PraisePhraseConfig> rewardPhraseConfigs)
        {
            _isInvokedByPraise = new Dictionary<PraisePhraseConfig, bool>();
            foreach (var config in rewardPhraseConfigs)
            {
                _isInvokedByPraise[config] = false;
            }
        }
        
        public void SubscribeEvents()
        {
            GlobalEventSystem.Broker.Receive<BrokeBoneEvent>()
                .Subscribe(BrokeBoneEventHandler)
                .AddTo(_disposable);
            GlobalEventSystem.Broker.Receive<DestroyObjectEvent>()
                .Subscribe(DestroyObjectEventHandler)
                .AddTo(_disposable);
            GlobalEventSystem.Broker.Receive<HitPeopleEvent>()
                .Subscribe(BumpPeopleEventHandler)
                .AddTo(_disposable);       
            
            GlobalEventSystem.Broker.Receive<PlayerCarHitWallEvent>()
                .Subscribe(PlayerCarHitWallEventHandler)
                .AddTo(_disposable);
            
            GlobalEventSystem.Broker.Receive<CarNPCDestroyedEvent>()
                .Subscribe(CarNpcDestroyedHandler)
                .AddTo(_disposable);
            
            _playerComboService.OnComboChange += OnComboChangeHandler;
        }

        private void OnComboChangeHandler(int combo)
        {
            foreach (var phrase in _phraseConfigs)
            {
                if (phrase.EPraiseType == EPraiseType.Combo)
                {
                    if (_isInvokedByPraise[phrase] == false)
                    {
                        if (combo >= phrase.TargetValue)
                        {
                            InvokePraise(phrase);
                        }
                    }
                }
            }
        }

        private void DestroyObjectEventHandler(DestroyObjectEvent destroyObjectEvent)
        {
            _destroyedObjectsCount++;
            foreach (var phrase in _phraseConfigs)
            {
                if (phrase.EPraiseType == EPraiseType.ObjectDestroy)
                {
                    if (_isInvokedByPraise[phrase] == false)
                    {
                        if (_destroyedObjectsCount >= phrase.TargetValue)
                        {
                            InvokePraise(phrase);
                        }
                    }
                }
            }
        }       
        private void BumpPeopleEventHandler(HitPeopleEvent bumpedPeoplEvent)
        {
            _bumpedPeopleCount++;
            foreach (var phrase in _phraseConfigs)
            {
                if (phrase.EPraiseType == EPraiseType.PeopleBump)
                {
                    if (_isInvokedByPraise[phrase] == false)
                    {
                        if (_bumpedPeopleCount >= phrase.TargetValue)
                        {
                            InvokePraise(phrase);
                        }
                    }
                }
            }
        }       
        
        private void PlayerCarHitWallEventHandler(PlayerCarHitWallEvent hitWallEvent)
        {
            foreach (var phrase in _phraseConfigs)
            {
                if (phrase.EPraiseType == EPraiseType.WallHit)
                {
                    if (_isInvokedByPraise[phrase] == false)
                    {
                        if (hitWallEvent.CarSpeed >= phrase.TargetValue)
                        {
                            InvokePraise(phrase);
                        }
                    }
                }
            }
        }      
        private void CarNpcDestroyedHandler(CarNPCDestroyedEvent carNpcDestroyedEvent)
        {
            _destroyedNpcCarsCount++;
            foreach (var phrase in _phraseConfigs)
            {
                if (phrase.EPraiseType == EPraiseType.HitNpcCar)
                {
                    if (_isInvokedByPraise[phrase] == false)
                    {
                        if (_destroyedNpcCarsCount >= phrase.TargetValue)
                        {
                            InvokePraise(phrase);
                        }
                    }
                }
            }
        }
        
        private void BrokeBoneEventHandler(BrokeBoneEvent brokeBoneEvent)
        {
            foreach (var phrase in _phraseConfigs)
            {
                if (phrase.EPraiseType == EPraiseType.BrokeBones)
                {
                    if (_isInvokedByPraise[phrase] == false)
                    {
                        if (brokeBoneEvent.TotalBrokenBones >= phrase.TargetValue)
                        {
                            InvokePraise(phrase);
                        }
                    }
                }
            }
        }

        private void InvokePraise(PraisePhraseConfig phrase)
        {
            ShowPhrase(phrase);
            _scoreService.AddScoreForPraise(phrase.ScoreToAdd);
            _isInvokedByPraise[phrase] = true;
        }

        private void ShowPhrase(PraisePhraseConfig phrase)
        {
            GlobalEventSystem.Broker.Publish(new ShowPraisePhraseEvent() {PraisePhrase  = phrase.Phrase, ScoreToAdd = phrase.ScoreToAdd});
        }
        public void Clear()
        {
            _isInvokedByPraise = _isInvokedByPraise
                .ToDictionary(kvp => kvp.Key, kvp => false);

            _destroyedObjectsCount = 0;
            _bumpedPeopleCount = 0;
            _destroyedNpcCarsCount = 0;
        }
        public void Dispose()
        {
            _disposable?.Dispose();
            _playerComboService.OnComboChange -= OnComboChangeHandler;
        }
    }
}