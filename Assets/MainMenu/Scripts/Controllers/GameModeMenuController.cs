using UnityEngine;

public class GameModeMenuController : MonoBehaviour
{
    public void Solo()
    {
        MenuManager.OpenMenu(Menu.BATTLEFIELD_MENU, gameObject);
    }

    public void Multiplayer()
    {
        MenuManager.OpenMenu(Menu.BATTLEFIELD_MENU, gameObject);
    }

    public void Back()
    {
        MenuManager.OpenMenu(Menu.MAIN_MENU, gameObject);
    }
}
