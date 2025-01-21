using Game.Scripts.Core.StateMachine;
using Game.Scripts.Infrastructure.LevelStateMachin.States;

namespace Game.Scripts.Infrastructure.LevelStateMachin
{
    public sealed class LevelStateMachine : StateMachineBase<IState<ELevelState>, ELevelState>
    {
        public LevelStateMachine
        (
            PlayLevelState playLevelState,
            FailLevelState failLevelState,
            CompleteLevelState completeLevelState,
            ExitLevelState exitLevelState,
            FinalDialogueLevelState finalDialogueLevelState,
            LevelMissionState levelMissionState
        )
        {
            Initialize();

            ConfigureState(levelMissionState)
                .Permit(ELevelState.Play, playLevelState)
                .Permit(ELevelState.Exit, exitLevelState);
            
            ConfigureState(playLevelState)
                .Permit(ELevelState.Fail, failLevelState)
                .Permit(ELevelState.Complete, completeLevelState)
                .Permit(ELevelState.FinalDialogue, finalDialogueLevelState)
                .Permit(ELevelState.Exit, exitLevelState);

            ConfigureState(failLevelState)
                .Permit(ELevelState.Play, playLevelState)
                .Permit(ELevelState.Exit, exitLevelState);


            ConfigureState(completeLevelState)
                .Permit(ELevelState.Exit, exitLevelState);

            ConfigureState(exitLevelState)
                .Permit(ELevelState.Play, playLevelState)
                .Permit(ELevelState.LevelMission, levelMissionState);

            ConfigureState(finalDialogueLevelState)
                .Permit(ELevelState.Complete, completeLevelState);
                
            SetState(exitLevelState);
            OnEnterState(CurrentState);
        }
    }
}