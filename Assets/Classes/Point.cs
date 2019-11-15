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
}