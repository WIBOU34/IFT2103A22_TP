using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public void Play()
    {        
        MenuManager.OpenMenu(Menu.DIFFICULTY_MENU, gameObject);
    }

    public void Options()
    {
        MenuManager.OpenMenu(Menu.OPTIONS_MENU, gameObject);
        MenuManager.OptionsMenuOpenedFromPauseMenu = false;
    }

    public void Quit()
    {
        Application.Quit();
    }
}
