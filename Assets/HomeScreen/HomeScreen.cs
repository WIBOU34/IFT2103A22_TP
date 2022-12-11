using UnityEngine;

public class HomeScreen : MonoBehaviour
{
    private SoundManager soundManager;

    private void Start()
    {
        MenuManager.loadingScreen = GameObject.Find("LoadingScreen").gameObject;
        MenuManager.persistence = GameObject.Find("Persistence").gameObject;
        GameObject canvas = GameObject.Find("Canvas");
        AudioSource mainMenuMusicAudioSource = canvas.GetComponent<AudioSource>();
        soundManager = SoundManager.Instance;
        soundManager.mainMenuMusicAudioSource = mainMenuMusicAudioSource;
        soundManager.PlayMainMenuMusic();
    }

    void Update()
    {
        if (Input.anyKey)
        {
            MenuManager.OpenMenu(Menu.MAIN_MENU, gameObject);
        }
    }
}
