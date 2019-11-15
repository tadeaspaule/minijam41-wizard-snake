using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public SnakeController snakeController;
    public MapController mapController;
    
    public void GameOver()
    {
        snakeController.ResetSnake();
        mapController.ResetFood();
        mapController.UpdateMap();
        // TODO disable snake/map controller and show some UI on score etc
        Debug.Log("collided");
    }
}
