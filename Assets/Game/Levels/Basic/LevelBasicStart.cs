using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        foreach (var player in players)
        {
            player.AddComponent<PlayableCharacter>();
            player.GetComponent<PlayableCharacter>().weapons = weapons;
            player.GetComponent<PlayableCharacter>().bulletTrailMaterial = bulletTrailMaterial;

        }
    }
}
