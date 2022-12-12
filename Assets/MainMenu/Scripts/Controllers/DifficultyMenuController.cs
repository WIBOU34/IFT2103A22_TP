using UnityEngine;

public class DifficultyMenuController : MonoBehaviour
{
    private GameLoader gameLoader;
    private SoundManager soundManager;
    private Vector3 initialButtonScale = new Vector3(3, 3, 0);

    void Start()
    {
        gameLoader = MenuManager.persistence.GetComponent<GameLoader>();
        soundManager = SoundManager.Instance;
    }

    public void Easy(GameObject button)
    {
        soundManager.PlayMenuButtonOnClickSound();
        gameLoader.difficulty = Difficulty.EASY;
        MenuManager.OpenMenu(Menu.GAME_MODE_MENU, gameObject);
        button.transform.localScale = initialButtonScale; 
    }

    public void Intermediate(GameObject button)
    {
        soundManager.PlayMenuButtonOnClickSound();
        gameLoader.difficulty = Difficulty.INTERMEDIATE;
        MenuManager.OpenMenu(Menu.GAME_MODE_MENU, gameObject);
        button.transform.localScale = initialButtonScale;
    }

    public void Back(GameObject button)
    {
        soundManager.PlayMenuButtonOnClickSound();
        MenuManager.OpenMenu(Menu.MAIN_MENU, gameObject);
        button.transform.localScale = initialButtonScale;
    }
}
