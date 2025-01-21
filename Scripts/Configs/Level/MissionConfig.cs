using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.Configs.Level
{
    [CreateAssetMenu(fileName = nameof(MissionConfig), menuName = "Missions/" + nameof(MissionConfig))]
    [InlineEditor]
    public class MissionConfig : ScriptableObject
    {
        [field:InfoBox("Gameplay")]
        [field:SerializeField]public string TextForGameplayDescriptionLineUI{ get;private set; }

        [field:InfoBox("Level Info")]
        [field:SerializeField]public string TextForMissionInfoDescriptionLineUI{ get;private set; }
        
        [field:InfoBox("Weekly Info")]
        [field:SerializeField]public string TextForMissionDescriptionLineUI{ get;private set; }
        
        [field:InfoBox("For Level Complete Progress Bar")]
        [field:SerializeField]public string TextForLevelCompleteProgressBarUI { get;private set; }

    }

    public enum EMissionType
    {
        Distance = 0,
        BonesBroken = 1,
        DealDamageToPlayer = 2,
        ObjectsDestroyed = 3,
        TargetHit = 4,
        EarnScore = 5,
        Bowling = 6,
        HitPeople = 7,
        HitCars = 8,
        CollectTrophy = 9
    }
}