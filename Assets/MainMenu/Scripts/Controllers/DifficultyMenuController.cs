using UnityEngine;

public class DifficultyMenuController : MonoBehaviour
{
    private GameLoader gameLoader;

    void Start()
    {
        gameLoader = MenuManager.persistence.GetComponent<GameLoader>();
    }

    public void Easy()
    {
        gameLoader.difficulty = Difficulty.EASY;
        MenuManager.OpenMenu(Menu.GAME_MODE_MENU, gameObject);
    }

    public void Intermediate()
    {
        gameLoader.difficulty = Difficulty.INTERMEDIATE;
        MenuManager.OpenMenu(Menu.GAME_MODE_MENU, gameObject);
    }

    public void Back()
    {
        MenuManager.OpenMenu(Menu.MAIN_MENU, gameObject);
    }
}
