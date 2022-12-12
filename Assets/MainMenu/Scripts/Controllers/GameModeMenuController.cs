using UnityEngine;

public class GameModeMenuController : MonoBehaviour
{
    private SoundManager soundManager;

    private void Start()
    {
        soundManager = SoundManager.Instance;
    }

    public void Solo()
    {
        soundManager.PlayMenuButtonOnClickSound();
        MenuManager.IsInitialised = false;
        ActivateLoadingScreenToLoadGame(1);
    }

    public void Multiplayer()
    {
        soundManager.PlayMenuButtonOnClickSound();
        MenuManager.IsInitialised = false;
        ActivateLoadingScreenToLoadGame(2);
    }

    public void Back()
    {
        soundManager.PlayMenuButtonOnClickSound();
        MenuManager.OpenMenu(Menu.DIFFICULTY_MENU, gameObject);
    }

    private void ActivateLoadingScreenToLoadGame(int totalPlayers)
    {
        GameLoader gameLoader = MenuManager.persistence.GetComponent<GameLoader>();
        gameLoader.totalPlayers = totalPlayers;
        MenuManager.loadingScreen.SetActive(true);
        gameObject.SetActive(false);
        soundManager.ClearAudioSources();
        gameLoader.LoadGame(2);
    }
}
