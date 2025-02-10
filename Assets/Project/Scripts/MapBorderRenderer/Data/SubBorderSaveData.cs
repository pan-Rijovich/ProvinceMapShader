using System.Collections.Generic;

namespace MapBorderRenderer
{
    [System.Serializable]
    public struct SubBorderSaveData
    {
        public int[][] SortedPointsLists;        
        public bool IsCycled;
        public int ClusterIndexForColor1;
        public int ClusterIndexForColor2;
    }
}