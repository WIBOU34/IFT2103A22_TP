using UnityEngine;

public class MenuMusicController : MonoBehaviour
{
    private SoundManager soundManager;
    private AudioSource mainMenuMusicAudioSource;
    private float nextTime = 0;

    private void Start()
    {
        GameObject canvas = GameObject.Find("Canvas");
        mainMenuMusicAudioSource = canvas.GetComponent<AudioSource>();
        AudioSource menuButtonEffectsAudioSource = canvas.AddComponent<AudioSource>();
        soundManager = SoundManager.Instance;
        soundManager.mainMenuMusicAudioSource = mainMenuMusicAudioSource;
        soundManager.menuButtonEffectsAudioSource = menuButtonEffectsAudioSource;
        soundManager.PlayMainMenuMusic();
    }

    private void Update()
    {
        if (Time.time >= nextTime)
        {
            if (mainMenuMusicAudioSource.isPlaying)
            {
                float tempsRestantMusic = mainMenuMusicAudioSource.clip.length - mainMenuMusicAudioSource.time;
                if (mainMenuMusicAudioSource.time < soundManager.fonduCroiseTime)
                {
                    mainMenuMusicAudioSource.volume += soundManager.musicVolume / soundManager.fonduCroiseTime;
                }                
                else if (tempsRestantMusic < soundManager.fonduCroiseTime)
                {                    
                    mainMenuMusicAudioSource.volume -= mainMenuMusicAudioSource.volume / soundManager.fonduCroiseTime;
                }
            }
            else
            {
                soundManager.PlayMainMenuMusic();
            }

            nextTime += 1;
        }
    }
}
