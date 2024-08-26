using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioControl : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip backgroundMusic;  // For example, normal hit sound
    public AudioClip critHitSound;  // For example, critical hit sound
    public AudioClip normalHitSound;
    public AudioClip spellCasting;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
