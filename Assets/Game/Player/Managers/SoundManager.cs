using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public sealed class SoundManager
{
    private static SoundManager instance = null;
    private static readonly object padlock = new object();

    public AudioSource audioSource;
    private AudioClip dyingSound;
    private AudioClip pistolSound;
    private AudioClip reloadSound;
    private bool dyingSoundHasPlayed = false;


    SoundManager()
    {
        dyingSound = Resources.Load<AudioClip>("Audios/Character/MaleEfforts/Man_Damage_Extreme_1");
        pistolSound = Resources.Load<AudioClip>("Audios/weapons/guns/hi action");
        reloadSound = Resources.Load<AudioClip>("Audios/PostApocalypseGunsDemo/Pistols/Pistol_ClipIn_05");
    }

    public static SoundManager Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new SoundManager();
                }
                return instance;
            }
        }
    }

    public void PlayReloadSound()
    {
        audioSource.PlayOneShot(reloadSound);
    }

    public void PlayPistolSound()
    {
        audioSource.PlayOneShot(pistolSound);
    }

    public void PlayDyingSound()
    {
        if (!dyingSoundHasPlayed)
        {
            audioSource.PlayOneShot(dyingSound);
            dyingSoundHasPlayed = true;
        }        
    }
}