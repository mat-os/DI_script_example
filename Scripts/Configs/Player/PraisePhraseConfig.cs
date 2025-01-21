using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.Configs.Player
{
    [CreateAssetMenu(fileName = nameof(PraisePhraseConfig), menuName = "Configs/" + nameof(PraisePhraseConfig))]
    [InlineEditor]
    public class PraisePhraseConfig: ScriptableObject
    {
        [field:SerializeField] public EPraiseType EPraiseType {get;private set;}
        [field:SerializeField] public string Phrase {get;private set;}
        [field:SerializeField] public int ScoreToAdd {get;private set;}
        [field:SerializeField] public  int TargetValue {get;private set;}
    }

    public enum EPraiseType
    {
        BrokeBones,
        Combo,
        GetStars,
        WallHit,
        ObjectDestroy,
        PeopleBump,
        HitNpcCar
    }
}