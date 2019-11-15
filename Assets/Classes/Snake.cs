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

    public void Move(int xMove, int yMove)
    {
        body.Insert(0,head);
        body.RemoveAt(body.Count-1);
        head = new Point(head.x+xMove,head.y+yMove);
    }
}