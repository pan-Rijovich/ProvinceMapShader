using UnityEngine;

public struct BorderPoint
{
    public int X;
    public int Y;
    public int FromPixelIndex;
    public int ToPixelIndex;
    //public float UVX;
    //public float UVY;
    public bool IsEdgePoint;
    public int DebugColor;


    public override bool Equals(object obj)
    {
        if (!(obj.GetType() == typeof(BorderPoint))) return false;

        BorderPoint other = (BorderPoint)obj;
        return X == other.X && Y == other.Y;
    }

    public override int GetHashCode()
    {
        return (X,Y).GetHashCode();
    }

    public override string ToString()
    {
        var msg = "";
        msg += $"Pos: ({X},{Y}), ";
        return msg;
    }



    public static bool operator ==(BorderPoint point, BorderPoint other)
    {
        return point.X == other.X && point.Y == other.Y;         
    }

    public static bool operator !=(BorderPoint point, BorderPoint other)
    {
        return point.X != other.X || point.Y != other.Y;
    }
    
    public static BorderPoint operator +(BorderPoint point, Vector2Int adder)
    {
        return new()
        {
            X = point.X + adder.x, 
            Y = point.Y + adder.y,
            DebugColor = point.DebugColor
        };
    }
}
