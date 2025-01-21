namespace Game.Scripts.Core.StateMachine
{
    public interface ITriggerResponder<TTrigger>
    {
        public void FireTrigger(TTrigger trigger);
    }
}