﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapController : MonoBehaviour
{
    public SnakeController snakeController;
    public RectTransform gridMap;
    public static int SIZE = 16;
    MapCell[,] tiles = new MapCell[SIZE,SIZE];

    public Point food = null;
    float timer = 0f;
    float foodTimer = 1f;

    public Point spell = null;
    public Spell placedSpell = null;
    float spellTimer = 0f;
    float spellTimerCap = 5f;
    public Sprite spellSprite;

    List<Point> tempWalls = new List<Point>();
    List<Point> permaWalls = new List<Point>();
    List<Point> wallWarnings = new List<Point>();
    float tempWallTimer = 0f;
    float tempWallTimerCap = 7f;
    float permaWallTimer = 0f;
    float permaWallTimerCap = 12f;
    float wallWarningTime = 2f;

    List<Point> traps = new List<Point>();
    List<Point> trapWarnings = new List<Point>();
    float trapTimer = 0f;
    float trapTimerCap = 5f;
    float trapWarningTime = 2f;
    float trapDisappearDelay = 12f;

    public bool disabled = true;
    
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < SIZE*SIZE; i++) {
            tiles[i%SIZE,i/SIZE] = gridMap.GetChild(i).GetComponent<MapCell>();
        }
    }

    void Update()
    {
        if (disabled) return;
        timer += Time.deltaTime;
        if (timer >= foodTimer) {
            timer = 0f;
            if (food != null) return;
            ResetFood();
        }

        tempWallTimer += Time.deltaTime;
        if (tempWallTimer >= tempWallTimerCap) {
            tempWallTimer = 0f;
            PlanWallPlace(true);
        }

        permaWallTimer += Time.deltaTime;
        if (permaWallTimer >= permaWallTimerCap) {
            permaWallTimer = 0f;
            PlanWallPlace(false);
        }

        trapTimer += Time.deltaTime;
        if (trapTimer >= trapTimerCap) {
            trapTimer = 0f;
            TrapSetup();
        }

        spellTimer += Time.deltaTime;
        if (spellTimer >= spellTimerCap) {
            if (spell != null) tiles[spell.x,spell.y].HideTile();
            spell = null;
            placedSpell = null;
            StartCoroutine(DelayedSpellSpawn(2f));
        }
    }

    Point GetUnoccupiedSpot()
    {
        List<Point> options = new List<Point>();
        for (int y = 0; y < SIZE; y++) {
            for (int x = 0; x < SIZE; x++) {
                if (IsTileEmpty(x,y)) options.Add(new Point(x,y));
            }
        }
        if (options.Count == 0) return null;
        return options[Random.Range(0,options.Count)];
    }

    bool IsTileEmpty(int x, int y)
    {
        return (!snakeController.snake.InBody(x,y)
                && !tiles[x,y].isCollider
                && !tiles[x,y].isWarning
                && !tiles[x,y].isTrap
                && (food == null || !food.Equals(x,y))
                && (spell == null || !spell.Equals(x,y)));
    }

    public void ResetMap()
    {
        disabled = false;
        ResetFood();
        wallWarnings.Clear();
        tempWalls.Clear();
        permaWalls.Clear();
        traps.Clear();
        trapWarnings.Clear();
        spell = null;
        spellTimer = 0f;
        tempWallTimer = 0f;
        permaWallTimer = 0f;
        trapTimer = 0f;
    }

    public void ResetFood()
    {
        food = GetUnoccupiedSpot();
        tiles[food.x,food.y].SetAsFood();
    }

    public IEnumerator DelayedSpellSpawn(float delay)
    {
        if (spell != null) {
            // have to reset spell vars
            tiles[spell.x,spell.y].HideTile();
            
        }
        spell = null;
        placedSpell = null;
        yield return new WaitForSeconds(delay);
        if (spell == null) {
            spellTimer = 0f;
            spell = GetUnoccupiedSpot();
            placedSpell = Spell.GetSpell();
            Sprite sprite = Resources.Load<Sprite>($"spells/{placedSpell.name}");
            tiles[spell.x,spell.y].UpdateImage(sprite == null ? spellSprite : sprite);
        }
    }

    void TrapSetup()
    {
        Point p = GetUnoccupiedSpot();
        if (p == null) return;
        trapWarnings.Add(p);
        StartCoroutine(TrapPlacement(p));
    }

    IEnumerator TrapPlacement(Point p)
    {
        yield return new WaitForSeconds(trapWarningTime);
        traps.Add(p);
        trapWarnings.RemoveAll(tw => tw.x == p.x && tw.y == p.y);
        StartCoroutine(TrapRemove(p));
    }

    IEnumerator TrapRemove(Point p)
    {
        yield return new WaitForSeconds(trapDisappearDelay);
        traps.RemoveAll(tw => tw.x == p.x && tw.y == p.y);
    }

    public void PlanWallPlace(bool isTemp)
    {
        List<Point> options = new List<Point>();
        List<Point> wallEnds = new List<Point>();
        for (int y = 0; y < SIZE; y++) {
            for (int x = 0; x < SIZE; x++) {
                if (IsTileEmpty(x,y)) options.Add(new Point(x,y));
            }
        }
        while (options.Count > 0) {
            wallEnds.Clear();
            int i = Random.Range(0,options.Count);
            Point p = options[i];
            options.RemoveAt(i);
            if (p.x >= 2 && !snakeController.snake.InBody(p.x-1,p.y) && !snakeController.snake.InBody(p.x-2,p.y)) {
                wallEnds.Add(new Point(p.x-2,p.y));
            }
            if (p.x <= 13 && !snakeController.snake.InBody(p.x+1,p.y) && !snakeController.snake.InBody(p.x+2,p.y)) {
                wallEnds.Add(new Point(p.x+2,p.y));
            }
            if (p.y >= 2 && !snakeController.snake.InBody(p.x,p.y-1) && !snakeController.snake.InBody(p.x,p.y-2)) {
                wallEnds.Add(new Point(p.x,p.y-2));
            }
            if (p.y <= 13 && !snakeController.snake.InBody(p.x,p.y+1) && !snakeController.snake.InBody(p.x,p.y+2)) {
                wallEnds.Add(new Point(p.x,p.y+2));
            }
            if (wallEnds.Count > 0) {
                StartCoroutine(WallSetup(p,wallEnds[Random.Range(0,wallEnds.Count)],isTemp));
                return;
            }
        }
    }

    IEnumerator WallSetup(Point p1, Point p2, bool isTemp)
    {
        PlaceWallWarning(p1.x,p1.y);
        PlaceWallWarning(p2.x,p2.y);
        PlaceWallWarning((p1.x + p2.x) / 2,(p1.y + p2.y) / 2);
        yield return new WaitForSeconds(wallWarningTime);
        PlaceWall(p1.x,p1.y,isTemp);
        PlaceWall(p2.x,p2.y,isTemp);
        PlaceWall((p1.x + p2.x) / 2,(p1.y + p2.y) / 2,isTemp);
    }

    IEnumerator WallRemove(int x, int y)
    {
        yield return new WaitForSeconds(tempWallTimerCap);
        tempWalls.RemoveAll(p => p.x == x && p.y == y);
    }

    void PlaceWallWarning(int x, int y)
    {
        tiles[x,y].PlaceWallWarning();
        wallWarnings.Add(new Point(x,y));
    }

    void PlaceWall(int x, int y, bool isTemp)
    {
        wallWarnings.RemoveAll(p => p.x == x && p.y == y);
        if (isTemp) {
            tiles[x,y].PlaceTempWall();
            tempWalls.Add(new Point(x,y));
            StartCoroutine(WallRemove(x,y));
        }
        else {
            tiles[x,y].PlacePermaWall();
            permaWalls.Add(new Point(x,y));
        }        
    }

    public void UpdateMap()
    {
        for (int y = 0; y < SIZE; y++) {
            for (int x = 0; x < SIZE; x++) {
                if (!snakeController.snake.InBody(x,y)) tiles[x,y].HideTile();
            }
        }
        if (food != null) tiles[food.x,food.y].SetAsFood();
        if (spell != null) {
            Sprite sprite = Resources.Load<Sprite>($"spells/{placedSpell.name}");
            tiles[spell.x,spell.y].UpdateImage(sprite == null ? spellSprite : sprite);
        }

        foreach (Point p in tempWalls) {
            tiles[p.x,p.y].PlaceTempWall();
        }
        foreach (Point p in permaWalls) {
            tiles[p.x,p.y].PlacePermaWall();
        }
        foreach (Point p in wallWarnings) {
            tiles[p.x,p.y].PlaceWallWarning();
        }

        foreach (Point p in traps) {
            tiles[p.x,p.y].PlaceTrap();
        }
        foreach (Point p in trapWarnings) {
            tiles[p.x,p.y].PlaceTrapWarning();
        }
    }

    public void SetTile(int x, int y, Sprite sprite, Color color, int yRotation, int zRotation)
    {
        tiles[x,y].UpdateImage(sprite);
        tiles[x,y].RotateImage(yRotation,zRotation);
        tiles[x,y].SetColor(color);
    }

    public void SetTile(Point p, Sprite sprite, Color color, int yRotation, int zRotation)
    {
        SetTile(p.x,p.y,sprite,color, yRotation,zRotation);
    }

    public void SetTileColor(Point point, Color color)
    {
        tiles[point.x,point.y].SetColor(color);
    }

    public MapCell GetTile(Point point)
    {
        return tiles[point.x,point.y];
    }

    public void RemoveTrap(Point point)
    {
        traps.RemoveAll(p => p.x == point.x && p.y == point.y);
    }

    public void DestroyWall(Point p)
    {
        tempWalls.RemoveAll(tw => tw.x == p.x && tw.y == p.y);
        permaWalls.RemoveAll(pw => pw.x == p.x && pw.y == p.y);
    }
}
