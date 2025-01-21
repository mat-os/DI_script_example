namespace Game.Scripts.Core.StateMachine
{
    public abstract class State<T> : IState<T>
    {
        protected ITriggerResponder<T> StateMachine;

        public virtual void OnEnter(ITriggerResponder<T> stateMachine)
        {
            StateMachine = stateMachine;
        }

        public virtual void ManualFixedUpdate(float fixedDeltaTime)
        {
        }

        public virtual void ManualUpdate(float deltaTime)
        {
        }

        public virtual void OnExit()
        {
            StateMachine = null;
        }

        public virtual void Dispose()
        {
            StateMachine = null;
        }
    }
}