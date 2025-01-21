using Configs;
using Cysharp.Threading.Tasks;
using Game.Scripts.Configs.Vfx;
using Game.Scripts.Infrastructure.Services.Player;
using Game.Scripts.UI.Screens.Serviсes;
using Game.Scripts.Utils.Debug;
using UnityEngine;
using Zenject;

namespace Game.Scripts.UI.Screens.Messages
{
    public class JumpTutorialMessage : TutorialMessage
    {
        private MessageBoxService _messageBoxService;
        
        [Header("Swipe Settings")]
        [SerializeField] private float swipeThreshold = 50f; // Минимальное расстояние для свайпа вверх
        [SerializeField] private float maxHorizontalDeviation = 30f; // Максимальное допустимое отклонение по X

        private Vector2 _startPosition; // Начальная позиция свайпа
        private PlayerFlightControlService _playerFlightControlService;
        private PlayerJumpService _playerJumpService;
        private SlowMotionService _slowMotionService;
        private PlayerConfig _playerConfig;

        [Inject]
        public void Construct(MessageBoxService messageBoxService,
            PlayerFlightControlService playerFlightControlService,
            PlayerJumpService playerJumpService,
            SlowMotionService slowMotionService,
            GameConfig gameConfig)
        {
            _playerConfig = gameConfig.PlayerConfig;
            _slowMotionService = slowMotionService;
            _playerJumpService = playerJumpService;
            _playerFlightControlService = playerFlightControlService;
            _messageBoxService = messageBoxService;
        }

        public override UniTask OnOpenStart()
        {
            _slowMotionService.StartSlowMo(ESlowMotionType.JumpTutorial);
            return base.OnOpenStart();
        }

        private void Update()
        {
            // Начало свайпа
            if (Input.GetMouseButtonDown(0))
            {
                _startPosition = Input.mousePosition;
            }

            // Завершение свайпа
            if (Input.GetMouseButtonUp(0))
            {
                Vector2 endPosition = Input.mousePosition;

                if (IsSwipeUp(_startPosition, endPosition))
                {
                    _messageBoxService.CloseScreen<JumpTutorialMessage>();
                }
            }
        }
        
        private bool IsSwipeUp(Vector2 start, Vector2 end)
        {
            float deltaY = end.y - start.y; // Движение по вертикали
            float deltaX = Mathf.Abs(end.x - start.x); // Движение по горизонтали
            
            // Условия для засчитывания свайпа вверх:
            // 1. Смещение по оси Y больше, чем swipeThreshold
            // 2. Смещение по оси X не превышает maxHorizontalDeviation (чтобы свайп был вертикальным)
            if (deltaY > swipeThreshold && deltaX < maxHorizontalDeviation)
            {
                //_playerFlightControlService.SetIsPlayerCanJump(true);
                _playerJumpService.PerformJump(new Vector2(0, _playerConfig.SwipeConfig.MaxSwipeDistance), 1);
                _slowMotionService.StopSlowMo(ESlowMotionType.JumpTutorial);
                CustomDebugLog.Log($"Свайп вверх засчитан! ΔY: {deltaY}, ΔX: {deltaX}");
                return true;
            }

            return false;
        }
    }
}