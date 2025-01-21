using Game.Scripts.Core.StateMachine;

namespace Game.Scripts.Infrastructure.LevelStateMachin.States
{
    public class LevelMissionState  : State<ELevelState>
    {
        LevelMissionState()
        {
            
        }

        public override void OnEnter(ITriggerResponder<ELevelState> stateMachine)
        {
            base.OnEnter(stateMachine);
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}