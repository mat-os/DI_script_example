using UnityEngine;

namespace Game.Scripts.Utils
{
    public class LookAtCamera : MonoBehaviour
    {
        private Camera _camera;

        void Start() => _camera = Camera.main;

        void Update() => transform.forward = _camera.transform.forward;

    }
}