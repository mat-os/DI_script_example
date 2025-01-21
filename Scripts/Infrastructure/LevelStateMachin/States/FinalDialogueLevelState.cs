using Configs;
using DG.Tweening;
using Game.Scripts.Core.StateMachine;
using Game.Scripts.Infrastructure.Services;
using Game.Scripts.Infrastructure.Services.Analytics;

namespace Game.Scripts.Infrastructure.LevelStateMachin.States
{
    public class FinalDialogueLevelState : State<ELevelState>
    {
        FinalDialogueLevelState()
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