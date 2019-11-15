using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeController : MonoBehaviour
{
    public MapController mapController;
    public GameController gameController;
    
    public Snake snake;
    Vector2Int direction = Vector2Int.up;

    float timer = 0f;
    float moveTimer = 0.2f;

    KeyCode[][] keys = new KeyCode[][]{
        new KeyCode[]{KeyCode.UpArrow,KeyCode.W},
        new KeyCode[]{KeyCode.DownArrow,KeyCode.S},
        new KeyCode[]{KeyCode.LeftArrow,KeyCode.A},
        new KeyCode[]{KeyCode.RightArrow,KeyCode.D}
    };
    Vector2Int[] directions = new Vector2Int[]{
        Vector2Int.down,
        Vector2Int.up,
        Vector2Int.left,
        Vector2Int.right
    };
    
    // Start is called before the first frame update
    void Start()
    {
        ResetSnake();
        mapController.UpdateMap();
    }

    public void ResetSnake()
    {
        int n = 3;
        snake = new Snake(Random.Range(n,MapController.SIZE-n),Random.Range(n,MapController.SIZE-n));
    }

    // Update is called once per frame
    void Update()
    {
        bool found = false;
        for (int i = 0; i < 4; i++) {
            foreach (KeyCode kc in keys[i]) {
                if (Input.GetKeyDown(kc) && IsValidDirection(directions[i])) {
                    direction = directions[i];
                    found = true;
                    break;
                }
            }
            if (found) break;
        }
        timer += Time.deltaTime;
        if (timer >= moveTimer) {
            // move
            if (mapController.food.Equals(snake.head.x+direction.x,snake.head.y+direction.y)) {
                // ate food
                snake.Move(direction.x,direction.y,false);
                mapController.ResetFood();
            }
            else {
                snake.Move(direction.x,direction.y);
            }

            // check if collided with anything
            if (snake.CollidingWithBody(snake.head) || snake.head.x < 0 || snake.head.y < 0
            || snake.head.x >= MapController.SIZE || snake.head.y >= MapController.SIZE) {
                gameController.GameOver();
                return;
            }

            timer = 0f;
            mapController.UpdateMap();

        }
    }

    bool IsValidDirection(Vector2Int dir)
    {
        return dir.x != direction.x && dir.y != direction.y;
    }
}
