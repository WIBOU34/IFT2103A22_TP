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

    public AudioSource titleAnimationAudioSource;
    public AudioSource playerSoundEffectsAudioSource;
    public AudioSource gameEndingMusic;
    public AudioSource lowHealthGameMusicSource;
    public AudioSource gameMusicAudioSourceTrack1;
    public AudioSource gameMusicAudioSourceTrack2;
    public AudioSource gameMusicAudioSourceTrack3;
    public AudioSource mainMenuMusicAudioSourceTrack1;
    public AudioSource mainMenuMusicAudioSourceTrack2;
    public AudioSource menuButtonEffectsAudioSource;
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
    public AudioClip gameMusic;
    public AudioClip gameMusic2;
    public AudioClip gameMusic3;
    public AudioClip lastGameMusicPlaying;
    public AudioClip mainMenuMusic;
    public AudioClip mainMenuMusic2;
    public AudioClip lastMainMenuMusicPlaying;
    private AudioClip menuButtonOnClickSound;
    private AudioClip menuButtonHoverSound;
    private AudioClip lowHealthClip;
    private AudioClip victoryMusic;
    private AudioClip gameOverMusic;
    private AudioClip titleAnimationSound;
    private bool dyingSoundHasPlayed = false;
    public float musicVolume = 1;
    public float foleyVolume = 1;
    private float sfxVolume = 1;
    public int fonduCroiseTime = 10;
    public int fonduCroiseTimeGame = 20;
    public bool stopMenuMusic = false;
    public bool stopGameMusic = false;
    public bool pauseGameMusic = false;
    public bool gameIsPaused = false;

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
        //gameMusic = Resources.Load<AudioClip>("Audios/free horror ambience 2/ha-abomination");
        //gameMusic2 = Resources.Load<AudioClip>("Audios/free horror ambience 2/ha-amorph");
        gameMusic = Resources.Load<AudioClip>("Audios/Music/Complete Mysterious Forest Game Music Pack/Tension Cue 1 @100 BMP Duration 01_00/Tension Cue 1 @100 BMP Duration 01_00");
        gameMusic2 = Resources.Load<AudioClip>("Audios/Music/Complete Mysterious Forest Game Music Pack/Tension Cue 2 @100 BMP Duration 01_00/Tension Cue 2 @100 BMP Duration 01_00");
        gameMusic3 = Resources.Load<AudioClip>("Audios/Music/Complete Mysterious Forest Game Music Pack/Tension Cue 3 @100 BMP Duration 01_00/Tension Cue 3 @100 BMP Duration 01_00");
        lastGameMusicPlaying = gameMusic3;
        mainMenuMusic = Resources.Load<AudioClip>("Audios/MX/Ambience/CoF");
        mainMenuMusic2 = Resources.Load<AudioClip>("Audios/MX/Ambience/CoF15");
        lastMainMenuMusicPlaying = mainMenuMusic2;
        menuButtonOnClickSound = Resources.Load<AudioClip>("Audios/Menu/Menu_Buttons_3");
        menuButtonHoverSound = Resources.Load<AudioClip>("Audios/Menu/Menu_Buttons_7");
        lowHealthClip = Resources.Load<AudioClip>("Audios/Character/LowHealth Loop");
        victoryMusic = Resources.Load<AudioClip>("Audios/Music/Complete Mysterious Forest Game Music Pack/Moods/Victory music @88 BPM duration 00_20/Victory music @88 BPM duration 00_20");
        gameOverMusic = Resources.Load<AudioClip>("Audios/Music/Complete Mysterious Forest Game Music Pack/Moods/Sad Music @88 BPM duration 00_20/Sad Music @88 BPM duration 00_20");
        titleAnimationSound = Resources.Load<AudioClip>("Audios/lamp wave/sfx lamp wave");
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

    public void PlayTitleAnimationSound()
    {
        AddToSFXAudioSources(titleAnimationAudioSource);
        titleAnimationAudioSource.clip = titleAnimationSound;
        titleAnimationAudioSource.Play();
    }

    public void PlayVictoryMusic()
    {
        StopAllGameAudioSources();
        AddToMusicAudioSources(gameEndingMusic);
        gameEndingMusic.clip = victoryMusic;
        gameEndingMusic.loop = true;
        gameEndingMusic.Play();
    }

    public void PlayGameOverMusic()
    {
        StopAllGameAudioSources();
        AddToMusicAudioSources(gameEndingMusic);
        gameEndingMusic.clip = gameOverMusic;
        gameEndingMusic.loop = true;
        gameEndingMusic.Play();
    }

    public void PlayLowHealthMusic()
    {
        PauseGameMusic();
        AddToMusicAudioSources(lowHealthGameMusicSource);
        lowHealthGameMusicSource.clip = lowHealthClip;
        lowHealthGameMusicSource.loop = true;
        lowHealthGameMusicSource.Play();
    }

    public void StopLowHealthMusicAndResumeGameMusic()
    {
        lowHealthGameMusicSource.Stop();
        ResumeGameMusic();
    }

    public void PlayMenuButtonHoverSound()
    {
        AddToSFXAudioSources(menuButtonEffectsAudioSource);
        menuButtonEffectsAudioSource.PlayOneShot(menuButtonHoverSound);
    }

    public void PlayMenuButtonOnClickSound()
    {
        AddToSFXAudioSources(menuButtonEffectsAudioSource);
        menuButtonEffectsAudioSource.PlayOneShot(menuButtonOnClickSound);
    }

    public void PlayMainMenuMusicTrack1()
    {
        if (!pauseGameMusic)
        {
            StopGameMusic();
        }        
        stopMenuMusic = false;
        AddToMusicAudioSources(mainMenuMusicAudioSourceTrack1);

        mainMenuMusicAudioSourceTrack1.clip = mainMenuMusic;
        lastMainMenuMusicPlaying = mainMenuMusicAudioSourceTrack1.clip;
        mainMenuMusicAudioSourceTrack1.volume = 0; //Pour fondu croisé, volume ajusté dans MenuMusicController
        mainMenuMusicAudioSourceTrack1.Play();
    }

    public void PlayMainMenuMusicTrack2()
    {
        if (!pauseGameMusic)
        {
            StopGameMusic();
        }
        stopMenuMusic = false;
        AddToMusicAudioSources(mainMenuMusicAudioSourceTrack2);

        mainMenuMusicAudioSourceTrack2.clip = mainMenuMusic2;
        lastMainMenuMusicPlaying = mainMenuMusicAudioSourceTrack2.clip;
        mainMenuMusicAudioSourceTrack2.volume = 0; //Pour fondu croisé, volume ajusté dans MenuMusicController
        mainMenuMusicAudioSourceTrack2.Play();
    }

    public void StopMainMenuMusic()
    {
        stopMenuMusic = true;

        if (mainMenuMusicAudioSourceTrack1 != null)
        {
            mainMenuMusicAudioSourceTrack1.Stop();
        }

        if (mainMenuMusicAudioSourceTrack2 != null)
        {
            mainMenuMusicAudioSourceTrack2.Stop();
        }
    }

    public void PlayGameMusicTrack1()
    {
        StopMainMenuMusic();
        stopGameMusic = false;
        AddToMusicAudioSources(gameMusicAudioSourceTrack1);

        gameMusicAudioSourceTrack1.clip = gameMusic;
        lastGameMusicPlaying = gameMusicAudioSourceTrack1.clip;
        gameMusicAudioSourceTrack1.volume = 0; //Pour fondu croisé, volume ajusté dans GameMusicController
        gameMusicAudioSourceTrack1.Play();
    }

    public void PlayGameMusicTrack2()
    {
        StopMainMenuMusic();
        stopGameMusic = false;
        AddToMusicAudioSources(gameMusicAudioSourceTrack2);

        gameMusicAudioSourceTrack2.clip = gameMusic;
        lastGameMusicPlaying = gameMusicAudioSourceTrack2.clip;
        gameMusicAudioSourceTrack2.volume = 0; //Pour fondu croisé, volume ajusté dans GameMusicController
        gameMusicAudioSourceTrack2.Play();
    }

    public void PlayGameMusicTrack3()
    {
        StopMainMenuMusic();
        stopGameMusic = false;
        AddToMusicAudioSources(gameMusicAudioSourceTrack3);

        gameMusicAudioSourceTrack3.clip = gameMusic;
        lastGameMusicPlaying = gameMusicAudioSourceTrack3.clip;
        gameMusicAudioSourceTrack3.volume = 0; //Pour fondu croisé, volume ajusté dans GameMusicController
        gameMusicAudioSourceTrack3.Play();
    }

    public void StopGameMusic()
    {
        stopGameMusic = true;

        if (gameMusicAudioSourceTrack1 != null)
        {
            gameMusicAudioSourceTrack1.Stop();
        }

        if (gameMusicAudioSourceTrack2 != null)
        {
            gameMusicAudioSourceTrack2.Stop();
        }

        if (gameMusicAudioSourceTrack3 != null)
        {
            gameMusicAudioSourceTrack3.Stop();
        }
    }

    public void PauseGameMusic()
    {
        if (gameMusicAudioSourceTrack1 != null)
        {
            gameMusicAudioSourceTrack1.Pause();
        }

        if (gameMusicAudioSourceTrack2 != null)
        {
            gameMusicAudioSourceTrack2.Pause();
        }

        if (gameMusicAudioSourceTrack3 != null)
        {
            gameMusicAudioSourceTrack3.Pause();
        }

        pauseGameMusic = true;
        stopGameMusic = true;
    }

    public void ResumeGameMusic()
    {
        stopGameMusic = false;
        pauseGameMusic = false;

        if (gameMusicAudioSourceTrack1 != null)
        {
            gameMusicAudioSourceTrack1.Play();
        }

        if (gameMusicAudioSourceTrack2 != null)
        {
            gameMusicAudioSourceTrack2.Play();
        }

        if (gameMusicAudioSourceTrack3 != null)
        {
            gameMusicAudioSourceTrack3.Play();
        }
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

    public void StopAllGameAudioSources()
    {
        foreach (AudioSource audioSource in musicAudioSources)
        {
            if (audioSource != null)
            {
                audioSource.Stop();
            }            
        }

        foreach (AudioSource audioSource in foleyAudioSources)
        {
            if (audioSource != null)
            {
                audioSource.Stop();
            }
        }

        foreach (AudioSource audioSource in sfxAudioSources)
        {
            if (audioSource != null)
            {
                audioSource.Stop();
            }
        }

        ClearAudioSources();
    }

    public void ClearAudioSources()
    {
        musicAudioSources = new List<AudioSource>();
        foleyAudioSources = new List<AudioSource>();
        sfxAudioSources = new List<AudioSource>();
    }

    public void PauseAudioSources()
    {
        foreach (AudioSource audioSource in musicAudioSources)
        {
            if (audioSource != null)
            {
                audioSource.Pause();
            }
        }

        foreach (AudioSource audioSource in foleyAudioSources)
        {
            if (audioSource != null)
            {
                audioSource.Pause();
            }
        }

        foreach (AudioSource audioSource in sfxAudioSources)
        {
            if (audioSource != null)
            {
                audioSource.Pause();
            }
        }
    }

    public void ResumeAudioSources()
    {
        foreach (AudioSource audioSource in musicAudioSources)
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

        foreach (AudioSource audioSource in musicAudioSources)
        {
            if (audioSource != null)
            {
                audioSource.volume = volume;
            }
        }
    }

    public void UpdateFoleyVolume(float volume)
    {
        foleyVolume = volume;

        foreach (AudioSource audioSource in foleyAudioSources)
        {
            if (audioSource != null)
            {
                audioSource.volume = volume;
            }
        }
    }

    public void UpdateSFXVolume(float volume)
    {
        sfxVolume = volume;

        foreach (AudioSource audioSource in sfxAudioSources)
        {
            if (audioSource != null)
            {
                audioSource.volume = volume;
            }
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