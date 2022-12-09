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

    // Start is called before the first frame update
    public void Start()
    {
        soundManager = SoundManager.Instance;
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
    }

    void OnDamageTaken(float health)
    {     
        playerHealthBar.value = health;
        if (playerHealthBar.value > 0)
        {
            soundManager.PlayerDamageTakenSound();
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
