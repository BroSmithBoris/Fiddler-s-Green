using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip[] ShipSounds;
    private AudioSource AudioSource;

    private float time = 12f;

    void Start() 
    {
        AudioSource = GetComponent<AudioSource>();
    }

    void UpdateTime() 
    {
        time = 12f;
    }
    void Update()
    {
        time -= Time.deltaTime;
        if (time <= 0f)
        {
           var rnd = Random.Range(0, ShipSounds.Length - 1);
           AudioSource.PlayOneShot(ShipSounds[rnd],0.3f);
           UpdateTime();
        }
    }
}
