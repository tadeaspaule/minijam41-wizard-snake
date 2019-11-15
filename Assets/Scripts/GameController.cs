using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public SnakeController snakeController;
    public MapController mapController;

    public Text highscoreText;
    public Text scoreText;
    public Text spellText;

    int score = 0;
    int highscore = 0;
    string holdingSpell = null;
    
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

    public void PickedUpSpell(string spell)
    {
        spellText.gameObject.SetActive(true);
        int cost = 5;
        spellText.text = $"{spell} ({cost})";
        holdingSpell = spell;
        if (score >= cost) spellText.color = Color.white;
        else spellText.color = new Color(1f,1f,1f,0f);
    }

    public void UsedSpell()
    {
        score -= 5;
        holdingSpell = null;
        spellText.gameObject.SetActive(false);
    }
    
    void UpdateScore()
    {
        scoreText.text = score.ToString();
        highscoreText.text = highscore.ToString();
        if (holdingSpell == null) return;
        // possibly makes spell available if enough score
        if (score >= 5) spellText.color = Color.white;
        else spellText.color = new Color(1f,1f,1f,0f);
    }

    void UpdateHighscore()
    {
        PlayerPrefs.SetInt("highscore",score);
        highscore = score;
    }
}
