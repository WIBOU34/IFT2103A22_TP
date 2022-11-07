using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public void Resume()
    {
        MenuManager.IsPaused = false;
        MenuManager.IsInitialised = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Options()
    {
        MenuManager.OpenMenu(Menu.OPTIONS_MENU, gameObject);
        MenuManager.OptionsMenuOpenedFromPauseMenu = true;
    }

    public void LeaveGame()
    {
        MenuManager.OpenMenu(Menu.MAIN_MENU, gameObject);
    }
}
