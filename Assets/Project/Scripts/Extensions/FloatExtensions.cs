public static class FloatExtensions
{
    public static float Normalize(this float value, float min, float max, float minRange = 0, float maxRange = 1)
    {
        if (min == max)
            throw new System.ArgumentException("Min and max cannot be the same value.");

        float normalizedValue = (float)(value - min) / (max - min);
        return normalizedValue * (maxRange - minRange) + minRange;
    }
}
