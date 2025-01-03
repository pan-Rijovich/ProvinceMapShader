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
}