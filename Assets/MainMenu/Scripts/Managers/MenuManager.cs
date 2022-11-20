using UnityEngine;

public static class MenuManager
{
    public static bool IsInitialised { get; set; }
    public static bool OptionsMenuOpenedFromPauseMenu { get; set; }
    public static GameObject homeScreen, loadingScreen, mainMenu, optionsMenu, gameModeMenu, persistence;

    public static void Init()
    {
        GameObject canvas = GameObject.Find("Canvas");
        homeScreen = canvas.transform.Find("HomeScreen").gameObject;
        mainMenu = canvas.transform.Find("Main Menu").gameObject;
        optionsMenu = canvas.transform.Find("Options Menu").gameObject;
        gameModeMenu = canvas.transform.Find("Game Mode Menu").gameObject;

        IsInitialised = true;
    }

    public static void OpenMenu(Menu menu, GameObject callingMenu)
    {
        if (!IsInitialised)
        {
            Init();
        }

        switch (menu)
        {
            case Menu.MAIN_MENU:
                mainMenu.SetActive(true);
                break;
            case Menu.OPTIONS_MENU:
                optionsMenu.SetActive(true);
                break;
            case Menu.GAME_MODE_MENU:
                gameModeMenu.SetActive(true);
                break;
        }

        callingMenu.SetActive(false);
    }
}
