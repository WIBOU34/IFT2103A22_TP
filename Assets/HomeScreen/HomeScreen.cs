using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeScreen : MonoBehaviour
{
    void Update()
    {
        if (Input.anyKey)
        {
            MenuManager.OpenMenu(Menu.MAIN_MENU, gameObject);
        }
    }
}