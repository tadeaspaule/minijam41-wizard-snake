using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour
{
    public SnakeController snakeController;
    public MapController mapController;

    public TextMeshProUGUI highscoreText;
    public TextMeshProUGUI scoreText;

    int score = 0;
    int highscore = 0;
    
    void Start()
    {
        highscore = PlayerPrefs.GetInt("highscore",0);
        UpdateScore();
    }

    public void GameOver()
    {
        snakeController.ResetSnake();
        mapController.ResetFood();
        mapController.UpdateMap();
        // TODO disable snake/map controller and show some UI on score etc
        Debug.Log("collided");
        if (score > highscore) UpdateHighscore();
        score = 0;
        UpdateScore();
    }

    public void AteFood()
    {
        score++;
        UpdateScore();
    }
    
    void UpdateScore()
    {
        scoreText.text = score.ToString();
        highscoreText.text = highscore.ToString();
    }

    void UpdateHighscore()
    {
        PlayerPrefs.SetInt("highscore",score);
        highscore = score;
    }
}
