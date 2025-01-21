using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Scripts.UI.Screens.Base.Screens;
using Game.Scripts.UI.Screens.Serviсes;
using UnityEngine;
using Zenject;

namespace Game.Scripts.UI.Screens.Messages
{
    public class TrophyCollectMessage : MessageBox
    {
        [SerializeField]private float _delayBeforeHide;
        
        private MessageBoxService _messageBoxService;
        
        [Inject]
        private void Construct(MessageBoxService messageBoxService)
        {
            _messageBoxService = messageBoxService;
        }
        public override UniTask OnOpenStart()
        {
            DOVirtual.DelayedCall(_delayBeforeHide, () =>
            {
                _messageBoxService.CloseScreen<TrophyCollectMessage>();
            });
            return base.OnOpenStart();
        }
    }
}