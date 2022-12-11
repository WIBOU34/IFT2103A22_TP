using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public sealed class SoundManager
{
    private static SoundManager instance = null;
    private static readonly object padlock = new object();
    private string fileName = @".\soundsSettings.json";
    private SoundsViewModel soundsViewModel = new SoundsViewModel();

    public AudioSource playerSoundEffectsAudioSource;
    public AudioSource gameMusicAudioSource;
    public AudioSource mainMenuMusicAudioSource;
    private List<AudioSource> musicAudioSources = new List<AudioSource>();
    private List<AudioSource> foleyAudioSources = new List<AudioSource>();
    private List<AudioSource> sfxAudioSources = new List<AudioSource>();
    private AudioClip dyingSound;
    private AudioClip pistolSound;
    private AudioClip reloadSound;
    private AudioClip damageTakenSound;
    private AudioClip zombieVoiceSound;
    private AudioClip zombieStepsSound;
    private AudioClip zombieAttackSound;
    private AudioClip zombieDyingSound;
    private AudioClip gameMusic;
    private AudioClip mainMenuMusic;
    private bool dyingSoundHasPlayed = false;
    private float musicVolume = 1;
    public float foleyVolume = 1;
    private float sfxVolume = 1;

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
        mainMenuMusic = Resources.Load<AudioClip>("Audios/MX/Ambience/CoFLoop2");
        LoadVolumes();
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

    public void PlayMainMenuMusic() 
    {
        AddToMusicAudioSources(mainMenuMusicAudioSource);
        mainMenuMusicAudioSource.clip = mainMenuMusic;
        mainMenuMusicAudioSource.loop = true;
        mainMenuMusicAudioSource.Play();
    }

    public void PlayGameMusic()
    {
        AddToMusicAudioSources(gameMusicAudioSource);
        gameMusicAudioSource.clip = gameMusic;
        gameMusicAudioSource.loop = true;
        gameMusicAudioSource.Play();
    }

    public void PlayZombieVoiceSound(AudioSource audioSource)
    {
        AddToFoleyAudioSources(audioSource);
        audioSource.clip = zombieVoiceSound;
        audioSource.loop = true;
        audioSource.spatialBlend = 1;
        audioSource.minDistance = 1;
        audioSource.maxDistance = 10;
        audioSource.Play();        
    }

    public void PlayZombieStepsSound(AudioSource audioSource)
    {
        AddToFoleyAudioSources(audioSource);
        audioSource.clip = zombieStepsSound;
        audioSource.loop = true;
        audioSource.spatialBlend = 1;
        audioSource.minDistance = 1;
        audioSource.maxDistance = 10;
        audioSource.Play();
    }

    public void PlayZombieAttackSound(AudioSource audioSource)
    {
        AddToSFXAudioSources(audioSource);
        audioSource.PlayOneShot(zombieAttackSound);
    }

    public void PlayZombieDyingSound(AudioSource audioSource)
    {
        AddToSFXAudioSources(audioSource);
        audioSource.PlayOneShot(zombieDyingSound);
    }

    public void PlayerDamageTakenSound()
    {
        AddToSFXAudioSources(playerSoundEffectsAudioSource);
        playerSoundEffectsAudioSource.PlayOneShot(damageTakenSound);
    }

    public void PlayReloadSound()
    {
        AddToSFXAudioSources(playerSoundEffectsAudioSource);
        playerSoundEffectsAudioSource.PlayOneShot(reloadSound);
    }

    public void PlayPistolSound()
    {
        AddToSFXAudioSources(playerSoundEffectsAudioSource);
        playerSoundEffectsAudioSource.PlayOneShot(pistolSound);
    }

    public void PlayDyingSound()
    {
        if (!dyingSoundHasPlayed)
        {
            AddToSFXAudioSources(playerSoundEffectsAudioSource);
            playerSoundEffectsAudioSource.PlayOneShot(dyingSound);
            dyingSoundHasPlayed = true;
        }        
    }

    public void ClearAudioSources()
    {
        musicAudioSources.Clear();
        foleyAudioSources.Clear();
        sfxAudioSources.Clear();
    }

    public void PauseAudioSources()
    {
        foreach(AudioSource audioSource in musicAudioSources)
        {
            audioSource.Pause();
        }

        foreach(AudioSource audioSource in foleyAudioSources)
        {
            audioSource.Pause();
        }

        foreach(AudioSource audioSource in sfxAudioSources)
        {
            audioSource.Pause();
        }
    }

    public void ResumeAudioSources()
    {
        foreach(AudioSource audioSource in musicAudioSources)
        {
            if (audioSource.clip.name == mainMenuMusic.name)
            {
                audioSource.Stop();
            }
        }

        foreach (AudioSource audioSource in musicAudioSources)
        {
            if (audioSource.clip.name != mainMenuMusic.name)
            {
                audioSource.Play();
            }           
        }

        foreach (AudioSource audioSource in foleyAudioSources)
        {
            audioSource.Play();
        }

        foreach (AudioSource audioSource in sfxAudioSources)
        {
            audioSource.Play();
        }
    }

    public void UpdateMusicVolume(float volume)
    {
        musicVolume = volume;

        foreach(AudioSource audioSource in musicAudioSources)
        {
            audioSource.volume = volume;
        }
    }

    public void UpdateFoleyVolume(float volume)
    {
        foleyVolume = volume;

        foreach(AudioSource audioSource in foleyAudioSources)
        {
            audioSource.volume = volume;
        }
    }

    public void UpdateSFXVolume(float volume)
    {
        sfxVolume = volume;

        foreach(AudioSource audioSource in sfxAudioSources)
        {
            audioSource.volume = volume;
        }
    }

    private void AddToMusicAudioSources(AudioSource audioSource)
    {
        if (!musicAudioSources.Contains(audioSource))
        {
            musicAudioSources.Add(audioSource);
            UpdateMusicVolume(musicVolume);
        }
    }

    private void AddToFoleyAudioSources(AudioSource audioSource)
    {
        if (!foleyAudioSources.Contains(audioSource))
        {
            foleyAudioSources.Add(audioSource);
            UpdateFoleyVolume(foleyVolume);
        }
    }

    private void AddToSFXAudioSources(AudioSource audioSource)
    {
        if (!sfxAudioSources.Contains(audioSource))
        {
            sfxAudioSources.Add(audioSource);
            UpdateSFXVolume(sfxVolume);
        }
    }

    private void LoadVolumes()
    {
        if (File.Exists(fileName))
        {
            string json = File.ReadAllText(fileName);
            soundsViewModel = JsonConvert.DeserializeObject<SoundsViewModel>(json);
        }
        else
        {
            InitSoundsViewModel();
            Save();
        }

        musicVolume = soundsViewModel.MusicVolume / 100;
        foleyVolume = soundsViewModel.FoleysVolume / 100;
        sfxVolume = soundsViewModel.SFXVolume / 100;

        UpdateMusicVolume(musicVolume);
        UpdateFoleyVolume(foleyVolume);
        UpdateSFXVolume(sfxVolume);
    }

    private void InitSoundsViewModel()
    {
        soundsViewModel = new SoundsViewModel
        {
            MusicVolume = 100,
            FoleysVolume = 100,
            SFXVolume = 100
        };
    }

    private void Save()
    {
        string json = JsonConvert.SerializeObject(soundsViewModel);

        if (!File.Exists(fileName))
        {
            File.WriteAllText(fileName, json);
        }
        else
        {
            File.Delete(fileName);
            File.WriteAllText(fileName, json);
        }
    }
}