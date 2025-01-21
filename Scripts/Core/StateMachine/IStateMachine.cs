using Stateless;

namespace Game.Scripts.Core.StateMachine
{
    public interface IStateMachine<TState, TTrigger> : ITriggerResponder<TTrigger>
        where TState : IState<TTrigger>
    {
        public StateMachine<TState, TTrigger> StateMachine { get; }
        public void Initialize();
        public TState GetState();
        public void SetState(TState state);
        public StateMachine<TState, TTrigger>.StateConfiguration ConfigureState(TState state);
        public void OnEnterState(TState state);
        public void OnExitState(TState state);
    }
}