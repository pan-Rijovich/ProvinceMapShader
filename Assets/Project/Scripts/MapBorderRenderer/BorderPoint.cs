using UnityEngine;

public struct BorderPoint
{
    public int X;
    public int Y;
    //public float UVX;
    //public float UVY;
    public int DebugColor;

    public BorderPoint(int index, int width, int height)
    {
        Y = (index / width);
        X = (index % width);
        Y = (Y + 1) * 2;
        X = (X + 1) * 2;
        DebugColor = 0;
        //UVY = (index / width) / width;
        //UVX = (index % width) / height;
    }

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
