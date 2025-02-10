using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubBorderCreationData : IEnumerable<BorderPoint>
{
    public HashSet<BorderPoint> UnsortedPoints = new(8);
    public List<LinkedList<BorderPoint>> SortedPointsLists = new();
    public List<LineRenderer> Lines { get; set; } = new();
    public bool IsCycled;
    public readonly int Color1;
    public readonly int Color2;
    public readonly int ClusterIndexForColor1;
    public readonly int ClusterIndexForColor2;
    public readonly int HashID;

    public SubBorderCreationData(int color1, int color2, int clusterIndexForColor1, int clusterIndexForColor2)
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
        if (obj.GetType() == typeof(SubBorderCreationData) == false) return false;

        var other = (SubBorderCreationData)obj;
        
        //return HashID == other.HashID;
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