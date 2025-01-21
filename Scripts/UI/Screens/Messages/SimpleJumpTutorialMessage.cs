using Cysharp.Threading.Tasks;
using Game.Scripts.UI.Screens.Serviсes;
using UnityEngine;
using Zenject;

namespace Game.Scripts.UI.Screens.Messages
{
    public class SimpleJumpTutorialMessage : TutorialMessage
    {
        private MessageBoxService _messageBoxService;

        [Inject]
        public void Construct(MessageBoxService messageBoxService)
        {
            _messageBoxService = messageBoxService;
        }
        public override UniTask OnOpenStart()
        {
            return base.OnOpenStart();
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                _messageBoxService.CloseScreen<SimpleJumpTutorialMessage>();
            }
        }
    }
}