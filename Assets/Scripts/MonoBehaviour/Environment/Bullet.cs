using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float Health = 1.0f;
    public AudioClip[] Sound;
    public AudioSource AudioSource;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.other.tag == "DavyJonson")
        {
            collision.other.gameObject.GetComponent<DavyJones>().IsDavyHit(5);
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
