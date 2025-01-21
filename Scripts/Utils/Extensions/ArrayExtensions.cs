using UnityEngine;

namespace Game.Scripts.Utils.Extensions
{
    public static class ArrayExtensions
    {
        public static T GetRandomElement<T>(this T[] array)
        {
            int index = Random.Range(0, array.Length);
            return array[index];
        }
    }
}