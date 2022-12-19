using System.Collections;
using TMPro;
using UnityEngine;

public class GameOverController : MonoBehaviour
{
    private const string SURVIVED = "Congratulations\nYou Survived";
    private const string PERISHED = "Game Over\nYou Died";
    private const string NBR_SECONDS_UNTIL_MENU = "\n\nReturning to menu in 5 seconds";
    private SoundManager soundManager;
    private AudioSource gameEndingAudioSource;

    private void Awake()
    {
        soundManager = SoundManager.Instance;
        gameEndingAudioSource = gameObject.AddComponent<AudioSource>();
        soundManager.gameEndingMusic = gameEndingAudioSource;
    }

    public void Start()
    {
    }

    public void Init(bool win)
    {
        ZombieController.LeavingGame();
        GameObject textField = gameObject.transform.Find("Title").gameObject;
        if (win)
        {
            textField.GetComponent<TextMeshProUGUI>().text = SURVIVED + NBR_SECONDS_UNTIL_MENU;
            soundManager.PlayVictoryMusic();
        }
        else
        {
            textField.GetComponent<TextMeshProUGUI>().text = PERISHED + NBR_SECONDS_UNTIL_MENU;
            soundManager.PlayGameOverMusic();
        }

        this.gameObject.SetActive(true);
        MenuManager.loadingScreen.SetActive(false);

        StartCoroutine(LeaveGameWait());
    }

    private IEnumerator LeaveGameWait()
    {
        yield return new WaitForSeconds(5);
        this.LeaveGame();
    }

    private void LeaveGame()
    {
        Time.timeScale = 1f;
        GameLoader gameLoader = MenuManager.persistence.GetComponent<GameLoader>();
        MenuManager.IsInitialised = false;
        soundManager.StopAllGameAudioSources();
        MenuManager.loadingScreen.SetActive(true);
        gameObject.SetActive(false);
        gameLoader.LoadGame(1);
    }
}
