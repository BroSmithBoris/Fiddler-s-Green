using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saber : MonoBehaviour
{
    [SerializeField] private Control Player;
    public AudioClip[] Sound;
    public AudioSource AudioSource;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "DavyJonson" && Player.IsAttacking)
        {
            other.gameObject.GetComponent<DavyJones>().IsDavyHit(5);
        }
    }
}
