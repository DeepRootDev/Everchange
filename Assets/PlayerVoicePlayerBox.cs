using System;
using UnityEngine;

public class PlayerVoicePlayerBox : MonoBehaviour
{

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void PlaySound(AudioClip soundToPlay)
    {
        audioSource.PlayOneShot(soundToPlay);
    }
}
