using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    private SoundManager soundManager;
    private Vector3 buttonsInitialScale = new Vector3(3, 3, 0);

    private void Start()
    {
        soundManager = SoundManager.Instance;
    }

    public void Play(GameObject button)
    {
        soundManager.PlayMenuButtonOnClickSound();
        MenuManager.OpenMenu(Menu.DIFFICULTY_MENU, gameObject);
        button.transform.localScale = buttonsInitialScale;
    }

    public void Options(GameObject button)
    {
        soundManager.PlayMenuButtonOnClickSound();
        MenuManager.OpenMenu(Menu.OPTIONS_MENU, gameObject);
        MenuManager.OptionsMenuOpenedFromPauseMenu = false;
        button.transform.localScale = buttonsInitialScale;
    }

    public void Quit(GameObject button)
    {
        soundManager.PlayMenuButtonOnClickSound();
        Application.Quit();
        button.transform.localScale = buttonsInitialScale;
    }
}
