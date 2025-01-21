using System.Collections;
using Game.Scripts.Infrastructure.Services;
using UnityEngine;

namespace Game.Scripts.Infrastructure.Bootstrapper
{
    public interface ICoroutineRunnerService 
    {
        Coroutine StartCoroutine(IEnumerator routine);
        void StopCoroutine(Coroutine routine);
        void StopAllCoroutines();
    }
}