using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapController : MonoBehaviour
{
    public SnakeController snakeController;
    public RectTransform gridMap;
    public static int SIZE = 16;
    MapCell[,] tiles = new MapCell[SIZE,SIZE];

    public Point food = null;
    float timer = 0f;
    float foodTimer = 1f;
    
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < SIZE*SIZE; i++) {
            tiles[i%SIZE,i/SIZE] = gridMap.GetChild(i).GetComponent<MapCell>();
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= foodTimer) {
            timer = 0f;
            if (food != null) return;
            ResetFood();
        }
    }

    public void ResetFood()
    {
        List<Point> options = new List<Point>();
        for (int y = 0; y < SIZE; y++) {
            for (int x = 0; x < SIZE; x++) {
                if (!snakeController.snake.InBody(x,y)) options.Add(new Point(x,y));
            }
        }
        food = options[Random.Range(0,options.Count)];
        tiles[food.x,food.y].SetAsFood();
    }

    public void UpdateMap()
    {
        for (int y = 0; y < SIZE; y++) {
            for (int x = 0; x < SIZE; x++) {
                if (!snakeController.snake.InBody(x,y) && (food == null || !food.Equals(x,y))) tiles[x,y].HideTile();
            }
        }
    }

    public void SetTile(int x, int y, Sprite sprite, int yRotation, int zRotation)
    {
        tiles[x,y].UpdateImage(sprite);
        tiles[x,y].RotateImage(yRotation,zRotation);
    }

    public void SetTile(Point p, Sprite sprite, int yRotation, int zRotation)
    {
        SetTile(p.x,p.y,sprite,yRotation,zRotation);
    }
}
