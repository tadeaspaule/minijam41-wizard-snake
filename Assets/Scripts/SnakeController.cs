using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeController : MonoBehaviour
{
    public MapController mapController;
    public GameController gameController;
    
    public Snake snake;
    Vector2Int direction = Vector2Int.up;
    Vector2Int lastDirection = Vector2Int.up;

    float timer = 0f;
    float moveTimer = 0.2f;
    float moveTimerExtra = 0f;

    Color snakeColor = Color.white;

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
    public Sprite snakeHead;

    #endregion
    
    #region Spells

    public int activeEffect = -1;
    public static int GHOST = 0;

    const int BLINK_DIST = 5;
    Color BLINK_COL = new Color(0.8f,0.8f,1f,0.5f);

    const int SHRINK_COUNT = 3;

    #endregion
    
    #region Traps

    string[] traps = new string[]{"speedup"};
    const float SPEEDUP_DURATION = 4f;

    #endregion
    
    // Start is called before the first frame update
    void Start()
    {
        ResetSnake();
        mapController.UpdateMap();
    }

    public void ResetSnake()
    {
        snake = new Snake(7,12);
        snake.body.Add(new Point(7,13));
        snake.body.Add(new Point(7,14));
        moves.Clear();
        direction = directions[0];
        directionCode = directionCodes[0];
        moves.Add(directionCode);
        moves.Add(directionCode);
        moves.Add(directionCode);

        // extra param resets
        activeEffect = -1;
        moveTimerExtra = 0f;
    }

    void HandleSpellUse()
    {
        string spell = gameController.holdingSpell.name;
        if (spell.Equals("ghost")) {
            activeEffect = GHOST;
            snakeColor = new Color(1f,1f,1f,0.5f);
            UpdateSnakeColor();
        }
        else if (spell.Equals("blink")) {
            snake.head.x += lastDirection.x*BLINK_DIST;
            snake.head.y += lastDirection.y*BLINK_DIST;
            snake.head.ResetToWithinBounds(MapController.SIZE);
            mapController.SetTile(snake.head,snakeHead,snakeColor,0,directionCode*90);
            mapController.UpdateMap();
        }
        else if (spell.Equals("shrink")) {
            int n = snake.body.Count;
            for (int i = 0; i < SHRINK_COUNT && i < n; i++) {
                snake.body.RemoveAt(snake.body.Count-1);
            }
            mapController.UpdateMap();
        }
    }

    void HandleTrap(string trap)
    {
        if (trap.Equals("speedup")) {
            moveTimerExtra = -0.1f;
            StartCoroutine(ResetMoveTimer(SPEEDUP_DURATION));
        }
    }

    IEnumerator ResetMoveTimer(float delay)
    {
        yield return new WaitForSeconds(delay);
        moveTimerExtra = 0f;
    }

    void UpdateSnakeColor()
    {
        mapController.SetTileColor(snake.head,snakeColor);
        foreach (Point p in snake.body) mapController.SetTileColor(p,snakeColor);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && gameController.CanUseSpell()) {
            HandleSpellUse();
            gameController.UsedSpell();
        }

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
        if (timer >= moveTimer + moveTimerExtra) {
            lastDirection = direction;
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
            if (snake.head.x < 0 || snake.head.y < 0
            || snake.head.x >= MapController.SIZE || snake.head.y >= MapController.SIZE
            || mapController.GetTile(snake.head).isCollider) {
                if (activeEffect == GHOST) {
                    snake.head.ResetToWithinBounds(MapController.SIZE);
                    activeEffect = -1; // used up the effect
                    snakeColor = Color.white;
                    UpdateSnakeColor();
                }
                else {
                    gameController.GameOver();
                    return;
                }
            }

            if (snake.CollidingWithBody(snake.head)) {
                gameController.GameOver();
                return;
            }

            // check if stepped on trap
            if (mapController.GetTile(snake.head).isTrap) {
                Debug.Log("stepped on trap");
                mapController.RemoveTrap(snake.head);
                HandleTrap(traps[Random.Range(0,traps.Length)]);
            }

            // check if picked up spell
            if (snake.head.Equals(mapController.spell)) {
                gameController.PickedUpSpell(mapController.placedSpell);
            }

            timer = 0f;
            mapController.UpdateMap();
            mapController.SetTile(snake.head,snakeHead,snakeColor,0,directionCode*90);
            
            // if holding blink, draw the "destination"
            if (gameController.HoldingSpellName().Equals("blink")) {
                Point p = new Point(snake.head.x+direction.x*BLINK_DIST,snake.head.y+direction.y*BLINK_DIST);
                p.ResetToWithinBounds(MapController.SIZE);
                mapController.SetTile(p,snakeHead,BLINK_COL,0,directionCode*90);
            }
            
            // update snake body sprites
            if (snake.body.Count > 1 && moves.Count > 2) {
                SetSnakeBody(moves[0],moves[1],snake.body[0]);
                SetSnakeBody(moves[1],moves[2],snake.body[1]);
            }
            else if (snake.body.Count > 0) {
                mapController.SetTile(snake.body[0],snakeStraight,snakeColor,0,directionCode*90);
            }

        }
    }

    bool IsValidDirection(Vector2Int dir)
    {
        return dir.x != lastDirection.x && dir.y != lastDirection.y;
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
        mapController.SetTile(point,sprite,snakeColor,yDeg,zDeg);
    }
}
