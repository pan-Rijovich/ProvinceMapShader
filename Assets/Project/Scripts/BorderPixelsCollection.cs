using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderPixelsCollection : IEnumerable<HashSet<int>>
{
    public readonly List<HashSet<int>> Pixels = new();
    public readonly int Color;
    
    public BorderPixelsCollection(int color)
    {
        Color = color;
    }

    public void AddCluster(HashSet<int> cluster)
    {
        Pixels.Add(cluster);
    }

    public bool HasPixelIndex(int index)
    {
        foreach (var cluster in Pixels)
        {
            if(cluster.Contains(index)) return true;
        }
        return false;
    }

    public bool TryGetClusterNumberForPixelIndex(int index, out int clusterNumber)
    {
        for (int i = 0; i < Pixels.Count; i++)
        {
            if (Pixels[i].Contains(index))
            {
                clusterNumber = i;
                return true;
            }
        }
        clusterNumber = -1;
        return false;
    }
    

    public IEnumerator<HashSet<int>> GetEnumerator() => Pixels.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}