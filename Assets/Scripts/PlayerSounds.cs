using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    public AudioClip[] StepsSound;
    public AudioClip FireSound;
    public AudioClip SaberSound;
    private Control PlayerControl;
    private AudioSource AudioSource;

    void Start()
    {
        AudioSource = GetComponent<AudioSource>();
        PlayerControl = GetComponent<Control>();
    }

    void Update()
    {
        if (PlayerControl.StepSoundActive)
        {
            if (!AudioSource.isPlaying)
            {
                var rnd = Random.Range(0, StepsSound.Length - 1);
                AudioSource.PlayOneShot(StepsSound[rnd]);
            }
        }

        if (PlayerControl.FireSoundActive)
            AudioSource.PlayOneShot(FireSound);

        if (PlayerControl.SaberSoundActive)
            AudioSource.PlayOneShot(SaberSound);
    }
}
