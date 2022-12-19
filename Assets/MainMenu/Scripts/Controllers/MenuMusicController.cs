using UnityEngine;

public class MenuMusicController : MonoBehaviour
{
    private SoundManager soundManager;
    private AudioSource mainMenuMusicAudioSourceTrack1;
    private AudioSource mainMenuMusicAudioSourceTrack2;
    private AudioSource menuButtonEffectsAudioSource;
    private float nextTime = 0;
    private bool track1IsPlaying = false;
    private bool track2IsPlaying = false;
    private float nbSecondeMusic2Joue = 0;
    private float nbSecondeMusic1Joue = 0;

    private void Start()
    {
        init();
        soundManager.PlayMainMenuMusicTrack1();
    }

    private void init()
    {
        soundManager = SoundManager.Instance;
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas != null)
        {
            mainMenuMusicAudioSourceTrack1 = canvas.GetComponent<AudioSource>();
            mainMenuMusicAudioSourceTrack2 = canvas.AddComponent<AudioSource>();
            menuButtonEffectsAudioSource = canvas.AddComponent<AudioSource>();
            soundManager.mainMenuMusicAudioSourceTrack1 = mainMenuMusicAudioSourceTrack1;
            soundManager.mainMenuMusicAudioSourceTrack2 = mainMenuMusicAudioSourceTrack2;
            soundManager.menuButtonEffectsAudioSource = menuButtonEffectsAudioSource;
        }
        else
        {
            GameObject menu = GameObject.Find("Menu");
            mainMenuMusicAudioSourceTrack1 = menu.AddComponent<AudioSource>();
            mainMenuMusicAudioSourceTrack2 = menu.AddComponent<AudioSource>();
            soundManager.mainMenuMusicAudioSourceTrack1 = mainMenuMusicAudioSourceTrack1;
            soundManager.mainMenuMusicAudioSourceTrack2 = mainMenuMusicAudioSourceTrack2;
            menuButtonEffectsAudioSource = menu.AddComponent<AudioSource>();
            soundManager.menuButtonEffectsAudioSource = menuButtonEffectsAudioSource;
        }
    }

    private void Update()
    {
        if(!soundManager.stopMenuMusic)
        {
            if (mainMenuMusicAudioSourceTrack2 == null || mainMenuMusicAudioSourceTrack1 == null)
            {
                init();
            }

            if (mainMenuMusicAudioSourceTrack1.isPlaying == false && mainMenuMusicAudioSourceTrack2.isPlaying == false)
            {
                soundManager.PlayMainMenuMusicTrack1();
            }

            if (mainMenuMusicAudioSourceTrack2 != null && mainMenuMusicAudioSourceTrack1 != null)
            {
                if (Time.time >= nextTime)
                {
                    if (mainMenuMusicAudioSourceTrack2.isPlaying)
                    {
                        nbSecondeMusic2Joue++;
                        float tempsRestantMusic = mainMenuMusicAudioSourceTrack2.clip.length - mainMenuMusicAudioSourceTrack2.time;

                        if (nbSecondeMusic2Joue == soundManager.fonduCroiseTime)
                        {
                            track1IsPlaying = false;
                            soundManager.mainMenuMusicAudioSourceTrack1.Stop();
                            nbSecondeMusic1Joue = 0;
                        }

                        if (mainMenuMusicAudioSourceTrack2.time < soundManager.fonduCroiseTime)
                        {
                            if (mainMenuMusicAudioSourceTrack2.volume + soundManager.musicVolume / soundManager.fonduCroiseTime < soundManager.musicVolume)
                            {
                                mainMenuMusicAudioSourceTrack2.volume += soundManager.musicVolume / soundManager.fonduCroiseTime;
                            }
                            else
                            {
                                mainMenuMusicAudioSourceTrack2.volume = soundManager.musicVolume;
                            }
                        }
                        else if (tempsRestantMusic <= soundManager.fonduCroiseTime)
                        {
                            if (!track1IsPlaying)
                            {
                                mainMenuMusicAudioSourceTrack1.volume += soundManager.musicVolume / soundManager.fonduCroiseTime;
                                soundManager.PlayMainMenuMusicTrack1();
                                track1IsPlaying = true;
                            }

                            if (mainMenuMusicAudioSourceTrack2.volume - soundManager.musicVolume / soundManager.fonduCroiseTime > 0)
                            {
                                mainMenuMusicAudioSourceTrack2.volume -= soundManager.musicVolume / soundManager.fonduCroiseTime;
                            }
                            else
                            {
                                mainMenuMusicAudioSourceTrack2.volume = 0;
                            }
                        }
                    }
                    else
                    {
                        track2IsPlaying = false;
                    }

                    if (mainMenuMusicAudioSourceTrack1.isPlaying)
                    {
                        nbSecondeMusic1Joue++;
                        if (nbSecondeMusic1Joue == soundManager.fonduCroiseTime)
                        {
                            track2IsPlaying = false;
                            soundManager.mainMenuMusicAudioSourceTrack2.Stop();
                            nbSecondeMusic2Joue = 0;
                        }

                        float tempsRestantMusic = mainMenuMusicAudioSourceTrack1.clip.length - mainMenuMusicAudioSourceTrack1.time;
                        if (mainMenuMusicAudioSourceTrack1.time < soundManager.fonduCroiseTime)
                        {
                            if (mainMenuMusicAudioSourceTrack1.volume + soundManager.musicVolume / soundManager.fonduCroiseTime < soundManager.musicVolume)
                            {
                                mainMenuMusicAudioSourceTrack1.volume += soundManager.musicVolume / soundManager.fonduCroiseTime;
                            }
                            else
                            {
                                mainMenuMusicAudioSourceTrack1.volume = soundManager.musicVolume;
                            }
                        }
                        else if (tempsRestantMusic <= soundManager.fonduCroiseTime)
                        {
                            if (!track2IsPlaying)
                            {
                                mainMenuMusicAudioSourceTrack2.volume += soundManager.musicVolume / soundManager.fonduCroiseTime;
                                soundManager.PlayMainMenuMusicTrack2();
                                track2IsPlaying = true;
                            }

                            if (mainMenuMusicAudioSourceTrack1.volume - soundManager.musicVolume / soundManager.fonduCroiseTime > 0)
                            {
                                mainMenuMusicAudioSourceTrack1.volume -= soundManager.musicVolume / soundManager.fonduCroiseTime;
                            }
                            else
                            {
                                mainMenuMusicAudioSourceTrack1.volume = 0;
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
}
