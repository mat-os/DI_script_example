using System;
using Game.Scripts.LevelElements.Triggers;
using PG;
using UnityEngine;

namespace Game.Scripts.LevelElements.Car
{
    public class CarView : MonoBehaviour
    {
        [field:SerializeField] public Transform PlayerRoot { get;private set; }
        [field:SerializeField] public CarVfxView CarVfxView { get;private set; }
        [field:SerializeField] public Transform CarModel { get;private set; }
        [field:SerializeField] public Transform ExplosionFxPosition { get;private set; }
        [field:SerializeField]public Rigidbody Rigidbody { get;private set; }
        [field:SerializeField]public CarCollideTrigger WallHitTrigger  { get;private set; }
        [field: SerializeField] public WheelView[] Wheels  { get;private set; }
        [field: SerializeField] public VehicleDamageController VehicleDamageController { get;private set; }
        [field: SerializeField] public CarCustomizationController CarCustomizationController { get;private set; }
    }
}