using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Border : IEnumerable<SubBorder>
{
    public readonly long ID;
    public readonly List<SubBorder> SubBorders;
    public int Color1 { get; set; }
    public int Color2 { get; set; }

    public Border(long id, int color1, int color2)
    {
        ID = id;
        Color1 = color1;
        Color2 = color2;
        SubBorders = new(1);
    }

    public SubBorder this[int index] {
        get => SubBorders[index];
        set => SubBorders[index] = value; 
    }

    public SubBorder AddSubBorder(int clusterIndexForColor1, int clusterIndexForColor2)
    {
        var subBorder = new SubBorder(Color1, Color2, clusterIndexForColor1, clusterIndexForColor2);
        SubBorders.Add(subBorder);
        return subBorder;
    }
    
    public bool TryGetSubBorder(int clusterIndexForColor1, int clusterIndexForColor2, out SubBorder result)
    {
        result = null;
        foreach (var subborder in SubBorders)
        {
            if (subborder.ClusterIndexForColor1 == clusterIndexForColor1 && subborder.ClusterIndexForColor2 == clusterIndexForColor2)
            {
                result = subborder;
                return true;
            }
        }
        return false;
    }

    public bool HasSubBorder(int clusterIndexForColor1, int clusterIndexForColor2)
    {
        foreach (var subborder in SubBorders)
        {
            if (subborder.ClusterIndexForColor1 == clusterIndexForColor1 && subborder.ClusterIndexForColor2 == clusterIndexForColor2)
            {
                return true;
            }
        }
        return false;
    }
    

    public IEnumerator<SubBorder> GetEnumerator() => SubBorders.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}