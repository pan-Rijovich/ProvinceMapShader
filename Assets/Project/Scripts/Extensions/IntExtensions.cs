using UnityEngine;

public static class IntExtensions
{
    public static float Normalize(this int value, int min, int max, float minRange = 0, float maxRange = 1)
    {
        if (min == max)
            throw new System.ArgumentException("Min and max cannot be the same value.");

        float normalizedValue = (float)(value - min) / (max - min);
        return normalizedValue * (maxRange - minRange) + minRange;
    }

    public static int GetDigitNumber(this int digit, int number)
    {
        return (int)((digit % Mathf.Pow(10, number)) / Mathf.Pow(10, number - 1));
    }
    
    public static Color32 ToColor32(this int value)
    {
        byte r = (byte)((value >> 24) & 0xFF);
        byte g = (byte)((value >> 16) & 0xFF);
        byte b = (byte)((value >> 8) & 0xFF);
        byte a = (byte)(value & 0xFF);

        return new Color32(r, g, b, a);
    }
}