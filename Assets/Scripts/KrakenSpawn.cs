using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KrakenSpawn : MonoBehaviour
{
    public GameObject SpawnZone1;
    public GameObject Davy;
    public GameObject Kraken;
    public Control Player;
    
    public AudioClip KrakenSpawnSound;
    public AudioSource AudioSource;
    private bool KrakenCare;
    [SerializeField] private Animator KrakenAnimator;

    private bool DavyMove = true;
    private GUIStyle style = new GUIStyle();

    void Start()
    {
        style.fontSize = 40;
    }
    private void OnTriggerStay(Collider other) 
    {
        if (other.tag == "Player")
        {
            KrakenCare = true;
            if ((int)Player.PlayerArea == 0)
            {
                Kraken.SetActive(true);
                if (DavyMove)
                {
                    AudioSource.PlayOneShot(KrakenSpawnSound);
                    Davy.transform.position = SpawnZone1.transform.position;
                    DavyMove = false;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            KrakenCare = false;
            KrakenAnimator.SetBool(0, true);
            KrakenAnimator.SetBool(1, true);
            KrakenAnimator.SetBool("IsDie", true);
            Invoke("KrakenDie",2.0f);
            Kraken.SetActive(false);
        }
    }

    private void KrakenDie()
    {
        Kraken.SetActive(false);
    }

    private void OnGUI() 
    {
        if (KrakenCare)
        {
            GUI.Label(new Rect(Screen.width/2 - 300f,Screen.height/2 - 400f,500f,300f), "Caution! Leviathan attacks!", style);
        }
    }
}