using Game.Scripts.Core.StateMachine;
using Game.Scripts.Infrastructure.Services;
using Game.Scripts.Infrastructure.Services.Level;

namespace Game.Scripts.Infrastructure.LevelStateMachin.States
{
    public class ExitLevelState : State<ELevelState>
    {
        private TargetHitMissionService _targetHitMissionService;
        private ObjectsDestroyMissionService _objectsDestroyMissionService;
        private BowlingMissionService _bowlingMissionService;
        private DestroyCarMissionService _destroyCarMissionService;
        private PeopleHitMissionService _peopleHitMissionService;
        private CameraService _cameraService;

        ExitLevelState(TargetHitMissionService targetHitMissionService, ObjectsDestroyMissionService objectsDestroyMissionService, 
            BowlingMissionService bowlingMissionService, DestroyCarMissionService destroyCarMissionService, 
            PeopleHitMissionService peopleHitMissionService, CameraService cameraService)
        {
            _cameraService = cameraService;
            _peopleHitMissionService = peopleHitMissionService;
            _destroyCarMissionService = destroyCarMissionService;
            _bowlingMissionService = bowlingMissionService;
            _objectsDestroyMissionService = objectsDestroyMissionService;
            _targetHitMissionService = targetHitMissionService;
        }

        public override void OnEnter(ITriggerResponder<ELevelState> stateMachine)
        {
            _targetHitMissionService.Clear();
            _peopleHitMissionService.Clear();
            _objectsDestroyMissionService.Clear();
            _bowlingMissionService.Clear();
            _destroyCarMissionService.Clear();
            _cameraService.Clear();
            base.OnEnter(stateMachine);
        }
    }
}