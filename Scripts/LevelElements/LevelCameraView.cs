using Cinemachine;
using UnityEngine;

namespace Game.Scripts.LevelElements
{
    public class LevelCameraView : MonoBehaviour
    {
        [field:Header("Cameras")]
        [field: SerializeField] public CinemachineVirtualCamera GameplayCamera { get; private set; }
        [field: SerializeField] public CinemachineVirtualCamera WallHitCamera { get; private set; }
        [field: SerializeField] public CinemachineVirtualCamera MenuCamera { get; private set; }
        [field: SerializeField] public CinemachineVirtualCamera FlyEndCamera { get; private set; }
        [field: SerializeField] public CinemachineVirtualCamera FlyCamera { get; private set; }
        [field: SerializeField] public CinemachineVirtualCamera[] OtherCamerasForPlayerHumanoidFollow { get; private set; }
        
    }
}