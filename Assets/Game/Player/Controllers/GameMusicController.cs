using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMusicController : MonoBehaviour
{
    private SoundManager soundManager;
    private AudioSource gameMusicAudioSource;
    private float nextTime = 0;

    private void Start()
    {
        GameObject canvas = GameObject.Find("Canvas");
        gameMusicAudioSource = canvas.GetComponent<AudioSource>();
        soundManager = SoundManager.Instance;
        soundManager.gameMusicAudioSource = gameMusicAudioSource;
        soundManager.PlayGameMusic();
    }

    private void Update()
    {
        if (Time.time >= nextTime)
        {
            if (gameMusicAudioSource.isPlaying)
            {
                float tempsRestantMusic = gameMusicAudioSource.clip.length - gameMusicAudioSource.time;
                if (gameMusicAudioSource.time < soundManager.fonduCroiseTimeGame)
                {
                    gameMusicAudioSource.volume += soundManager.musicVolume / soundManager.fonduCroiseTimeGame;
                }
                else if (tempsRestantMusic < soundManager.fonduCroiseTimeGame)
                {
                    gameMusicAudioSource.volume -= gameMusicAudioSource.volume / soundManager.fonduCroiseTimeGame;
                }
            }
            else
            {
                soundManager.PlayGameMusic();
            }

            nextTime += 1;
        }
    }
}
