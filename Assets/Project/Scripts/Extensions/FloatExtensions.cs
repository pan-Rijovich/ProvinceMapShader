using UnityEngine;

public static class FloatExtensions
{
    public static float Normalize(this float value, float min, float max, float minRange = 0, float maxRange = 1)
    {
        if (min == max)
            throw new System.ArgumentException("Min and max cannot be the same value.");

        float normalizedValue = (float)(value - min) / (max - min);
        return normalizedValue * (maxRange - minRange) + minRange;
    }
    
    public static float Clamp(this float value, float min, float max)
    {
        return Mathf.Clamp(value, min, max);
    }
    
    public static float Clamp01(this float value)
    {
        return Mathf.Clamp01(value);
    }
}
