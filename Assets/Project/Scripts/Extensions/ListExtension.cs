using UnityEngine;

namespace System.Collections.Generic
{
    public static class ListExtension
    {
        public static List<List<T>> SegmentList<T>(this List<T> sourceList, int segmentSize)
        {
            var segmentedList = new List<List<T>>();

            for (int i = 0; i < sourceList.Count; i += segmentSize)
            {
                var segment = sourceList.GetRange(i, Mathf.Min(segmentSize, sourceList.Count - i));
                segmentedList.Add(segment);
            }

            return segmentedList;
        }
    }
}