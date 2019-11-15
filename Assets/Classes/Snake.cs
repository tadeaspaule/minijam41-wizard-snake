using System.Collections.Generic;

public class Snake
{
    public List<Point> body;
    public Point head;

    public Snake(int x, int y)
    {
        body = new List<Point>();
        head = new Point(x,y);
    }

    public void Move(int xMove, int yMove, bool removeLast)
    {
        body.Insert(0,head);
        if (removeLast) body.RemoveAt(body.Count-1);
        head = new Point(head.x+xMove,head.y+yMove);
    }

    public void Move(int xMove, int yMove)
    {
        Move(xMove,yMove,true);
    }

    public bool InBody(int x, int y)
    {
        if (head.Equals(x,y)) return true;
        foreach (Point p in body) if (p.Equals(x,y)) return true;
        return false;
    }
}