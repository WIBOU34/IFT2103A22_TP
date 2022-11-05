using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleFieldMenuController : MonoBehaviour
{
    public void Scene1()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Scene2()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Back()
    {
        MenuManager.OpenMenu(Menu.GAME_MODE_MENU, gameObject);
    }
}
