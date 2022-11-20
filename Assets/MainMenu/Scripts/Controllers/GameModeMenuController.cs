using UnityEngine;

public class GameModeMenuController : MonoBehaviour
{
    public void Solo()
    {
        MenuManager.IsInitialised = false;
        ActivateLoadingScreenToLoadGame();
    }

    public void Multiplayer()
    {
        MenuManager.IsInitialised = false;
        ActivateLoadingScreenToLoadGame();
    }

    public void Back()
    {
        MenuManager.OpenMenu(Menu.MAIN_MENU, gameObject);
    }

    private void ActivateLoadingScreenToLoadGame()
    {        
        GameLoader gameLoader = MenuManager.loadingScreen.GetComponent<GameLoader>();        
        MenuManager.loadingScreen.SetActive(true);
        gameObject.SetActive(false);
        gameLoader.LoadGame(2);
    }
}
