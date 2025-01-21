using System;
using UnityEngine;

namespace Game.Scripts.LevelElements.Car
{
    public class CarVfxView : MonoBehaviour
    {
        [field:SerializeField] public ParticleSystem BoostPs { get;private set; }

        [field:Header("Trail")] 
        [field:SerializeField] public Transform ParentForEffects{ get;private set; }
        [field:SerializeField] public TrailRenderer TrailRef { get;private set; }

        [field:Header("Car engine")]
        [field:SerializeField] public ParticleSystem _engineHealth75Particles;
        [field:SerializeField] public ParticleSystem _engineHealth50Particles;
        [field:SerializeField] public ParticleSystem _engineHealth25Particles;

        private void Awake()
        {
            TrailRef.gameObject.SetActive (false);
        }
    }
}