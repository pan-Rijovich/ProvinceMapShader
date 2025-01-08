namespace UnityEngine
{
    public static class Color32Extension
    {
        public static bool LessThanColor(this Color32 a, Color32 b)
        {
            return ToUInt(a) < ToUInt(b);
        }

        public static uint ToUInt(this Color32 color)
        {
            return (uint)(color.a << 24 | color.r << 16 | color.g << 8 | color.b);
        }
        
        public static int ToInt(this Color32 color)
        {
            return (color.r << 24) | (color.g << 16) | (color.b << 8) | color.a;
        }
    }
}
