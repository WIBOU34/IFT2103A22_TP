using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarManager : MonoBehaviour
{
    public int playerNumber;
    private GameObject healthBar;
    private Slider playerHealthBar;
    private TextMeshProUGUI playerText;
    private Damageable playerHealth;
    private SoundManager soundManager;
    private AudioSource lowHealthGameMusicSource;
    private float lastHitTime;
    private float regenerateHealthTime = 10;
    private float regenerateHealthValue = 20;

    // Start is called before the first frame update
    public void Start()
    {
        soundManager = SoundManager.Instance;
        lowHealthGameMusicSource = gameObject.AddComponent<AudioSource>();
        soundManager.lowHealthGameMusicSource = lowHealthGameMusicSource;
        playerHealth = gameObject.GetComponent<Damageable>();

        GameObject hud = GameObject.Find("HUD");

        GameObject playerHud = hud.gameObject.transform.GetChild(playerNumber - 1).gameObject;
        playerHud.SetActive(true);
        healthBar = playerHud.gameObject.transform.GetChild(1).gameObject;
        playerHealthBar = healthBar.GetComponent<Slider>();
        playerText = playerHud.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        playerHealthBar.value = playerHealth.health;
        HidePlayersHealthBarIfZero();
        ShowPlayersHealthBarIfGreaterThanZero();
    }

    // Update is called once per frame
    void Update()
    {
        HidePlayersHealthBarIfZero();
        ShowPlayersHealthBarIfGreaterThanZero();

        if (playerHealthBar.value > 0 && playerHealthBar.value < 100)
        {
            if (Time.time - lastHitTime >= regenerateHealthTime)
            {
                lastHitTime = Time.time;
                if (playerHealthBar.value + regenerateHealthValue <= 100)
                {
                    playerHealth.health += regenerateHealthValue;
                    playerHealthBar.value += regenerateHealthValue;
                }                
                else
                {
                    playerHealth.health = 100;
                    playerHealthBar.value = 100;
                }

                if (soundManager.lowHealthGameMusicSource.isPlaying && playerHealthBar.value >= 25)
                {
                    soundManager.StopLowHealthMusicAndResumeGameMusic();
                }
            }
        }
    }

    void OnDamageTaken(float health)
    {
        lastHitTime = Time.time;
        playerHealthBar.value = health;        

        if (playerHealthBar.value > 0)
        {
            soundManager.PlayerDamageTakenSound();
        }

        if (playerHealthBar.value > 0 && playerHealthBar.value < 25)
        {
            soundManager.PlayLowHealthMusic();
        }
    }

    private void HidePlayersHealthBarIfZero()
    {
        if (playerHealthBar.value == 0)
        {
            healthBar.SetActive(false);
            playerText.text = "Game Over";
        }
    }

    private void ShowPlayersHealthBarIfGreaterThanZero()
    {
        if (playerHealthBar.value > 0)
        {
            healthBar.SetActive(true);
            playerText.text = "Player " + playerNumber;
        }
    }
}
