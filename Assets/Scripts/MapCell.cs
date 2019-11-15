using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapCell : MonoBehaviour
{
    Image image;
    
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
    }

    public void UpdateImage(Sprite sprite)
    {
        image.color = Color.white;
        image.sprite = sprite;
    }

    public void HideTile()
    {
        image.color = new Color(1f,1f,1f,0f);
    }
}
