using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public sealed class SoundManager
{
    private static SoundManager instance = null;
    private static readonly object padlock = new object();

    public AudioSource audioSource;
    private AudioClip dyingSound;
    private AudioClip pistolSound;
    private AudioClip reloadSound;
    private AudioClip damageTakenSound;
    private AudioClip zombieVoiceSound;
    private AudioClip zombieStepsSound;
    private AudioClip zombieAttackSound;
    private AudioClip zombieDyingSound;
    private AudioClip gameMusic;
    private bool dyingSoundHasPlayed = false;


    SoundManager()
    {
        dyingSound = Resources.Load<AudioClip>("Audios/Character/MaleEfforts/Man_Damage_Extreme_1");
        pistolSound = Resources.Load<AudioClip>("Audios/weapons/guns/hi action");
        reloadSound = Resources.Load<AudioClip>("Audios/PostApocalypseGunsDemo/Pistols/Pistol_ClipIn_05");
        damageTakenSound = Resources.Load<AudioClip>("Audios/Character/MaleEfforts/Man_Damage_1");
        zombieVoiceSound = Resources.Load<AudioClip>("Audios/Enemies/Zombie/Voice/Zombie_Normal_Ver1_1");
        zombieStepsSound = Resources.Load<AudioClip>("Audios/Enemies/Zombie/Steps/Zombie_Steps_01");
        zombieAttackSound = Resources.Load<AudioClip>("Audios/Enemies/Zombie/Voice/Zombie_Attack_2");
        zombieDyingSound = Resources.Load<AudioClip>("Audios/Enemies/Zombie/Voice/Zombie_Dead_Ver1_1");
        gameMusic = Resources.Load<AudioClip>("Audios/free horror ambience 2/ha-abomination");
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

    public void PlayGameMusic()
    {
        audioSource.clip = gameMusic;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void PlayZombieVoiceSound(AudioSource audioSource)
    {
        audioSource.clip = zombieVoiceSound;
        audioSource.loop = true;
        audioSource.spatialBlend = 1;
        audioSource.minDistance = 1;
        audioSource.maxDistance = 10;
        audioSource.Play();        
    }

    public void PlayZombieStepsSound(AudioSource audioSource)
    {
        audioSource.clip = zombieStepsSound;
        audioSource.loop = true;
        audioSource.spatialBlend = 1;
        audioSource.minDistance = 1;
        audioSource.maxDistance = 10;
        audioSource.Play();
    }

    public void PlayZombieAttackSound(AudioSource audioSource)
    {
        audioSource.PlayOneShot(zombieAttackSound);
    }

    public void PlayZombieDyingSound(AudioSource audioSource)
    {
        audioSource.PlayOneShot(zombieDyingSound);
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