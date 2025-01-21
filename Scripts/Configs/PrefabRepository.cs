using Configs;
using Fungus;
using Game.Scripts.LevelElements.Car;
using Game.Scripts.LevelElements.Player;
using Game.Scripts.Utils;
using PG;
using RotaryHeart.Lib.SerializableDictionary;
using TMPro;
using UnityEngine;

namespace Game.Scripts.Configs
{
    [CreateAssetMenu(fileName = nameof(PrefabRepository), menuName = "Configs/" + nameof(PrefabRepository))]
    public class PrefabRepository : ScriptableObject
    {
        [field: Header("Player")]
        [field: SerializeField] public PlayerView PlayerView { get; private set; }      

        [field: Header("Cars")]
        [field: SerializeField] public PlayerController PlayerControllerPrefab { get; private set; }
        [field: SerializeField] public PlayerController PlayerControllerPrefab_ForMobile { get; private set; }
        [field: SerializeField] public SerializableDictionaryBase<ECarType, CarView> Cars { get; private set; }
        
        
        [field: Header("Flag")]
        [field:SerializeField] public GameObject FlagPrefab{ get; private set; }   
        [field:SerializeField] public AnimationParameterConfig FlagSpawnAnimationConfig{ get; private set; }  
        
        [field: Header("Fungus")]
        [field:SerializeField] public Flowchart Flowchart{ get; private set; }   
    }
}