using System.Collections;
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

        if (spell != null) {
            // spell is placed, tick timer until it disappears
            spellTimer += Time.deltaTime;
            if (spellTimer >= spellTimerCap) {
                tiles[spell.x,spell.y].HideTile();
                spell = null;
                placedSpell = null;
                StartCoroutine(DelayedSpellSpawn(2f));
            }
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
        return options[Random.Range(0,options.Count)];
    }

    bool IsTileEmpty(int x, int y)
    {
        return (!snakeController.snake.InBody(x,y)
                && !tiles[x,y].isCollider
                && (food == null || !food.Equals(x,y))
                && (spell == null || !spell.Equals(x,y)));
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
            spell = null;
            placedSpell = null;
        }
        yield return new WaitForSeconds(delay);
        if (spell == null) {
            spellTimer = 0f;
            spell = GetUnoccupiedSpot();
            placedSpell = Spell.GetSpell();
            Sprite sprite = Resources.Load<Sprite>($"spells/{placedSpell.name}");
            tiles[spell.x,spell.y].UpdateImage(sprite == null ? spellSprite : sprite);
            // TODO can have different sprites / colors for different spells?
        }
    }

    public void UpdateMap()
    {
        for (int y = 0; y < SIZE; y++) {
            for (int x = 0; x < SIZE; x++) {
                if (IsTileEmpty(x,y)) tiles[x,y].HideTile();
            }
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
}
