using System;
using System.Collections.Generic;
using RootMotion.Dynamics;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Animations;

namespace Game.Scripts.LevelElements.Player
{
    public class PlayerView : MonoBehaviour
    {
        [field: SerializeField]public PuppetMaster PuppetMaster { get; private set; }
        [field: SerializeField]  public Rigidbody RigidbodyRoot { get; private set; }
        [field: SerializeField] public ParentConstraint ParentConstraint{ get; private set; }
        [field:SerializeField] public List<PlayerMuscleGroup> MuscleGroups{ get; private set; }
        [field:SerializeField]public Transform CameraTarget { get; private set; }
        [field:SerializeField]public LineRenderer DirectionLineRenderer { get; private set; }
        [field:SerializeField]public PlayerBoneView[] PlayerBones { get; private set; }
        [field:SerializeField]public float speed { get; private set; }

        private void Update()
        {
            CameraTarget.transform.position = Vector3.Lerp(CameraTarget.transform.position,
                RigidbodyRoot.transform.position, speed * Time.deltaTime);
        }

        #region Muscle Setup

        // Button to populate the list of muscles in the inspector
        [Button("Populate Muscle Groups")]
        private void PopulateMuscleGroups()
        {
            MuscleGroups = new List<PlayerMuscleGroup>();
            var playerMuscles = PuppetMaster.muscles;
            Debug.Log(PuppetMaster);
            Debug.Log(PuppetMaster.muscles.Length);

            foreach (var muscle in playerMuscles)
            {
                var playerMuscleGroup = new PlayerMuscleGroup();
                playerMuscleGroup.MuscleJoint = muscle.joint;
                playerMuscleGroup.MuscleGroup = muscle.props.group;
                playerMuscleGroup.ExtendedMuscleGroup = DetermineSideFromName(muscle);
                if (!MuscleGroups.Contains(playerMuscleGroup))
                {
                    MuscleGroups.Add(playerMuscleGroup);
                }
            }
        }

        public EExtendedMuscleGroup DetermineSideFromName(Muscle muscle)
        {
            string name = muscle.joint.gameObject.name;

            if (name.Contains("UpperLeg"))
            {
                return name.EndsWith("l", System.StringComparison.OrdinalIgnoreCase) ? EExtendedMuscleGroup.LeftUpperLeg : EExtendedMuscleGroup.RightUpperLeg;
            }
            else if (name.Contains("LowerLeg"))
            {
                return name.EndsWith("l", System.StringComparison.OrdinalIgnoreCase) ? EExtendedMuscleGroup.LeftLowerLeg : EExtendedMuscleGroup.RightLowerLeg;
            }
            else if (name.Contains("Shoulder"))
            {
                return name.EndsWith("l", System.StringComparison.OrdinalIgnoreCase) ? EExtendedMuscleGroup.LeftShoulder : EExtendedMuscleGroup.RightShoulder;
            }
            else if (name.Contains("Elbow"))
            {
                return name.EndsWith("l", System.StringComparison.OrdinalIgnoreCase) ? EExtendedMuscleGroup.LeftElbow : EExtendedMuscleGroup.RightElbow;
            }
            else if (name.Contains("Hand"))
            {
                return name.EndsWith("l", System.StringComparison.OrdinalIgnoreCase) ? EExtendedMuscleGroup.LeftHand : EExtendedMuscleGroup.RightHand;
            }
            else if (name.Contains("Ankle"))
            {
                return name.EndsWith("l", System.StringComparison.OrdinalIgnoreCase) ? EExtendedMuscleGroup.LeftAnkle : EExtendedMuscleGroup.RightAnkle;
            }
            else if (muscle.props.group == Muscle.Group.Hips)
            {
                return EExtendedMuscleGroup.Hips;
            }
            else if (muscle.props.group == Muscle.Group.Spine)
            {
                return EExtendedMuscleGroup.Spine;
            }
            else if (muscle.props.group == Muscle.Group.Head)
            {
                return EExtendedMuscleGroup.Head;
            }
            else if (muscle.props.group == Muscle.Group.Tail)
            {
                return EExtendedMuscleGroup.Tail;
            }
            else if (muscle.props.group == Muscle.Group.Prop)
            {
                return EExtendedMuscleGroup.Prop;
            }
            return EExtendedMuscleGroup.Hips; 
        }

        #endregion
    }
    

    [Serializable]
    public class PlayerMuscleGroup
    {
        public Joint MuscleJoint;
        public Muscle.Group MuscleGroup;
        public EExtendedMuscleGroup ExtendedMuscleGroup;
    }
    public enum EExtendedMuscleGroup
    {
        Hips,
        Spine,
        Head,
        
        LeftElbow,
        RightElbow,
        LeftShoulder,
        RightShoulder,
        LeftHand,
        RightHand,
        
        LeftUpperLeg,
        RightUpperLeg,
        LeftLowerLeg,
        RightLowerLeg,
        LeftAnkle,
        RightAnkle,
        
        Tail,
        Prop
    }
}