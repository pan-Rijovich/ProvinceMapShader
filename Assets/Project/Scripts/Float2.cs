public struct Float2
{
    public float x;
    public float y;
    public int debugColor;
    public int positionIndex { get; set; }

    public Float2(float x, float y)
    {
        this.x = x;
        this.y = y;
        debugColor = 0;
        positionIndex = 0;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is Float2)) return false;

        Float2 other = (Float2)obj;
        return this.x == other.x && this.y == other.y;
    }

    // Переопределение метода GetHashCode для использования в коллекциях
    public override int GetHashCode()
    {
        return (x, y).GetHashCode(); // Используем кортеж для вычисления хэш-кода
    }

    public override string ToString()
    {
        return $"({x},{y})";
    }
}
