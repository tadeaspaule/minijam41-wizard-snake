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

    public Sprite groundSprite;
    public Sprite snakeSprite;

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
    }

    public void UpdateMap()
    {
        foreach (MapCell mc in tiles) mc.UpdateImage(groundSprite);
        foreach (Point p in snakeController.snake.body) {
            tiles[p.x,p.y].UpdateImage(snakeSprite);
        }
        tiles[snakeController.snake.head.x,snakeController.snake.head.y].UpdateImage(snakeSprite);

        
        tiles[food.x,food.y].UpdateImage(snakeSprite);
    }
}
