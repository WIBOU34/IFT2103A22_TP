using UnityEngine;

public class OptionsMenuController : MonoBehaviour
{
    public void Back()
    {
        if (MenuManager.OptionsMenuOpenedFromPauseMenu)
        {
            MenuManager.OpenMenu(Menu.PAUSE_MENU, gameObject);
        }
        else
        {
            MenuManager.OpenMenu(Menu.MAIN_MENU, gameObject);
        }        
    }
}
