using Game.Scripts.Configs.Vfx;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.Configs.Level
{
    [CreateAssetMenu(fileName = nameof(TutorialConfig), menuName = "Configs/" + nameof(TutorialConfig))]
    [InlineEditor]
    public class TutorialConfig : ScriptableObject
    {
        
        [field:Header("Hold To Ride")]
        //[field:SerializeField]public SlowMotionSettings HoldToRideSlowMotionSettings { get;private set; }
        [field:SerializeField]public int LevelsToShowHoldToRideTutorial { get;private set; }
        
        
        /*[field:Header("Swipe To Jump")]
        [field:SerializeField]public float TimeScaleOnTutorial { get;private set; }
        
        [field:Header("Simple Swipe To Jump")]
        [field:SerializeField]public float TimeScaleOnJumpTutorial { get;private set; }*/
    }
}