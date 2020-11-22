using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthHUD : MonoBehaviour
{
    private Image HP_HUD;
    public GameObject Player;
    private float PlayerHP;
    private float alpha;

    void Start() 
    {
        HP_HUD = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerHP = Player.GetComponent<Control>().Health;

        if (PlayerHP == 70)
            alpha = 0.1f;
        
        if (PlayerHP == 40)
            alpha = 0.3f;

        if (PlayerHP == 10)
            alpha = 0.5f;
        
        if (PlayerHP == 0)
            alpha = 1f;

        HP_HUD.color = new Color(0.75f, 0f, 0f, alpha);
    }
}
