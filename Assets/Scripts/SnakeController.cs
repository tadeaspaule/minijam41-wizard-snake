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

    #region Movement vars

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
    const int UP = 0;
    const int DOWN = 2;
    const int LEFT = 1;
    const int RIGHT = 3;
    int[] directionCodes = new int[]{
        UP,DOWN,LEFT,RIGHT
    };
    int directionCode = UP;

    List<int> moves = new List<int>();

    public Sprite snakeStraight;
    public Sprite snakeCorner;

    #endregion
    
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
        moves.Clear();
        direction = directions[0];
        directionCode = directionCodes[0];
        moves.Add(directionCode);
    }

    // Update is called once per frame
    void Update()
    {
        bool found = false;
        for (int i = 0; i < 4; i++) {
            foreach (KeyCode kc in keys[i]) {
                if (Input.GetKeyDown(kc) && IsValidDirection(directions[i])) {
                    direction = directions[i];
                    directionCode = directionCodes[i];
                    found = true;
                    break;
                }
            }
            if (found) break;
        }
        timer += Time.deltaTime;
        if (timer >= moveTimer) {
            moves.Insert(0,directionCode);
            // move
            if (mapController.food != null && mapController.food.Equals(snake.head.x+direction.x,snake.head.y+direction.y)) {
                // ate food
                snake.Move(direction.x,direction.y,false);
                mapController.ResetFood();
                gameController.AteFood();
            }
            else {
                snake.Move(direction.x,direction.y);
                moves.RemoveAt(moves.Count-1);
            }

            // check if collided with anything
            if (snake.CollidingWithBody(snake.head) || snake.head.x < 0 || snake.head.y < 0
            || snake.head.x >= MapController.SIZE || snake.head.y >= MapController.SIZE) {
                gameController.GameOver();
                return;
            }

            timer = 0f;
            mapController.UpdateMap();
            mapController.SetTile(snake.head.x,snake.head.y,snakeStraight,0,0);
            // update snake body sprites
            if (snake.body.Count > 1 && moves.Count > 2) {
                SetSnakeBody(moves[0],moves[1],snake.body[0]);
                SetSnakeBody(moves[1],moves[2],snake.body[1]);
            }
            else if (snake.body.Count > 0) {
                mapController.SetTile(snake.body[0],snakeStraight,0,directionCode*90);
            }

        }
    }

    bool IsValidDirection(Vector2Int dir)
    {
        return dir.x != direction.x && dir.y != direction.y;
    }

    void SetSnakeBody(int move, int prevMove, Point point)
    {
        Sprite sprite = move == prevMove ? snakeStraight : snakeCorner;
        int yDeg = 0;
        int zDeg = 0;
        if (move == UP && prevMove == UP) {
            yDeg = 0;
            zDeg = 0;
        }
        else if (move == DOWN && prevMove == DOWN) {
            yDeg = 0;
            zDeg = 180;
        }
        else if (move == LEFT && prevMove == LEFT) {
            yDeg = 0;
            zDeg = 90;
        }
        else if (move == RIGHT && prevMove == RIGHT) {
            yDeg = 0;
            zDeg = 270;
        }
        else if (move == RIGHT && prevMove == UP) {
            yDeg = 0;
            zDeg = 0;
        }
        else if (move == LEFT && prevMove == UP) {
            yDeg = 180;
            zDeg = 0;
        }
        else if (move == LEFT && prevMove == DOWN) {
            yDeg = 0;
            zDeg = 180;
        }
        else if (move == RIGHT && prevMove == DOWN) {
            yDeg = 180;
            zDeg = 180;
        }
        else if (move == UP && prevMove == LEFT) {
            yDeg = 0;
            zDeg = 90;
        }
        else if (move == UP && prevMove == RIGHT) {
            yDeg = 180;
            zDeg = 90;
        }
        else if (move == DOWN && prevMove == RIGHT) {
            yDeg = 0;
            zDeg = 270;
        }
        else if (move == DOWN && prevMove == LEFT) {
            yDeg = 180;
            zDeg = 270;
        }
        mapController.SetTile(point.x,point.y,sprite,yDeg,zDeg);
    }
}
