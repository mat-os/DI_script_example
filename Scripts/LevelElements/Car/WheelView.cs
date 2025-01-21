using UnityEngine;

namespace Game.Scripts.LevelElements.Car
{
    public class WheelView : MonoBehaviour
    {
        [field:SerializeField] public Transform WheelRoot { get;private set; }
        [field:SerializeField] public SphereCollider SphereCollider { get;private set; }
        [field:SerializeField] public bool IsSteering  { get;private set; }
        public float Radius => SphereCollider.radius;
    }
}