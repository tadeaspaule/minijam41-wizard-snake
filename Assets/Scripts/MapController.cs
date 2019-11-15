using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapController : MonoBehaviour
{
    public SnakeController snakeController;
    public RectTransform gridMap;
    MapCell[,] tiles = new MapCell[16,16];

    public Sprite groundSprite;
    public Sprite snakeSprite;
    
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 16*16; i++) {
            tiles[i%16,i/16] = gridMap.GetChild(i).GetComponent<MapCell>();
        }
    }

    public void UpdateMap()
    {
        foreach (MapCell mc in tiles) mc.UpdateImage(groundSprite);
        foreach (Point p in snakeController.snake.body) {
            tiles[p.x,p.y].UpdateImage(snakeSprite);
        }
        tiles[snakeController.snake.head.x,snakeController.snake.head.y].UpdateImage(snakeSprite);
    }
}
