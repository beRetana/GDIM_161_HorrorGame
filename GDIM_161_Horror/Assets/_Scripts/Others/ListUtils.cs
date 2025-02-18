using System.Collections.Generic;

namespace OtherUtils
{
    public static class ListUtils
    {
        /// <summary>
        /// Shuffles the given list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Shuffle<T>(List<T> list)
        {
            int count = list.Count;

            for (int i = 0; i < count-1; i++)
            {
                int randomIndex = UnityEngine.Random.Range(0, list.Count);

                var temp = list[i];
                list[i] = list[randomIndex];
                list[randomIndex] = temp;
            }
        }
    }
}
