using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePanel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int i = Screen.height / 128;
        transform.localScale = new Vector3(i,i,1f);
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
