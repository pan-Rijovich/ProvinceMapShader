﻿public struct Int2
{
    public int x;
    public int y;
    public int debugColor;
    public int positionIndex { get; set; }

    public Int2(int x, int y)
    {
        this.x = x;
        this.y = y;
        debugColor = 0;
        positionIndex = 0;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is Int2)) return false;
        Int2 other = (Int2)obj;
        return this.x == other.x && this.y == other.y;
    }

    public override int GetHashCode()
    {
        return (x, y).GetHashCode();
    }

    public override string ToString()
    {
        return $"({x},{y})";
    }
}