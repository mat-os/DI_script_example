using System.Collections.Generic;
using Cinemachine;
using RotaryHeart.Lib.SerializableDictionary;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.LevelElements
{
    public class CutsceneView : MonoBehaviour
    {
        [ListDrawerSettings(ShowIndexLabels = true)]
        [SerializeField] public SerializableDictionaryBase<ECameraType, CamerasCutsceneSettings> CutsceneCameras = new ();
    }

    [System.Serializable]
    public class CamerasCutsceneSettings
    {
        [SerializeField] public CinemachineVirtualCamera Camera; 
    }
    
    [System.Serializable]
    public class CameraTypeEvent : UnityEvent<CameraType> { }
    public enum ECameraType
    {
        Stuntman,
        Car,
        Director,
        View,
        StuntGuildTarget
    }
}