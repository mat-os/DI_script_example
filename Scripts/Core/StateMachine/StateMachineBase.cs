using System;
using Game.Scripts.Utils.Debug;

namespace Game.Scripts.Core.StateMachine
{
    public abstract class StateMachineBase<TState, TTrigger> : IDisposable,
        IStateMachine<TState, TTrigger> where TState : IState<TTrigger>, IDisposable
    {
        public event Action<TState> StateChanged;
        public event Action<TState> EventStateEnter;
        public event Action<TState> EventStateExit;

        private Stateless.StateMachine<TState, TTrigger> _stateMachine;
        Stateless.StateMachine<TState, TTrigger> IStateMachine<TState, TTrigger>.StateMachine => _stateMachine;

        public TState CurrentState { get; protected set; }

        public virtual void Initialize()
        {
            _stateMachine = new Stateless.StateMachine<TState, TTrigger>(GetState, SetState);
        }

        public virtual void OnEnterState(TState state)
        {
            state.OnEnter(this);
            EventStateEnter?.Invoke(state);
        }

        public void ManualUpdate(float deltaTime)
        {
            CurrentState.ManualUpdate(deltaTime);
        }

        public void ManualManualFixedUpdate(float fixedDeltaTime)
        {
            CurrentState.ManualFixedUpdate(fixedDeltaTime);
        }

        public virtual void OnExitState(TState state)
        {
            state.OnExit();
            EventStateExit?.Invoke(state);
        }

        public virtual void Dispose()
        {
            StateChanged = null;
            EventStateEnter = null;
            EventStateExit = null;

            _stateMachine.Deactivate();
            CurrentState?.OnExit();
            CurrentState?.Dispose();
            CurrentState = default;

            CustomDebugLog.Log($"Dispose {GetType()}", DebugColor.Red);
        }

        public TState GetState()
        {
            return CurrentState;
        }

        public void SetState(TState state)
        {
            CustomDebugLog.Log($"{GetType().Name} New state {state.GetType().Name}", DebugColor.Orange);
            CurrentState = state;
            StateChanged?.Invoke(state);
        }

        public void FireTrigger(TTrigger trigger)
        {
            if (_stateMachine.CanFire(trigger))
            {
                try
                {
                    _stateMachine.Fire(trigger);
                }
                catch (Exception e)
                {
                    CustomDebugLog.LogError($"{trigger} cause error : {e}");
                }
            }
            else
                CustomDebugLog.LogWarning($"Cant change {CurrentState} by trigger {trigger}");
        }

        public Stateless.StateMachine<TState, TTrigger>.StateConfiguration ConfigureState(TState state)
        {
            return _stateMachine.Configure(state).OnEntry(() => OnEnterState(state)).OnExit(() => OnExitState(state));
        }
    }
}