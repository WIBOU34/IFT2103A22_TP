using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelBasicStart : MonoBehaviour
{
    public GameObject zombieTypeToSpawn;
    public ZombieController zombieController;
    public GameObject prefabPlayer;
    private List<GameObject> players;
    public List<GameObject> weapons;
    public Material bulletTrailMaterial;
    // Start is called before the first frame update
    void Start()
    {
        CreatePlayers(1);

        zombieController = new ZombieController();
        zombieController.Start();
        ZombieController.typeToSpawn = zombieTypeToSpawn;
        this.gameObject.AddComponent<MapCreator>();
        ZombieController.CreateZombieSpawner(new Vector3(-10, 0, 5));

        GameObject hud = GameObject.Find("HUD");
        hud.AddComponent<HealthBarManager>();        
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void CreatePlayers(int nbrPlayers)
    {
        for (int i = 0; i < nbrPlayers; i++)
        {
            GameObject.Instantiate(prefabPlayer);
        }

        players = GameObject.FindGameObjectsWithTag("Player").ToList();
        int playerNumber = 0;
        foreach (var player in players)
        {
            player.AddComponent<PlayableCharacter>();
            PlayableCharacter playableCharacter = player.GetComponent<PlayableCharacter>();
            playableCharacter.weapons = weapons;
            playableCharacter.playerNumber = playerNumber + 1;
            player.AddComponent<PauseMenuController>().playerInput = player.GetComponent<PlayerInput>();
        }
    }
}
