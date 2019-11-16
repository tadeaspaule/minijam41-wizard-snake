using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapCell : MonoBehaviour
{
    Image image;
    bool animated = false;
    Sprite[] frames;
    int frameI = 0;

    float timer = 0f;
    float timerStep = 1f / 6f;

    public bool isCollider = false;
    public bool isTrap = false;
    public bool isWarning = false;
    
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
    }

    void Update()
    {
        if (!animated) return;
        timer += Time.deltaTime;
        if (timer >= timerStep) {
            timer = 0f;
            frameI = (frameI+1) % frames.Length;
            image.sprite = frames[frameI];
        }
    }

    public void UpdateImage(Sprite sprite)
    {
        ResetBasics();
        image.sprite = sprite;
    }

    public void SetColor(Color color)
    {
        image.color = color;
    }

    public void HideTile()
    {
        ResetBasics();
        image.color = new Color(1f,1f,1f,0f);
    }

    public void RotateImage(int yDegrees, int zDegrees)
    {
        transform.rotation = Quaternion.Euler(0f,yDegrees,zDegrees);
    }

    public void SetAsFood()
    {
        ResetBasics();
        image.sprite = Resources.Load<Sprite>("cookie");
        // animated = true;
        // frames = Resources.LoadAll<Sprite>("food");
        // frameI = 0;
        // image.sprite = frames[0];
        // timer = 0f;
    }

    public void PlaceTempWall()
    {
        ResetBasics();
        isCollider = true;
        image.sprite = Resources.Load<Sprite>("tempwall");
    }

    public void PlacePermaWall()
    {
        ResetBasics();
        isCollider = true;
        image.sprite = Resources.Load<Sprite>("permawall");
    }

    public void PlaceWallWarning()
    {
        ResetBasics();
        isWarning = true;
        image.sprite = Resources.Load<Sprite>("redwarning");
    }

    public void PlaceTrap()
    {
        ResetBasics();
        isTrap = true;
        image.sprite = Resources.Load<Sprite>("trap");
    }

    public void PlaceTrapWarning()
    {
        ResetBasics();
        isWarning = true;
        image.sprite = Resources.Load<Sprite>("yellowwarning");
    }

    void ResetBasics()
    {
        image.color = Color.white;
        transform.rotation = Quaternion.identity;
        animated = false;
        isCollider = false;
        isTrap = false;
        isWarning = false;
    }
}
