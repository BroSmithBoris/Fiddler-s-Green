using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    private GUIStyle style = new GUIStyle();
    private bool end;

    private void Start()
    {
        style.fontSize = 40;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player")
            end = true;
    }
    private void OnGUI() 
    {
        if (end)
            GUI.Label(new Rect(Screen.width/2 - 300f,Screen.height/2 - 400f,500f,300f), "You have reached the Fiddler's glade!", style);
    }
}
