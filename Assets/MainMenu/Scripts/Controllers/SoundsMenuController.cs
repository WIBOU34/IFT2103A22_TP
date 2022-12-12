using Newtonsoft.Json;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SoundsMenuController : MonoBehaviour
{
    private InputManager inputManager;
    private SoundManager soundManager;
    private string fileName = @".\soundsSettings.json";
    private SoundsViewModel soundsViewModel = new SoundsViewModel();
    private Slider musicSlider;
    private Slider FoleysSlider;
    private Slider SFXSlider;

    void Start()
    {
        inputManager = InputManager.Instance;
        soundManager = SoundManager.Instance;

        musicSlider = gameObject.transform.Find("Music Container").transform.Find("Slider").GetComponent<Slider>();
        FoleysSlider = gameObject.transform.Find("Foleys Container").transform.Find("Slider").GetComponent<Slider>();
        SFXSlider = gameObject.transform.Find("SFX Container").transform.Find("Slider").GetComponent<Slider>();

        Load();
    }

    public void OpenBindsMenu()
    {
        soundManager.PlayMenuButtonOnClickSound();

        if (MenuManager.OptionsMenuOpenedFromPauseMenu)
        {
            GameObject menu = GameObject.Find("Menu");
            GameObject optionsMenu = menu.transform.Find("Options Menu").gameObject;
            optionsMenu.SetActive(true);
            gameObject.SetActive(false);
        }
        else
        {
            MenuManager.OpenMenu(Menu.OPTIONS_MENU, gameObject);
        }        
    }

    public void Save()
    {
        soundManager.PlayMenuButtonOnClickSound();

        soundsViewModel.MusicVolume = musicSlider.value;
        soundsViewModel.FoleysVolume = FoleysSlider.value;
        soundsViewModel.SFXVolume = SFXSlider.value;

        string json = JsonConvert.SerializeObject(soundsViewModel);

        if (!File.Exists(fileName))
        {
            File.WriteAllText(fileName, json);
        }
        else
        {
            File.Delete(fileName);
            File.WriteAllText(fileName, json);
        }

        soundManager.UpdateMusicVolume(soundsViewModel.MusicVolume / 100);
        soundManager.UpdateFoleyVolume(soundsViewModel.FoleysVolume / 100);
        soundManager.UpdateSFXVolume(soundsViewModel.SFXVolume / 100);
    }

    public void Back()
    {
        Load();
        UpdateSoundsSliders();

        soundManager.PlayMenuButtonOnClickSound();

        if (MenuManager.OptionsMenuOpenedFromPauseMenu)
        {
            GameObject menu = GameObject.Find("Menu");
            GameObject pauseMenu = menu.transform.Find("Pause Menu").gameObject;
            pauseMenu.SetActive(true);
            gameObject.SetActive(false);
        }
        else
        {
            MenuManager.OpenMenu(Menu.MAIN_MENU, gameObject);
        }
    }

    private void Load()
    {
        if (File.Exists(fileName))
        {
            string json = File.ReadAllText(fileName);
            soundsViewModel = JsonConvert.DeserializeObject<SoundsViewModel>(json);
        }
        else
        {
            InitSoundsViewModel();
            Save();
        }

        musicSlider.value = soundsViewModel.MusicVolume;
        FoleysSlider.value = soundsViewModel.FoleysVolume;
        SFXSlider.value = soundsViewModel.SFXVolume;
    }

    private void InitSoundsViewModel()
    {
        soundsViewModel = new SoundsViewModel
        {
            MusicVolume = 100,
            FoleysVolume = 100,
            SFXVolume = 100
        };
    }

    private void UpdateSoundsSliders()
    {
        musicSlider.value = soundsViewModel.MusicVolume;
        FoleysSlider.value = soundsViewModel.FoleysVolume;
        SFXSlider.value = soundsViewModel.SFXVolume;
    }
}
