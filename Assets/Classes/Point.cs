public class Point
{
    public int x;
    public int y;

    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public bool Equals(int x, int y)
    {
        return this.x == x && this.y == y;
    }

    public bool Equals(Point p)
    {
        if (p == null) return false;
        return this.x == p.x && this.y == p.y;
    }

    public void ResetToWithinBounds(int bound)
    {
        if (x < 0) x += bound;
        if (y < 0) y += bound;
        if (x >= bound) x -= bound;
        if (y >= bound) y -= bound;
    }

    public bool IsWithinBounds(int bound)
    {
        return x >= 0 && x < bound && y >= 0 && y < bound;
    }
}