using StarterAssets;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField]
    private GameObject menu;
    [SerializeField]
    private GameObject pauseMenu;
    [SerializeField]
    private GameObject optionsMenu;

    private GameObject player;
    
    public PlayerInput playerInput;

    public void Start()
    {
        menu = GameObject.Find("Menu");
        pauseMenu = menu.transform.Find("Pause Menu").gameObject;
        optionsMenu = GameObject.Find("Menu").transform.Find("Options Menu").gameObject;
        player = GameObject.FindGameObjectsWithTag("Player").Where(x => x.GetComponent<StarterAssetsInputs>().pause).FirstOrDefault();
        if (player != null)
        {
            playerInput = player.GetComponent<PlayerInput>();
        }        
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        playerInput.SwitchCurrentActionMap("Menu");       
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        playerInput.SwitchCurrentActionMap("Player");
        List<GameObject> players = GameObject.FindGameObjectsWithTag("Player").ToList();
        foreach (var player in players)
        {
            player.GetComponent<StarterAssetsInputs>().pause = false;
        }
        Time.timeScale = 1f;
    }

    public void Options()
    {
        MenuManager.OpenMenu(Menu.OPTIONS_MENU, gameObject);
        MenuManager.OptionsMenuOpenedFromPauseMenu = true;
    }

    public void LeaveGame()
    {
        Time.timeScale = 1f;
        MenuManager.OpenMenu(Menu.MAIN_MENU, gameObject);
    }
}
