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
        //UVY = (index / width) / width;
        //UVX = (index % width) / height;

        DebugColor = 0;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is BorderPoint)) return false;

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
        //msg += $"UV: ({UVX},{UVY}), ";
        msg += $"Color: {DebugColor}, ";
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
}
