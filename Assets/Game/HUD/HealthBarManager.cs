using UnityEngine;
using UnityEngine.UI;

public class HealthBarManager : MonoBehaviour
{
    public int playerNumber;
    private GameObject healthBar;
    private Slider playerHealthBar;
    private Damageable playerHealth;

    // Start is called before the first frame update
    public void Start()
    {
        playerHealth = gameObject.AddComponent<Damageable>();

        GameObject hud = GameObject.Find("HUD");

        GameObject playerHud = hud.gameObject.transform.GetChild(playerNumber - 1).gameObject;
        playerHud.SetActive(true);
        healthBar = playerHud.gameObject.transform.GetChild(1).gameObject;
        playerHealthBar = healthBar.GetComponent<Slider>();

        playerHealthBar.value = playerHealth.health;
        HidePlayersHealthBarIfZero();
        ShowPlayersHealthBarIfGreaterThanZero();
    }

    // Update is called once per frame
    void Update()
    {
        playerHealthBar.value = playerHealth.health;
        HidePlayersHealthBarIfZero();
        ShowPlayersHealthBarIfGreaterThanZero();
    }

    private void HidePlayersHealthBarIfZero()
    {
        if (playerHealthBar.value == 0)
        {
            healthBar.SetActive(false);
        }
    }

    private void ShowPlayersHealthBarIfGreaterThanZero()
    {
        if (playerHealthBar.value > 0)
        {
            healthBar.SetActive(true);
        }
    }
}
