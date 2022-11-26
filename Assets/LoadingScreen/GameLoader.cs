using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameLoader : MonoBehaviour
{
    public GameObject loadingScreen;
    public Slider loadingBar;
    public TextMeshProUGUI progessText;
    public int sceneIndex = 1;
    public int totalPlayers = 1;
    public Difficulty difficulty = Difficulty.EASY;

    public void Start()
    {
        gameObject.SetActive(true);
        MenuManager.gameOverScreen = GameObject.Find("GameOverScreen").gameObject;
        MenuManager.gameOverScreen.SetActive(false);
        DontDestroyOnLoad(gameObject);
        LoadGame(sceneIndex);
    }

    public void LoadGame(int sceneIndex)
    {
        StartCoroutine(LoadAsync(sceneIndex));
    }

    IEnumerator LoadAsync(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        loadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            float progess = Mathf.Clamp01(operation.progress / 0.9f);

            loadingBar.value = progess;
            progessText.text = progess * 100f + "%";

            yield return null;
        }

        loadingScreen.SetActive(false);
    }
}
