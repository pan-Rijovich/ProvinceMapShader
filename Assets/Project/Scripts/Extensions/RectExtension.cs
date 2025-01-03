namespace UnityEngine
{
    public static class RectExtension
    {
        public static bool IsInsideOtherRect(this Rect r, Rect otherRect)
        {
            return r.xMin >= otherRect.xMin 
                && r.xMax <= otherRect.xMax 
                && r.yMin >= otherRect.yMin 
                && r.yMax <= otherRect.yMax;
        }

        public static bool CanBeInsideOtherRect(this Rect r, Rect otherRect)
        {
            return r.width <= otherRect.width 
                && r.height <= otherRect.height;
        }

        public static bool TryGetOffsetToEnterOtherRect(this Rect rect, Rect otherRect, out Vector3 offset)
        {
            offset = Vector3.zero;
            if (rect.CanBeInsideOtherRect(otherRect))
            {
                if (rect.xMin < otherRect.xMin)
                {
                    offset.x = otherRect.xMin - rect.xMin;
                }
                else if (rect.xMax > otherRect.xMax)
                {
                    offset.x = otherRect.xMax - rect.xMax;
                }

                if (rect.yMin < otherRect.yMin)
                {
                    offset.y = otherRect.yMin - rect.yMin;
                }
                else if (rect.yMax > otherRect.yMax)
                {
                    offset.y = otherRect.yMax - rect.yMax;
                }
                return true;
            }
            return false;
        }
    }
}
