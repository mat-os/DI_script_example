using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Utils.Extensions
{
    public static class ListExtensions
    {
        public static T GetRandomElementWithRemove<T>(this List<T> list)
        {
            T element = GetRandomElement(list);
            list.Remove(element);

            return element;
        }

        public static T GetRandomElement<T>(this List<T> list)
        {
            int index = Random.Range(0, list.Count);
            return list[index];
        }
    }
}