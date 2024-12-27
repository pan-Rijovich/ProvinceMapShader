using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubBorder : IEnumerable<BorderPoint>
{
    public readonly HashSet<BorderPoint> UnsortedPoints = new(8);
    public readonly LinkedList<BorderPoint> SortedPoints = new();
    public int Color1{ get; set; }
    public int Color2 { get; set; }
    public readonly int ClusterIndexForColor1;
    public readonly int ClusterIndexForColor2;
    public readonly int HashID;

    public SubBorder(int color1, int color2, int clusterIndexForColor1, int clusterIndexForColor2)
    {
        Color1 = color1;
        Color2 = color2;
        ClusterIndexForColor1 = clusterIndexForColor1;
        ClusterIndexForColor2 = clusterIndexForColor2;

        HashID = (Color1, Color2, ClusterIndexForColor1, ClusterIndexForColor2).GetHashCode();
    }

    public void AddPoint(BorderPoint point)
    {
        UnsortedPoints.Add(point);
    }

    public override bool Equals(object obj)
    {
        if (obj.GetType() == typeof(SubBorder) == false) return false;

        var other = (SubBorder)obj;
        return Color1.Equals(other.Color1) && Color2.Equals(other.Color2) && 
               ClusterIndexForColor1 == other.ClusterIndexForColor1 && 
               ClusterIndexForColor2 == other.ClusterIndexForColor2;
    }

    public override int GetHashCode()
    {
        return HashID;
    }

    public IEnumerator<BorderPoint> GetEnumerator() => UnsortedPoints.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}