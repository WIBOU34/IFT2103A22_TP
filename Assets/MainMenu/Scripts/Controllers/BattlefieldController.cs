using UnityEngine;

public class BattlefieldController : MonoBehaviour
{
    private SoundManager soundManager;
    private Vector3 initialButtonScale = new Vector3(3, 3, 0);

    private void Awake()
    {
        soundManager = SoundManager.Instance;
    }
    
    public void Basic(GameObject button)
    {
        soundManager.PlayMenuButtonOnClickSound();
        MenuManager.IsInitialised = false;
        ActivateLoadingScreenToLoadGame(1, 2);
        button.transform.localScale = initialButtonScale;
    }

    public void Random(GameObject button)
    {
        soundManager.PlayMenuButtonOnClickSound();
        MenuManager.IsInitialised = false;
        ActivateLoadingScreenToLoadGame(1, 3);
        button.transform.localScale = initialButtonScale;
    }

    public void Back(GameObject button)
    {
        soundManager.PlayMenuButtonOnClickSound();
        MenuManager.OpenMenu(Menu.GAME_MODE_MENU, gameObject);
        button.transform.localScale = initialButtonScale;
    }

    private void ActivateLoadingScreenToLoadGame(int totalPlayers, int sceneIndex)
    {
        GameLoader gameLoader = MenuManager.persistence.GetComponent<GameLoader>();
        gameLoader.totalPlayers = totalPlayers;
        MenuManager.loadingScreen.SetActive(true);
        gameObject.SetActive(false);
        soundManager.ClearAudioSources();
        gameLoader.LoadGame(sceneIndex);
    }
}
