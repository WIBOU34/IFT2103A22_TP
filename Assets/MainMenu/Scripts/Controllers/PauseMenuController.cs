using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField]
    private GameObject menu = GameObject.Find("Menu");
    [SerializeField]
    private GameObject pauseMenu = menu.transform.Find("Pause Menu").gameObject;
    [SerializeField]
    private GameObject optionsMenu = GameObject.Find("Menu").transform.Find("Options Menu").gameObject;

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        //MenuManager.IsPaused = false;
        //MenuManager.IsInitialised = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Options()
    {
        //MenuManager.OpenMenu(Menu.OPTIONS_MENU, gameObject);
        //MenuManager.OptionsMenuOpenedFromPauseMenu = true;
    }

    public void LeaveGame()
    {
        Time.timeScale = 1f;
        //MenuManager.OpenMenu(Menu.MAIN_MENU, gameObject);
    }
}
