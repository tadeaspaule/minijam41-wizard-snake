using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeController : MonoBehaviour
{
    public MapController mapController;
    
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
        snake = new Snake(Random.Range(2,6),Random.Range(2,6));
        mapController.UpdateMap();
    }

    // Update is called once per frame
    void Update()
    {
        bool found = false;
        for (int i = 0; i < 4; i++) {
            foreach (KeyCode kc in keys[i]) {
                if (Input.GetKeyDown(kc)) {
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
            snake.Move(direction.x,direction.y);
            timer = 0f;
            mapController.UpdateMap();
        }
    }
}
