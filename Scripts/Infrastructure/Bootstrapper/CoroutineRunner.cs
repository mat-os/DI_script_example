using UnityEngine;

namespace Game.Scripts.Infrastructure.Bootstrapper
{
    public class CoroutineRunner : MonoBehaviour, ICoroutineRunnerService
    {
        public static CoroutineRunner Instance;
        
        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        
        private void OnDisable() => StopAllCoroutines();
    }
}