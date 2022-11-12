using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleFieldMenuController : MonoBehaviour
{
    public void Scene1()
    {
        MenuManager.IsInitialised = false;
        SceneManager.LoadScene(2);
    }

    public void Scene2()
    {
        MenuManager.IsInitialised = false;
        SceneManager.LoadScene(2);
    }

    public void Back()
    {
        MenuManager.OpenMenu(Menu.GAME_MODE_MENU, gameObject);
    }
}
