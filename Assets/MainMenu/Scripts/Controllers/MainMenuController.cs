using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    private SoundManager soundManager;

    private void Start()
    {
        soundManager = SoundManager.Instance;
    }

    public void Play()
    {
        soundManager.PlayMenuButtonOnClickSound();
        MenuManager.OpenMenu(Menu.DIFFICULTY_MENU, gameObject);
    }

    public void Options()
    {
        soundManager.PlayMenuButtonOnClickSound();
        MenuManager.OpenMenu(Menu.OPTIONS_MENU, gameObject);
        MenuManager.OptionsMenuOpenedFromPauseMenu = false;
    }

    public void Quit()
    {
        soundManager.PlayMenuButtonOnClickSound();
        Application.Quit();
    }
}
