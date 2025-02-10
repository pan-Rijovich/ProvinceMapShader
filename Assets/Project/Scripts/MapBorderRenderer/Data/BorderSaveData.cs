using System.Collections.Generic;

namespace MapBorderRenderer
{
    [System.Serializable]
    public struct BorderSaveData
    {
        public long ID;
        public SubBorderSaveData[] SubBorders;
    }
}