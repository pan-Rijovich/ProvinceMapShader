namespace UnityEngine
{
    public static class Color32Extension
    {
        public static bool LessThanColor(this Color32 a, Color32 b)
        {
            return Color32ToUInt(a) < Color32ToUInt(b);
        }

        public static uint Color32ToUInt(this Color32 color)
        {
            return (uint)(color.a << 24 | color.r << 16 | color.g << 8 | color.b);
        }
    }
}
