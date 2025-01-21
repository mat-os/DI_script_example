using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Configs
{
    [Serializable]
    public class DifficultySettings
    {
        [field:SerializeField]public List<DifficultyStep> DifficultySteps { get; private set; }
    }
    [Serializable]
    public class DifficultyStep
    {
        [field:GUIColor(0,1,0)]
        [field:SerializeField]public int AttemptNumber { get; private set; }

        [field: Header("Player Settings")]
        [field: SerializeField] public float FlyForceMultiplier { get; private set; } = 1;
        [field:SerializeField]public float JumpForceMultiplier { get; private set; } = 1;
        [field:SerializeField]public float CarAccelerationMultiplier { get; private set; } = 1;
        [field:SerializeField]public float PlayerDamageMultiplier { get; private set; } = 1;
        [field:SerializeField]public float PhysicsVelocityStopMultiplier { get; private set; } = 1;
    }
}