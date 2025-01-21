using UnityEngine;

namespace Game.Scripts.LevelElements.NPC
{
    public class NPCAnimatorStateBehaviour: StateMachineBehaviour
    {
        [SerializeField] private int idleAnimationCount = 5;
        [SerializeField] private string IdleStateName = "IdleStateIndex";

        public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        {
            int randomIndex = Random.Range(0, idleAnimationCount);
            animator.SetInteger(IdleStateName, randomIndex);
            base.OnStateMachineEnter(animator, stateMachinePathHash);
        }

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Выбираем случайное значение для IdleStateIndex
            int randomIndex = Random.Range(0, idleAnimationCount);
            animator.SetInteger(IdleStateName, randomIndex);
        }
    }
}