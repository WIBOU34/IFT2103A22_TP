using UnityEngine;

public class GameModeMenuController : MonoBehaviour
{
    public void Solo()
    {
        MenuManager.IsInitialised = false;
        ActivateLoadingScreenToLoadGame(1);
    }

    public void Multiplayer()
    {
        MenuManager.IsInitialised = false;
        ActivateLoadingScreenToLoadGame(2);
    }

    public void Back()
    {
        MenuManager.OpenMenu(Menu.DIFFICULTY_MENU, gameObject);
    }

    private void ActivateLoadingScreenToLoadGame(int totalPlayers)
    {
        GameLoader gameLoader = MenuManager.persistence.GetComponent<GameLoader>();
        gameLoader.totalPlayers = totalPlayers;
        MenuManager.loadingScreen.SetActive(true);
        gameObject.SetActive(false);
        gameLoader.LoadGame(2);
    }
}
