﻿using System.Collections;
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
    public Spell holdingSpell = null;

    Color full = Color.white;
    Color half = new Color(1f,1f,1f,0.5f);
    Color transparent = new Color(1f,1f,1f,0f);

    public AnimationClip intro;

    #region Sounds

    public AudioSource audioSource;

    public AudioClip wallDestroy;

    public AudioClip blink;
    public AudioClip shrink;
    public AudioClip rage;
    public AudioClip ghost;

    public AudioClip timeSpeedup;
    public AudioClip timeSlowdown;
    public AudioClip controlReverse;

    #endregion
    
    void Start()
    {
        highscore = PlayerPrefs.GetInt("highscore",0);
        UpdateScore();
        UpdateSpellText();
        StartCoroutine(AfterIntroFinishes());
    }

    IEnumerator AfterIntroFinishes()
    {
        yield return new WaitForSeconds(intro.averageDuration);
        mapController.disabled = false;
        StartCoroutine(mapController.DelayedSpellSpawn(1f));
        snakeController.gameObject.SetActive(true);
    }

    public void GameOver()
    {
        snakeController.ResetSnake();
        mapController.ResetMap();
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

    public void DestroyedWall()
    {
        audioSource.PlayOneShot(wallDestroy);
        score++;
        UpdateScore();
    }

    public void SteppedOnTrap(string trap)
    {
        if (trap.Equals("speedup")) audioSource.PlayOneShot(timeSpeedup);
        else if (trap.Equals("control-switch")) audioSource.PlayOneShot(controlReverse);
    }

    public void PlayTimeSlowdown()
    {
        audioSource.PlayOneShot(timeSlowdown);
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

        if (holdingSpell.name.Equals("blink")) {
            audioSource.PlayOneShot(blink);
        }
        else if (holdingSpell.name.Equals("shrink")) {
            audioSource.PlayOneShot(shrink);
        }
        else if (holdingSpell.name.Equals("ghost")) {
            audioSource.PlayOneShot(ghost);
        }
        else if (holdingSpell.name.Equals("rage")) {
            audioSource.PlayOneShot(rage);
        }

        holdingSpell = null;
        UpdateSpellText();
        UpdateScore();

    }

    public bool CanUseSpell()
    {
        if (holdingSpell == null) return false;
        return score >= holdingSpell.cost;
    }

    public string HoldingSpellName()
    {
        return holdingSpell == null ? "" : holdingSpell.name;
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
