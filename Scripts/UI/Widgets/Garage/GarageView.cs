using Game.Scripts.Customization;
using Game.Scripts.LevelElements.Car;
using UnityEngine;

namespace Game.Scripts.UI.Widgets
{
    public class GarageView : MonoBehaviour
    {
        [field:SerializeField] public CarCustomizationController CarCustomizationController { get;private set; }
        
        [field:Header("Cameras")]
        [field:SerializeField] public GameObject[] GarageCameras { get;private set; }

        [field:Header("Fx")]
        [field:SerializeField] public ParticleSystem CarUpgrade { get;private set; }
        [field:SerializeField] public ParticleSystem StuntUpgrade { get;private set; }
        [field:SerializeField] public ParticleSystem IncomeUpgrade { get;private set; }
    }
}