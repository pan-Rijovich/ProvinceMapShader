using System.Collections.Generic;
using UnityEngine;

public class Float2Comparer : IEqualityComparer<Float2>
{
    private float tolerance;

    public Float2Comparer(float tolerance)
    {
        this.tolerance = tolerance;
    }

    public bool Equals(Float2 a, Float2 b)
    {
        return Mathf.Abs(a.x - b.x) < tolerance && Mathf.Abs(a.y - b.y) < tolerance;
    }

    public int GetHashCode(Float2 obj)
    {
        return obj.x.GetHashCode() ^ obj.y.GetHashCode();
    }
}