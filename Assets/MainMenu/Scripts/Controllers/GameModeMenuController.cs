using UnityEngine;

public class GameModeMenuController : MonoBehaviour
{
    private SoundManager soundManager;
    private Vector3 initialButtonScale = new Vector3(3, 3, 0);

    private void Start()
    {
        soundManager = SoundManager.Instance;
    }

    public void Solo(GameObject button)
    {
        soundManager.PlayMenuButtonOnClickSound();
        MenuManager.IsInitialised = false;
        ActivateLoadingScreenToLoadGame(1);
        button.transform.localScale = initialButtonScale;
    }

    public void Multiplayer(GameObject button)
    {
        soundManager.PlayMenuButtonOnClickSound();
        MenuManager.IsInitialised = false;
        ActivateLoadingScreenToLoadGame(2);
        button.transform.localScale = initialButtonScale;
    }

    public void Back(GameObject button)
    {
        soundManager.PlayMenuButtonOnClickSound();
        MenuManager.OpenMenu(Menu.DIFFICULTY_MENU, gameObject);
        button.transform.localScale = initialButtonScale;
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
