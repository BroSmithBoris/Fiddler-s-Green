using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DavySoundManager : MonoBehaviour
{
    public AudioClip StepSound;
    public AudioClip FireSound;
    public AudioClip LaughSound;
    private AudioSource AudioSource;
    private DavyJones Davy;
    void Start()
    {
        Davy = GetComponent<DavyJones>();
        AudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Davy.StepSoundActive && !AudioSource.isPlaying)
        {
            AudioSource.PlayOneShot(StepSound);
        }

        if (Davy.FireSoundActive && !AudioSource.isPlaying)
        {
            AudioSource.PlayOneShot(FireSound);
        }

        if (Davy.LaughSoundActive && !AudioSource.isPlaying)
        {
            AudioSource.PlayOneShot(LaughSound);

        }
    }
}
