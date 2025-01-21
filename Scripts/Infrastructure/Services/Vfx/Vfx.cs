using DG.Tweening;
using UnityEngine;
using UnityEngine.Animations;

namespace LevelElements.Vfx
{
    [RequireComponent(typeof(ParticleSystem))]
    public class Vfx : MonoBehaviour, IVfx
    {
        [SerializeField] private float secondsToDestroy = 2f; // Время жизни эффекта

        public bool ShouldUsePool => true;

        public float SecondsToDestroy => secondsToDestroy; // Публичное свойство для доступа к времени жизни

        private ParticleSystem _particleSystem;

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }
        public void Play()
        {
            gameObject.SetActive(true);
            _particleSystem.Play();
        }
        public void Dispose()
        {
            // Просто останавливаем эффект; он вернётся в пул автоматически по таймеру
            _particleSystem.Stop();
            gameObject.SetActive(false);
        }
    }
}