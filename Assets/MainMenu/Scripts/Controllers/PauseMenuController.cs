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
        AudioSource pauseMenuMusicAudioSource = menu.AddComponent<AudioSource>();
        soundManager.mainMenuMusicAudioSource = pauseMenuMusicAudioSource;
        AudioSource menuButtonEffectsAudioSource = menu.AddComponent<AudioSource>();
        soundManager.menuButtonEffectsAudioSource = menuButtonEffectsAudioSource;
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
        soundManager.PauseAudioSources();
        soundManager.PlayMainMenuMusic();
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        playerInput.SwitchCurrentActionMap("Menu");
    }

    public void Resume()
    {
        soundManager.PlayMenuButtonOnClickSound();
        pauseMenu.SetActive(false);
        playerInput.SwitchCurrentActionMap("Player");
        List<GameObject> players = GameObject.FindGameObjectsWithTag("Player").ToList();
        foreach (var player in players)
        {
            player.GetComponent<StarterAssetsInputs>().pause = false;
        }
        Time.timeScale = 1f;        
        soundManager.ResumeAudioSources();
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
