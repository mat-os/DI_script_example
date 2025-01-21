using UnityEngine;

namespace Game.Scripts.LevelElements
{
    public class LevelView : MonoBehaviour
    {
        [field:Header("Player")]
        [field: SerializeField] public Transform PlayerRoot { get; private set; }
        
        [field:Header("Camera View")]
        [field: SerializeField] public LevelCameraView LevelCameraView { get; private set; }
        
        [field:Header("Cutscene View")]
        [field: SerializeField] public CutsceneView CutsceneView { get; private set; }

        [field:Header("Other")]
        [field: SerializeField] public ParticleSystem WindFx { get; private set; }
    }
}