using UnityEngine;

public static class MenuManager
{
    public static bool IsInitialised { get; set; }
    public static bool OptionsMenuOpenedFromPauseMenu { get; set; }
    public static GameObject homeScreen, loadingScreen, mainMenu, optionsMenu, gameModeMenu, persistence, bindingInProcess, difficultyMenu, gameOverScreen, soundsMenu;

    public static void Init()
    {
        GameObject canvas = GameObject.Find("Canvas");
        homeScreen = canvas.transform.Find("HomeScreen").gameObject;
        mainMenu = canvas.transform.Find("Main Menu").gameObject;
        optionsMenu = canvas.transform.Find("Options Menu").gameObject;
        difficultyMenu = canvas.transform.Find("Difficulty Menu").gameObject;
        gameModeMenu = canvas.transform.Find("Game Mode Menu").gameObject;
        bindingInProcess = canvas.transform.Find("BindingInProcess").gameObject;
        soundsMenu = canvas.transform.Find("Sounds Menu").gameObject;

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
            case Menu.BINDING_IN_PROCRESS:
                bindingInProcess.SetActive(true);
                break;
            case Menu.DIFFICULTY_MENU:
                difficultyMenu.SetActive(true);
                break;
            case Menu.SOUNDS_MENU:
                soundsMenu.SetActive(true);
                break;
        }

        SetCursorState(false);

        callingMenu.SetActive(false);
    }

    private static void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
