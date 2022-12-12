using UnityEngine;

public class DifficultyMenuController : MonoBehaviour
{
    private GameLoader gameLoader;
    private SoundManager soundManager;

    void Start()
    {
        gameLoader = MenuManager.persistence.GetComponent<GameLoader>();
        soundManager = SoundManager.Instance;
    }

    public void Easy()
    {
        soundManager.PlayMenuButtonOnClickSound();
        gameLoader.difficulty = Difficulty.EASY;
        MenuManager.OpenMenu(Menu.GAME_MODE_MENU, gameObject);
    }

    public void Intermediate()
    {
        soundManager.PlayMenuButtonOnClickSound();
        gameLoader.difficulty = Difficulty.INTERMEDIATE;
        MenuManager.OpenMenu(Menu.GAME_MODE_MENU, gameObject);
    }

    public void Back()
    {
        soundManager.PlayMenuButtonOnClickSound();
        MenuManager.OpenMenu(Menu.MAIN_MENU, gameObject);
    }
}
