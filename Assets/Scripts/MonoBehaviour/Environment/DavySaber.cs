using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DavySaber : MonoBehaviour
{
    public AudioClip[] Sound;
    public AudioSource AudioSource;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.gameObject.GetComponent<Control>().IsPlayerHit(5.0f);
        }
    }
}
