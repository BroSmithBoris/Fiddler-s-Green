using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DavySpawn : MonoBehaviour
{
    public GameObject Davy;
    private bool IsSpawn;
    private GUIStyle style = new GUIStyle();
    void Start()
    {
        Invoke("Spawn", 5.0f);
        style.fontSize = 40;
    }

    void Spawn()
    {
        Davy.SetActive(true);
        IsSpawn = true;
    }

    private void OnGUI() 
    {
        if (!IsSpawn)
        {
            Cursor.visible = false;
            GUI.Label(new Rect(Screen.width/2 - 450f,Screen.height/2 - 200f,500f,300f), "Davy Jones has come for you, get ready for battle", style);
        }
    }
}
