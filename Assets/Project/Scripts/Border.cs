using System.Collections;
using System.Collections.Generic;

public class Border
{
    public long ID;
    public List<List<BorderPoint>> Points;

    public Border(long iD)
    {
        ID = iD;
        Points = new(2) { new(16) };
    }

    public List<BorderPoint> this[int index] {
        get => Points[index];
        set => Points[index] = value; 
    }
    
}