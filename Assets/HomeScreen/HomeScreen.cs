using UnityEngine;

public class HomeScreen : MonoBehaviour
{
    private void Start()
    {
        MenuManager.loadingScreen = GameObject.Find("LoadingScreen");
    }

    void Update()
    {
        if (Input.anyKey)
        {
            MenuManager.OpenMenu(Menu.MAIN_MENU, gameObject);
        }
    }
}
