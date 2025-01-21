using System;
using Game.Scripts.Core.Update.Interfaces;

namespace Game.Scripts.Core.StateMachine
{
    public interface IState<T> : IDisposable, IFixedUpdate, IUpdate
    {
        public void OnEnter(ITriggerResponder<T> stateMachine);

        public void OnExit();
    }
}