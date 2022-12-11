using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public sealed class SoundManager
{
    private static SoundManager instance = null;
    private static readonly object padlock = new object();

    public AudioSource audioSource;
    //public List<AudioSource> zombiesAudioSources;
    private AudioClip dyingSound;
    private AudioClip pistolSound;
    private AudioClip reloadSound;
    private AudioClip damageTakenSound;
    private AudioClip zombieNormalSound;
    private bool dyingSoundHasPlayed = false;


    SoundManager()
    {
        //zombiesAudioSources = new List<AudioSource>();
        dyingSound = Resources.Load<AudioClip>("Audios/Character/MaleEfforts/Man_Damage_Extreme_1");
        pistolSound = Resources.Load<AudioClip>("Audios/weapons/guns/hi action");
        reloadSound = Resources.Load<AudioClip>("Audios/PostApocalypseGunsDemo/Pistols/Pistol_ClipIn_05");
        damageTakenSound = Resources.Load<AudioClip>("Audios/Character/MaleEfforts/Man_Damage_1");
        zombieNormalSound = Resources.Load<AudioClip>("Audios/Enemies/Zombie/Voice/Zombie_Normal_Ver1_1");
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

    public void PlayZombieNormalSound(AudioSource audioSource)
    {
        audioSource.clip = zombieNormalSound;
        audioSource.loop = true;
        audioSource.spatialBlend = 1;
        audioSource.minDistance = 1;
        audioSource.maxDistance = 5;
        audioSource.Play();
    }

    public void PlayerDamageTakenSound()
    {
        audioSource.PlayOneShot(damageTakenSound);
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