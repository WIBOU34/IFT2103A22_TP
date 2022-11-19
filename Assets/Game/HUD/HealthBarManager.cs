using Newtonsoft.Json.Bson;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarManager : MonoBehaviour
{
    private GameObject healthBar1;
    private Slider player1HealthBar;
    private Damageable player1Health;
    private GameObject healthBar2;
    private Slider player2HealthBar;
    private Damageable player2Health;

    // Start is called before the first frame update
    public void Start()
    {
        List<GameObject> players = new List<GameObject>();
        players = GameObject.FindGameObjectsWithTag("Player").ToList();

        foreach (GameObject player in players)
        {
            PlayableCharacter playableCharacter = player.GetComponent<PlayableCharacter>();
            if (playableCharacter.playerNumber == 1)
            {
                player1Health = player.GetComponent<Damageable>();
            }
            else if (playableCharacter.playerNumber == 2)
            {
                player2Health = player.GetComponent<Damageable>();
            }
        }

        GameObject player1Hud = gameObject.transform.GetChild(0).gameObject;
        healthBar1 = player1Hud.gameObject.transform.GetChild(1).gameObject;
        player1HealthBar = healthBar1.GetComponent<Slider>();

        GameObject player2Hud = gameObject.transform.GetChild(1).gameObject;
        healthBar2 = player2Hud.gameObject.transform.GetChild(1).gameObject;
        player2HealthBar = healthBar2.GetComponent<Slider>();

        player1HealthBar.value = player1Health.health;

        if (players.Count > 1)
        {
            gameObject.transform.GetChild(1).gameObject.SetActive(true);
            player2HealthBar.value = player2Health.health;
        }

        HidePlayersHealthBarIfZero();
        ShowPlayersHealthBarIfGreaterThanZero();
    }

    // Update is called once per frame
    void Update()
    {
        player1HealthBar.value = player1Health.health;

        if (player2Health!= null)
        {
            player2HealthBar.value = player2Health.health;
        }
        
        HidePlayersHealthBarIfZero();
        ShowPlayersHealthBarIfGreaterThanZero();
    }

    private void HidePlayersHealthBarIfZero()
    {
        if (player1HealthBar.value == 0)
        {
            healthBar1.SetActive(false);
        }

        if (player2HealthBar.value == 0)
        {
            healthBar2.SetActive(false);
        }
    }

    private void ShowPlayersHealthBarIfGreaterThanZero()
    {
        if (player1HealthBar.value > 0)
        {
            healthBar1.SetActive(true);
        }

        if (player2HealthBar.value > 0)
        {
            healthBar2.SetActive(true);
        }
    } 
}
