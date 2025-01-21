using UnityEngine;

namespace Game.Scripts.LevelElements.Player
{
    public class PlayerBoneView : MonoBehaviour
    {
        [field: SerializeField]public EExtendedMuscleGroup ExtendedMuscleGroup{ get; private set; }
        [field: SerializeField]public Renderer Renderer { get; private set; }
        
        public Vector3 RendererCenter => Renderer.bounds.center;

    }
}