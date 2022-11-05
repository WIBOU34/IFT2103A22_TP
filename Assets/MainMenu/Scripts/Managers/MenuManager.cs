using UnityEngine;

public static class MenuManager
{
    public static bool IsInitialised { get; private set; }
    public static GameObject mainMenu, optionsMenu, gameModeMenu, battlefieldMenu;

    public static void Init()
    {
        GameObject canvas = GameObject.Find("Canvas");
        mainMenu = canvas.transform.Find("Main Menu").gameObject;
        optionsMenu = canvas.transform.Find("Options Menu").gameObject;
        gameModeMenu = canvas.transform.Find("Game Mode Menu").gameObject;
        battlefieldMenu = canvas.transform.Find("Battlefield Menu").gameObject;

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
            case Menu.BATTLEFIELD_MENU:
                battlefieldMenu.SetActive(true);
                break;
        }

        callingMenu.SetActive(false);
    }
}
