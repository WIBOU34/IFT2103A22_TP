using UnityEngine;

public class GameMusicController : MonoBehaviour
{
    private SoundManager soundManager;
    private AudioSource gameMusicAudioSourceTrack1;
    private AudioSource gameMusicAudioSourceTrack2;
    private AudioSource gameMusicAudioSourceTrack3;
    private float nextTime = 0;
    private bool track1IsPlaying = false;
    private bool track2IsPlaying = false;
    private bool track3IsPlaying = false;

    private void Awake()
    {
        soundManager = SoundManager.Instance;
        gameMusicAudioSourceTrack1 = gameObject.AddComponent<AudioSource>();
        gameMusicAudioSourceTrack2 = gameObject.AddComponent<AudioSource>();
        gameMusicAudioSourceTrack3 = gameObject.AddComponent<AudioSource>();
        soundManager.gameMusicAudioSourceTrack1 = gameMusicAudioSourceTrack1;
        soundManager.gameMusicAudioSourceTrack2 = gameMusicAudioSourceTrack2;
        soundManager.gameMusicAudioSourceTrack3 = gameMusicAudioSourceTrack3;
    }

    private void Start()
    {        
        soundManager.PlayGameMusicTrack1();
    }

    private void Update()
    {
        if (!soundManager.stopGameMusic)
        {
            if (Time.time >= nextTime)
            {
                if (gameMusicAudioSourceTrack3.isPlaying)
                {
                    float tempsRestantMusic = gameMusicAudioSourceTrack3.clip.length - gameMusicAudioSourceTrack3.time;
                    if (gameMusicAudioSourceTrack3.time < soundManager.fonduCroiseTimeGame)
                    {
                        if (gameMusicAudioSourceTrack3.volume + soundManager.musicVolume / soundManager.fonduCroiseTimeGame < soundManager.musicVolume)
                        {
                            gameMusicAudioSourceTrack3.volume += soundManager.musicVolume / soundManager.fonduCroiseTimeGame;
                        }
                        else
                        {
                            gameMusicAudioSourceTrack3.volume = soundManager.musicVolume;
                        }
                    }
                    else if (tempsRestantMusic < soundManager.fonduCroiseTimeGame)
                    {
                        if (!track1IsPlaying)
                        {
                            soundManager.PlayGameMusicTrack1();                        
                            track1IsPlaying = true;
                        }

                        if (gameMusicAudioSourceTrack3.volume - soundManager.musicVolume / soundManager.fonduCroiseTimeGame > 0)
                        {
                            gameMusicAudioSourceTrack3.volume -= soundManager.musicVolume / soundManager.fonduCroiseTimeGame;
                        }
                        else
                        {
                            gameMusicAudioSourceTrack3.volume = 0;
                        }
                    }
                }
                else
                {
                    track3IsPlaying = false;
                }

                if (gameMusicAudioSourceTrack2.isPlaying)
                {
                    float tempsRestantMusic = gameMusicAudioSourceTrack2.clip.length - gameMusicAudioSourceTrack2.time;
                    if (gameMusicAudioSourceTrack2.time < soundManager.fonduCroiseTimeGame)
                    {
                        if (gameMusicAudioSourceTrack2.volume + soundManager.musicVolume / soundManager.fonduCroiseTimeGame < soundManager.musicVolume)
                        {
                            gameMusicAudioSourceTrack2.volume += soundManager.musicVolume / soundManager.fonduCroiseTimeGame;
                        }
                        else
                        {
                            gameMusicAudioSourceTrack2.volume = soundManager.musicVolume;
                        }
                    }
                    else if (tempsRestantMusic < soundManager.fonduCroiseTimeGame)
                    {
                        if (!track3IsPlaying)
                        {
                            soundManager.PlayGameMusicTrack3();
                            gameMusicAudioSourceTrack3.volume += soundManager.musicVolume / soundManager.fonduCroiseTimeGame;                            
                            track3IsPlaying = true;
                        }

                        if (gameMusicAudioSourceTrack2.volume - soundManager.musicVolume / soundManager.fonduCroiseTimeGame > 0)
                        {
                            gameMusicAudioSourceTrack2.volume -= soundManager.musicVolume / soundManager.fonduCroiseTimeGame;
                        }
                        else
                        {
                            gameMusicAudioSourceTrack2.volume = 0;
                        }
                    }
                }
                else
                {
                    track2IsPlaying = false;
                }

                if (gameMusicAudioSourceTrack1.isPlaying)
                {
                    float tempsRestantMusic = gameMusicAudioSourceTrack1.clip.length - gameMusicAudioSourceTrack1.time;
                    if (gameMusicAudioSourceTrack1.time < soundManager.fonduCroiseTimeGame)
                    {
                        if (gameMusicAudioSourceTrack1.volume + soundManager.musicVolume / soundManager.fonduCroiseTimeGame < soundManager.musicVolume)
                        {
                            gameMusicAudioSourceTrack1.volume += soundManager.musicVolume / soundManager.fonduCroiseTimeGame;
                        }
                        else
                        {
                            gameMusicAudioSourceTrack1.volume = soundManager.musicVolume;
                        }
                    }
                    else if (tempsRestantMusic < soundManager.fonduCroiseTimeGame)
                    {
                        if (!track2IsPlaying)
                        {
                            soundManager.PlayGameMusicTrack2();
                            gameMusicAudioSourceTrack2.volume += soundManager.musicVolume / soundManager.fonduCroiseTimeGame;                            
                            track2IsPlaying = true;
                        }

                        if (gameMusicAudioSourceTrack1.volume - soundManager.musicVolume / soundManager.fonduCroiseTimeGame > 0)
                        {
                            gameMusicAudioSourceTrack1.volume -= soundManager.musicVolume / soundManager.fonduCroiseTimeGame;
                        }
                        else
                        {
                            gameMusicAudioSourceTrack1.volume = 0;
                        }
                    }
                }
                else
                {
                    track1IsPlaying = false;
                }

                nextTime += 1;
            }
        }
    }
}
