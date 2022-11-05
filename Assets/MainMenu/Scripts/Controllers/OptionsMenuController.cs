using UnityEngine;

public class OptionsMenuController : MonoBehaviour
{
    public void Back()
    {
        MenuManager.OpenMenu(Menu.MAIN_MENU, gameObject);
    }
}
