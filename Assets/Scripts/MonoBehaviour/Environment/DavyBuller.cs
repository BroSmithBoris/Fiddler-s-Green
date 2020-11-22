using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DavyBuller : MonoBehaviour
{
    [SerializeField] private float Health = 3.0f;
    public AudioClip[] Sound;
    private AudioSource AudioSource;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.other.tag == "Player")
        {
            collision.other.gameObject.GetComponent<Control>().IsPlayerHit(5.0f);
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        Health -= Time.deltaTime;
        if (Health < 0.0f)
            Destroy(gameObject);
    }
}
