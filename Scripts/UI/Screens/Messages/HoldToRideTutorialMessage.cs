using Cysharp.Threading.Tasks;
using Game.Scripts.Configs.Vfx;
using Game.Scripts.Infrastructure.Services.Player;
using Game.Scripts.UI.Screens.Serviсes;
using UnityEngine;
using Zenject;

namespace Game.Scripts.UI.Screens.Messages
{
    public class HoldToRideTutorialMessage : TutorialMessage
    {
        private MessageBoxService _messageBoxService;

        [SerializeField] private GameObject _root;
        private SlowMotionService _slowMotionService;

        [Inject]
        public void Construct(MessageBoxService messageBoxService, SlowMotionService slowMotionService)
        {
            _slowMotionService = slowMotionService;
            _messageBoxService = messageBoxService;
        }
        public override UniTask OnOpenStart()
        {
            //_slowMotionService.StartSlowMo(ESlowMotionType.HoldToRideTutorial);
            _root.gameObject.SetActive(false);
            return base.OnOpenStart();
        }

        public override UniTask OnCloseStart()
        {
            _slowMotionService.StopSlowMo(ESlowMotionType.HoldToRideTutorial);
            return base.OnCloseStart();
        }

        public override UniTask OnCloseComplete()
        {
            _slowMotionService.StopSlowMo(ESlowMotionType.HoldToRideTutorial);
            return base.OnCloseComplete();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _root.gameObject.SetActive(false);
                _slowMotionService.StopSlowMo(ESlowMotionType.HoldToRideTutorial);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                _root.gameObject.SetActive(true);
                _slowMotionService.StartSlowMo(ESlowMotionType.HoldToRideTutorial);
            }
        }
    }
}