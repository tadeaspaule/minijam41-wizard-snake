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
    
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
    }

    public void UpdateImage(Sprite sprite)
    {
        ResetBasics();
        image.sprite = sprite;
    }

    public void HideTile()
    {
        animated = false;
        image.color = new Color(1f,1f,1f,0f);
    }

    public void RotateImage(int yDegrees, int zDegrees)
    {
        transform.rotation = Quaternion.Euler(0f,yDegrees,zDegrees);
    }

    public void SetAsFood()
    {
        ResetBasics();
        animated = true;
        frames = Resources.LoadAll<Sprite>("food");
        frameI = 0;
    }

    void ResetBasics()
    {
        image.color = Color.white;
        transform.rotation = Quaternion.identity;
        animated = false;
    }
}
