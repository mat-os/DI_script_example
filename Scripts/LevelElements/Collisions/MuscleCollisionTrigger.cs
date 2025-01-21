using System;
using Game.Scripts.LevelElements.Player;
using RootMotion.Dynamics;
using UnityEngine;

namespace Game.Scripts.LevelElements.Collisions
{
    public class MuscleCollisionTrigger : CollisionComponent
    {
        [field: SerializeField] public  Muscle.Group PropsGroup { get; private set; }
        [field: SerializeField] public EExtendedMuscleGroup ExtendedMusculeGroup { get; private set; }
        [field: SerializeField] public Rigidbody Rigidbody { get; private set; }

        private void Awake()
        {
            if(Rigidbody == null)
                Rigidbody = GetComponent<Rigidbody>();
        }

        public void SetBodyPart(EExtendedMuscleGroup group, Muscle.Group propsGroup)
        {
            PropsGroup = propsGroup;
            ExtendedMusculeGroup = group;
        }
    }
}