using UnityEngine;
using System.Collections.Generic;

namespace OtherUtils
{
    public static class ListUtils
    {
        /// <summary>
        /// Shuffles the list in place.
        /// </summary>
        /// <typeparam name="T">The object type in the list</typeparam>
        /// <param name="list">The list you want to shuffle.</param>
        public static void Shuffle<T>(this List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int randomIndex = Random.Range(i, list.Count);
                T temp = list[i];
                list[i] = list[randomIndex];
                list[randomIndex] = temp;
            }
        }
    }
}
