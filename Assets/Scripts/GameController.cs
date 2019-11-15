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
    Spell holdingSpell = null;

    Color full = Color.white;
    Color half = new Color(1f,1f,1f,0.5f);
    Color transparent = new Color(1f,1f,1f,0f);
    
    void Start()
    {
        highscore = PlayerPrefs.GetInt("highscore",0);
        UpdateScore();
        UpdateSpellText();
        StartCoroutine(mapController.DelayedSpellSpawn(1f));
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
        holdingSpell = null;
        UpdateSpellText();
    }

    public void AteFood()
    {
        score++;
        UpdateScore();
    }

    public void PickedUpSpell(Spell spell)
    {
        holdingSpell = spell;
        UpdateSpellText();
        StartCoroutine(mapController.DelayedSpellSpawn(5f));
    }

    public void UsedSpell()
    {
        score -= holdingSpell.cost;
        holdingSpell = null;
        UpdateSpellText();
    }

    void UpdateSpellText()
    {
        if (holdingSpell == null) {
            spellText.color = transparent;
            return;
        }
        if (score >= holdingSpell.cost) {
            spellText.color = Color.white;
            spellText.text = $"{holdingSpell.name} ({holdingSpell.cost})";
        }
        else {
            spellText.color = half;
            spellText.text = $"{holdingSpell.name} ({holdingSpell.cost})";
        }
    }
    
    void UpdateScore()
    {
        scoreText.text = score.ToString();
        highscoreText.text = highscore.ToString();
        if (holdingSpell == null) return;
        // possibly makes spell available if enough score
        UpdateSpellText();
    }

    void UpdateHighscore()
    {
        PlayerPrefs.SetInt("highscore",score);
        highscore = score;
    }
}
