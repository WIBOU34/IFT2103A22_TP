using StarterAssets;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField]
    private GameObject menu;
    [SerializeField]
    private GameObject pauseMenu;
    [SerializeField]
    private GameObject optionsMenu;

    private GameObject bindingInProcess;

    private GameObject player;

    public PlayerInput playerInput;

    private SoundManager soundManager;

    public void Start()
    {
        soundManager = SoundManager.Instance;
        menu = GameObject.Find("Menu");        
        pauseMenu = menu.transform.Find("Pause Menu").gameObject;
        optionsMenu = menu.transform.Find("Options Menu").gameObject;
        bindingInProcess = menu.transform.Find("BindingInProcess").gameObject;
        MenuManager.optionsMenu = optionsMenu;
        MenuManager.bindingInProcess = bindingInProcess;
        MenuManager.IsInitialised = true;
        player = GameObject.FindGameObjectsWithTag("Player").Where(x => x.GetComponent<StarterAssetsInputs>().pause).FirstOrDefault();
        if (player != null)
        {
            playerInput = player.GetComponent<PlayerInput>();
        }        
    }

    public void Pause()
    {
        soundManager.gameIsPaused = true;
        soundManager.PauseAudioSources();
        soundManager.PlayMainMenuMusicTrack1();
        pauseMenu.SetActive(true);
        playerInput.SwitchCurrentActionMap("Menu");
    }

    public void Resume()
    {
        soundManager.PlayMenuButtonOnClickSound();
        soundManager.gameIsPaused = false;
        pauseMenu.SetActive(false);
        playerInput.SwitchCurrentActionMap("Player");
        List<GameObject> players = GameObject.FindGameObjectsWithTag("Player").ToList();
        foreach (var player in players)
        {
            player.GetComponent<StarterAssetsInputs>().pause = false;
        }
        Time.timeScale = 1f;        
        soundManager.StopMainMenuMusic();
        soundManager.ResumeGameMusic();
    }

    public void Options()
    {
        soundManager.PlayMenuButtonOnClickSound();
        MenuManager.OptionsMenuOpenedFromPauseMenu = true;
        optionsMenu.SetActive(true);
        pauseMenu.SetActive(false);
    }

    public void LeaveGame()
    {
        soundManager.PlayMenuButtonOnClickSound();
        Time.timeScale = 1f;
        ZombieController.LeavingGame();
        GameLoader gameLoader = MenuManager.persistence.GetComponent<GameLoader>();
        MenuManager.IsInitialised = false;
        MenuManager.loadingScreen.SetActive(true);
        gameObject.SetActive(false);
        soundManager.ClearAudioSources();
        gameLoader.LoadGame(1);
    }
}
