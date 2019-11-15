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
}